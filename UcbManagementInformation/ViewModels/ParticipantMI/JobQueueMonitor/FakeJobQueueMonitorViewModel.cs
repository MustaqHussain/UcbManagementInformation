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
using System.Collections.Generic;
using UcbManagementInformation.Web.MIFileUpload.JobQueue;
using UcbManagementInformation.MVVM;
using System.Collections.ObjectModel;
using UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs;

namespace UcbManagementInformation.ViewModels
{
    public class FakeJobQueueMonitorViewModel : ViewModel, IJobQueueMonitorViewModel
    {
        public FakeJobQueueMonitorViewModel()
        {
            CurrentJob = new JobBase()
            {
                Code = Guid.NewGuid(),
                //ProviderKey = "TESTYH",
                Status = JobStatus.Running,
                //UniqueFileName = @"C:\test",
                UserId = "dhays",
                JobSteps = new List<JobStep>() 
                {
                    new JobStep() {Code=Guid.NewGuid(),Category="Job",Name="JobStep1",PercentComplete=50,ProgressText="Progress",StartTime=DateTime.Now,Status=JobStepStatus.Running},
                    new JobStep() {Code=Guid.NewGuid(),Category="Job",Name="JobStep2",PercentComplete=0,ProgressText="Progress",Status=JobStepStatus.NotStarted},
                    new JobStep() {Code=Guid.NewGuid(),Category="Job",Name="JobStep3",PercentComplete=0,ProgressText="Progress",Status=JobStepStatus.NotStarted},
                    new JobStep() {Code=Guid.NewGuid(),Category="Job",Name="JobStep4",PercentComplete=0,ProgressText="Progress",Status=JobStepStatus.NotStarted},
                }
            };
            CurrentJobSteps = new ObservableCollection<JobStep>(CurrentJob.JobSteps);
            JobQueue = new ObservableCollection<JobBase>();
            JobQueue.Add(CurrentJob);
            CompletedJobs = new ObservableCollection<JobBase>();
            CompletedJobs.Add(CurrentJob);
        }
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
    }
}
