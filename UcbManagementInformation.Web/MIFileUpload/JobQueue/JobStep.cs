using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.Threading;


namespace UcbManagementInformation.Web.MIFileUpload.JobQueue
{
    public class JobStep : ComplexObject
    {
        public object syncRoot = new object();
        public JobStep()
        { }
        public JobStep(string name,string category,IJob parentJob,int order)
        {
            ParentJob = parentJob;
            Code = Guid.NewGuid();
            Name = name;
            Order = order;
            Category = category;
            tcb = new TimerCallback(JobUpdater);
            
        }
        public JobStep(DateTime? startTimeItem, DateTime? endTimeItem, JobStepStatus statusItem)
        {
            StartTime = startTimeItem;
            EndTime = endTimeItem;
            Status = statusItem;
            if (Status == JobStepStatus.NotStarted)
            {
                PercentComplete = 0;
            }
            else
            {
                PercentComplete = 100;
            }
            tcb = new TimerCallback(JobUpdater);
            
        }
        private Timer UpdaterTimer;
        private TimerCallback tcb;
        private void JobUpdater(object state)
        {
            runningTime++;
            double percent = Convert.ToInt32((Convert.ToDouble(runningTime) / Convert.ToDouble(Timeout)) * 80);
            UpdateProgress(percent);
        }
        public Guid Code { get; set; }
        public int Order { get; set; }
        public string Name
        {
            get;
            set;
        }
        public IJob ParentJob { get; set; }
        public string Category
        {
            get;
            set;
        }
        private string progressText;
        public string ProgressText
        {
            get
            {
                lock (syncRoot)
                {
                    return progressText;
                }
            }
            private set { progressText = value; }
        }
        private JobStepStatus status;
        public JobStepStatus Status
        {
            get
            {
                lock (syncRoot)
                {
                    return status;
                }
            }

            private set { status = value; }
        }
        private double percentComplete;
        public double PercentComplete
        {
            get 
            { 
                lock (syncRoot) 
                {
                    return percentComplete;
                }
            }
            private set { percentComplete = value; }
        }
        DateTime? startTime;
        public DateTime? StartTime
        {
            get
            {
                lock (syncRoot)
                {
                    return startTime;
                }
            }
            private set { startTime = value; }
        }
        DateTime? endTime;
       
        public DateTime? EndTime
        {
            get
            {
                lock (syncRoot)
                {
                    return endTime;
                }
            }
            private set { endTime = value; }
        }
        private int timeOut = 0;
        public int Timeout { get { return timeOut; } set { timeOut = value; } }
        int runningTime;
        private int RunningTime
        {
            get {return runningTime;}
            set {runningTime=value;}
        }
        public void Start()
        {
            lock (syncRoot)
            {
                StartTime = DateTime.Now;
                Status = JobStepStatus.Running;
                PercentComplete = 0;
                if (Timeout != 0)
                {
                    UpdaterTimer = new Timer(tcb, null, 0, 1000);
                }
            }
        }
        public void UpdateProgress(double percentComplete)
        {
            UpdateProgress(percentComplete, null);
        }
        public void UpdateProgress(double percentComplete,string progressText)
        {
            lock (syncRoot)
            {
                PercentComplete = percentComplete;
                ProgressText = progressText;
            }
        }

        public void Finish(bool success)
        {
            lock (syncRoot)
            {
                if (Timeout != 0)
                {
                    UpdaterTimer.Dispose();
                }
                EndTime = DateTime.Now;
                PercentComplete = 100;
                if (success)
                {
                    Status = JobStepStatus.Succeeded;
                }
                else
                {
                    Status = JobStepStatus.Failed;
                }
            }
        }

        public int Run()
        {
            return 0;
        }
    }
    public enum JobStepStatus
    {
        NotStarted=0,
        Running=1,
        Succeeded=2,
        Failed=3
    }
}