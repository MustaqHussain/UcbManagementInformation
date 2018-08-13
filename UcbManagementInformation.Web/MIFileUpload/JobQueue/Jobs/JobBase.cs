using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Data.Objects.DataClasses;
using System.Runtime.Serialization;

namespace UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs
{
    public class JobBase : ComplexObject,IJob
    {
        
        public Guid Code
        {
            get;
            set;
        }
        public string UserId { get; set; }
        
        public JobStatus Status
        {
            get;
            set;
        }
        public JobStep CurrentStep { get; set; }

        public Exception UnhandledException { get; set; }

        public List<JobStep> JobSteps
        {
            get;
            set;
        }
        public JobStep this[string stepKey]
        {
            get
            {
                return (from x in JobSteps
                        where x.Name == stepKey
                        select x).FirstOrDefault();
            }
        }
        public JobStep this[int stepIndex]
        {
            get
            {
                return JobSteps[stepIndex];
            }
        }
        public DateTime? AddedTime { get; set; }
        public DateTime? StartTime { get; set; }
        public DateTime? EndTime { get; set; }
        public string Description { get; set; }
        public string Data { get; set; }

        public virtual int RunJob()
        { return 0; }
        
    }

    
}