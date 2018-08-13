using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace UcbManagementInformation.Web.MIFileUpload.JobQueue
{
    public interface IJob
    {
        int RunJob();
        Guid Code { get; set; }
        JobStatus Status
        {
            get;
            set;
        }
        string UserId { get; set; }
        JobStep CurrentStep { get; set; }

        Exception UnhandledException { get; set; }

        List<JobStep> JobSteps
        {
            get;
            set;
        }

    }
    public enum JobStatus
    {
        Waiting=0,
        Running=1,
        Succeeded=2,
        Failed=3
    }
}
