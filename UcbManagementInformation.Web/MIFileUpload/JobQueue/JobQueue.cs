// Communications between asynchronous threads is a challenge 
    // with a high level of complexity
    // Sometimes, all the application really needs is to control
    // a background worker process that executes
    // sequential operations
    // this is an example class for such a design patternusing System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Threading;
// rather than use console for output, I like Debug
    // this method sends output to the Visual Studio output window
    // which persists after the console application has closed
    using System.Diagnostics;
using System;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using UcbManagementInformation.Web.ExceptionHandling;
using UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs;
namespace UcbManagementInformation.Web.MIFileUpload.JobQueue
{
    
    public class JobQueue
    {
        private static volatile JobQueue instance;
        private static object syncRoot = new Object();

        // there must be a default job
        private JobQueue(int sleeptime)
        {
            this.sleepTime = sleeptime;
        }
        public static JobQueue Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new JobQueue(1000);
                        }
                    }
                }
                return instance;
            }
        }

        private Queue<JobBase> pendingJobQueue = new Queue<JobBase>();

        public Queue<JobBase> PendingJobQueue
        {
            get { return pendingJobQueue; }

        }
        public JobBase FindJobByCode(Guid code)
        {
            return AtomicAction("FindJob", code);
        }
        public JobBase CurrentJob { get; private set; }

        // stopper flag for our forever looping job
        private bool isOn;

        public bool IsOn
        {
            get { return isOn; }
            set { isOn = value; }
        }

        // if no jobs exist the queue sleeps for a time in milliseconds
        private int sleepTime;

        // this adds a new job to those waiting
        public void AddJob(JobBase job)
        {
            if (this.FindJobByCode(job.Code) == null)
            {
                pendingJobQueue.Enqueue(job);
            }
        }

        // stops the run loop
        public void Stop()
        {
            IsOn = false;
        }
        private JobBase AtomicAction(string Action, Guid code)
        {
            lock (syncRoot)
            {
                switch (Action)
                {
                    case "Dequeue":
                        if (pendingJobQueue.Count > 0)
                        {
                            CurrentJob = pendingJobQueue.Dequeue();
                        }
                        else
                        {
                            CurrentJob = null;
                        }
                        return CurrentJob;
                    case "FindJob":
                        JobBase Job = pendingJobQueue.FirstOrDefault(x => x.Code == code);
                        if (Job == null)
                        {
                            if (CurrentJob != null)
                            {
                                if (CurrentJob.Code == code)
                                {
                                    return CurrentJob;
                                }
                                else
                                {
                                    return null;
                                }
                            }
                            else
                            {
                                return null;
                            }
                        }
                        else
                        {
                            return Job;
                        }
                }
                return null;
            }
        }

        // the main work loop
        public void Run()
        {
           
                // initialize the flag that keeps the loop going
                IsOn = true;
                // start
                while (IsOn)
                {
                    try
                    {
                        // get the next pending job, otherwise the default one
                        AtomicAction("Dequeue", Guid.Empty);
                        // execute the job and retrieve the delay value
                        if (CurrentJob != null)
                        {
                            CurrentJob.Status = JobStatus.Running;
                            IUnitOfWork uow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("UcbManagementInformation");
                            
                            IUcbManagementInformationRepository<UploadJobQueue> uploadJobQueueRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UploadJobQueue>>(uow.ObjectContext);
                            IUcbManagementInformationRepository<UcbManagementInformation.Server.DataAccess.JobStep> jobStepRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UcbManagementInformation.Server.DataAccess.JobStep>>(uow.ObjectContext);
                            IUcbManagementInformationRepository<Alert> alertRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<Alert>>(uow.ObjectContext);
                            IUcbManagementInformationRepository<AlertUser> alertUserRepository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<AlertUser>>(uow.ObjectContext);
                            
                            var waitingJob = uploadJobQueueRepository.First(x => x.Code == CurrentJob.Code);
                            waitingJob.Status = "Running";
                            waitingJob.TimeStarted = DateTime.Now;
                            uploadJobQueueRepository.Update(waitingJob);
                            uow.Commit();

                            int rv = CurrentJob.RunJob();

                            using (uow)
                            {
                                //Log the job and job steps;
                                waitingJob = uploadJobQueueRepository.First(x => x.Code == CurrentJob.Code);
                                uploadJobQueueRepository.Update(waitingJob);

                                if (rv == 0)
                                {
                                    CurrentJob.Status = JobStatus.Succeeded;
                                    waitingJob.EndTime = DateTime.Now;
                                    waitingJob.Status = "Succeeded";

                                }
                                else
                                {
                                    CurrentJob.Status = JobStatus.Failed;
                                    waitingJob.EndTime = DateTime.Now;
                                    waitingJob.Status = "Failed";
                                }
                                uploadJobQueueRepository.Update(waitingJob);
                                foreach (JobStep currentStep in CurrentJob.JobSteps)
                                {
                                    UcbManagementInformation.Server.DataAccess.JobStep stepToSave = new UcbManagementInformation.Server.DataAccess.JobStep()
                                    {
                                        Code = currentStep.Code,
                                        Category = currentStep.Category,
                                        EndTime = currentStep.EndTime,
                                        StartTime = currentStep.StartTime,
                                        Status = currentStep.Status.ToString(),
                                        Name = currentStep.Name,
                                        UploadJobQueueCode = CurrentJob.Code,
                                        StepOrder=currentStep.Order
                                    };
                                    jobStepRepository.Add(stepToSave);

                                }
                                var alertToUpdate = alertRepository.FirstOrDefault(x => x.RelatedEntityCode == CurrentJob.Code);
                                if (alertToUpdate != null)
                                {
                                    alertToUpdate.Status = "Completed";
                                    alertToUpdate.CompletedDate = DateTime.Now;
                                    alertToUpdate.Message = "File upload job : " + waitingJob.JobDescription + " has finished. ";
                                    if (CurrentJob.Status == JobStatus.Succeeded)
                                    {
                                        alertToUpdate.Message += "The Job was successfull.";
                                    }
                                    else
                                    {
                                        alertToUpdate.Message += "The Job failed.";
                                        var failedStep = CurrentJob.JobSteps.FirstOrDefault(x => x.Status == JobStepStatus.Failed);
                                        if (failedStep != null)
                                        {
                                            alertToUpdate.Message += " The failing step was " + failedStep.Name;
                                        }
                                    }
                                }
                                uow.Commit();
                            }

                            if (CurrentJob.UnhandledException != null)
                            {
                                ExceptionPolicy.HandleException(CurrentJob.UnhandledException, ExceptionHandlingPolicies.UnhandledException);
                            }
                            //Job completed and saved to DB so remove from memory!!!!
                            CurrentJob = null;
                        }
                        // check to make sure a stop hasn't been issued
                        // and that the job calls for a wait
                        if (IsOn && sleepTime > 0)
                        {
                            // wait for the time specified
                            System.Threading.Thread.Sleep(sleepTime);
                        }
                    }
                    catch (Exception ex)
                    {
                        //publish ex;
                       
                       ExceptionPolicy.HandleException(ex, ExceptionHandlingPolicies.UnhandledException);
                       
                    }
                }
            
        }
    }

   
}

