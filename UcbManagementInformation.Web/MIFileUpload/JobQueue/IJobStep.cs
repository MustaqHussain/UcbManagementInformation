using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Web.MIFileUpload.JobQueue
{
    public interface IJobStep
    {
        Guid Code { get; set; }
        string Name { get; set; }
        JobStepStatus Status { get; set; }
        string PercentComplete { get; set; }
        DateTime? StartTime { get; set; }
        DateTime? EndTime { get; set; }
    }
   
}
