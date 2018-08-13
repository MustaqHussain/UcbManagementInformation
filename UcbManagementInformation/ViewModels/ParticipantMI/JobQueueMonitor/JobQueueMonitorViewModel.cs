using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.MVVM;
using System.Collections.ObjectModel;
using UcbManagementInformation.Web.MIFileUpload.JobQueue;
using UcbManagementInformation.Web.Services;
using UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs;
using System.ServiceModel.DomainServices.Client;
using System.Windows.Threading;
using System.Linq;
using System.Collections.Generic;
using UcbManagementInformation.Models;

namespace UcbManagementInformation.ViewModels
{
    public class JobQueueMonitorViewModel : ViewModel, IJobQueueMonitorViewModel
    {
        MIUploadContext myContext;
        DispatcherTimer timer;
        public JobQueueMonitorViewModel()
            : this(new MIUploadContext())
        { }

        public JobQueueMonitorViewModel(MIUploadContext context)
        {
            myContext = context;
            JobQueue = new ObservableCollection<JobBase>();
            CompletedJobs = new ObservableCollection<JobBase>();

            monitoringJobCode = App.Session[Helpers.SessionKey.JobCode] != null ? (Guid)App.Session[Helpers.SessionKey.JobCode]:Guid.Empty;
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(JobQueueMonitorViewModel_PropertyChanged);
        }

        void JobQueueMonitorViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedActiveJob":
                    if (selectedActiveJob != null && selectedActiveJob.Code != Guid.Empty)
                    {
                        monitoringJobCode = selectedActiveJob.Code;
                    }
                    break;
                case "SelectedCompletedJob":
                    if (selectedCompletedJob != null && selectedCompletedJob.Code != Guid.Empty)
                    {
                        monitoringJobCode = selectedCompletedJob.Code;
                    }
                    break;
            }
        }

        public override void Initialize()
        {
           timer = new DispatcherTimer();
            timer.Interval = TimeSpan.FromSeconds(1);
            timer.Tick += new EventHandler(timer_Tick);
            timer.Start();
           
       }

        void timer_Tick(object sender, EventArgs e)
        {
            myContext.GetActiveFileUploadJobs(ActiveJobsCallBack, null);
            myContext.GetCompleteFileUploadJobs(CompletedJobsCallback, null);
        }
        private void UpdateExistingStep(JobStep existing,JobStep newValues)
        {
            existing.Category = newValues.Category;
            existing.EndTime = newValues.EndTime;
            existing.Name = newValues.Name;
            existing.PercentComplete = newValues.PercentComplete;
            existing.ProgressText = newValues.ProgressText;
            existing.StartTime = newValues.StartTime;
            existing.Status = newValues.Status;
        }

        private JobStep CloneStep(JobStep newValues)
        {
            JobStep existing = new JobStep();
            existing.Code = newValues.Code;
            existing.Category = newValues.Category;
            existing.EndTime = newValues.EndTime;
            existing.Name = newValues.Name;
            existing.PercentComplete = newValues.PercentComplete;
            existing.ProgressText = newValues.ProgressText;
            existing.StartTime = newValues.StartTime;
            existing.Status = newValues.Status;
            return existing;
        }
        JobBase CloneJob(JobBase job)
        {
            JobBase Clone = new JobBase();
            Clone.Code = job.Code;
            Clone.CurrentStep = (job.CurrentStep == null)?null:CloneStep(job.CurrentStep);
            Clone.JobSteps = new List<JobStep>();
            foreach (JobStep currentStep in job.JobSteps)
            {
                Clone.JobSteps.Add(CloneStep(currentStep));            
            }
            Clone.Status = job.Status;
            Clone.UserId = job.UserId;
            Clone.AddedTime = job.AddedTime;
            Clone.Description = job.Description;
            Clone.EndTime = job.EndTime;
            Clone.StartTime = job.StartTime;
            return Clone;
        }

        void UpdateExistingJob(JobBase existing,JobBase newJob)
        {
            if (newJob.CurrentStep != null)
            {
                if (existing.CurrentStep == null)
                {
                    existing.CurrentStep = CloneStep(newJob.CurrentStep);
                }
                else
                {
                    if (existing.CurrentStep.Code == newJob.CurrentStep.Code)
                    {
                        UpdateExistingStep(existing.CurrentStep, newJob.CurrentStep);
                    }
                    else
                    {
                        existing.CurrentStep = CloneStep(newJob.CurrentStep);
                    }
                }
            }
            
            foreach (JobStep currentStep in newJob.JobSteps)
            {
                var existingStep = existing.JobSteps.FirstOrDefault(x => x.Code == currentStep.Code);
                if (existingStep != null)
                {
                    UpdateExistingStep(existingStep, currentStep);
                }
                else
                {
                    existing.JobSteps.Add(CloneStep(currentStep));
                }
            }
            existing.JobSteps = newJob.JobSteps;
            existing.Status = newJob.Status;
            existing.UserId = newJob.UserId;
            existing.AddedTime = newJob.AddedTime;
            existing.Description = newJob.Description;
            existing.EndTime = newJob.EndTime;
            existing.StartTime = newJob.StartTime;
            
        }
        private void ActiveJobsCallBack(InvokeOperation<IEnumerable<JobBase>> io)
        {
            List<JobBase> activeJobArray = new List<JobBase>();
            if (io.Value != null)
            {
                activeJobArray = new List<JobBase>(io.Value);
            }
            foreach (JobBase jobItem in activeJobArray)
            {
                if (jobItem.Code == monitoringJobCode)
                {
                    UpdateCurrentJob(jobItem);
                }
                else
                {
 
                }
                var jobFound = JobQueue.FirstOrDefault(x => x.Code == jobItem.Code);
                if (jobFound != null)
                {

                    //update job info
                    UpdateExistingJob(jobFound, jobItem);
                }
                else
                {
                    //insert job info
                    JobQueue.Add(CloneJob(jobItem));
                }
            }
            //Remove jobs from queue that don't exist in Array
            var c = from aItem in JobQueue
                    from bItem in activeJobArray.Where(bItem => aItem.Code == bItem.Code).DefaultIfEmpty()
                    where bItem == null
                    select aItem;
            JobQueue.RemoveAll(x=> c.Contains(x));
           
            
            
        }
        
        private void UpdateCurrentJob(JobBase jobItem)
        {
            if (CurrentJob == null)
            {
                CurrentJob = CloneJob(jobItem);
                CurrentJobSteps = new ObservableCollection<JobStep>(CurrentJob.JobSteps);
            }
            else
            {

                if (CurrentJob.Code == jobItem.Code)
                {
                    UpdateExistingJob(CurrentJob, jobItem);
                    foreach (JobStep stepItem in CurrentJobSteps)
                    {
                        var newStep = jobItem.JobSteps.First(x=>x.Code == stepItem.Code);
                        UpdateExistingStep(stepItem, newStep);
                    }
                }
                else
                {
                    CurrentJob = CloneJob(jobItem);
                    CurrentJobSteps = new ObservableCollection<JobStep>(CurrentJob.JobSteps);
            
                }
            }

        }
        
        private void CompletedJobsCallback(InvokeOperation<IEnumerable<JobBase>> io)
        {
            List<JobBase> completedJobArray = new List<JobBase>();
            if (io.Value != null)
            {
                completedJobArray = new List<JobBase>(io.Value);
            }
            foreach (JobBase jobItem in completedJobArray)
            {
                if (jobItem.Code == monitoringJobCode)
                {
                    UpdateCurrentJob(jobItem);
                }
                else
                { 
                }
                var jobFound = CompletedJobs.FirstOrDefault(x => x.Code == jobItem.Code);
                if (jobFound != null)
                {
                    //update job info
                    UpdateExistingJob(jobFound, jobItem);
                }
                else
                {
                    //insert job info
                    CompletedJobs.Add(CloneJob(jobItem));
                }
            }
            
                //Remove jobs from queue that don't exist in Array
                var c = from aItem in CompletedJobs
                        from bItem in completedJobArray.Where(bItem=> aItem.Code == bItem.Code).DefaultIfEmpty() 
                        where bItem == null
                        select aItem;

                 CompletedJobs.RemoveAll(x=> c.Contains(x));
           
        }

        private Guid monitoringJobCode = Guid.Empty;

        private ObservableCollection<JobStep> currentJobSteps;

        public ObservableCollection<JobStep> CurrentJobSteps
        {
            get { return currentJobSteps; }
            set { currentJobSteps = value; OnPropertyChanged("CurrentJobSteps"); }
        }
        private ObservableCollection<JobBase> jobQueue;

        public ObservableCollection<JobBase> JobQueue
        {
            get { return jobQueue; }
            set { jobQueue = value; OnPropertyChanged("JobQueue"); }
        }
        private ObservableCollection<JobBase> completedJobs;

        public ObservableCollection<JobBase> CompletedJobs
        {
            get { return completedJobs; }
            set { completedJobs = value; OnPropertyChanged("CompletedJobs"); }
        }
        private JobBase currentJob;

        public JobBase CurrentJob
        {
            get { return currentJob; }
            set { currentJob = value; OnPropertyChanged("CurrentJob"); }
        }
        private JobBase selectedCompletedJob;

        public JobBase SelectedCompletedJob
        {
            get { return selectedCompletedJob; }
            set { selectedCompletedJob = value; OnPropertyChanged("SelectedCompletedJob"); }
        }
        private JobBase selectedActiveJob;

        public JobBase SelectedActiveJob
        {
            get { return selectedActiveJob; }
            set { selectedActiveJob = value; OnPropertyChanged("SelectedActiveJob"); }
        }
    }
}
