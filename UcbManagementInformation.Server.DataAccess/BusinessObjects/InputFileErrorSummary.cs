using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    public class InputFileErrorSummary
    {
        [Key]
        public Guid Code { get; set; }
        public int FileLevelInfo { get; set; }
        public int FileLevelWarning { get; set; }
        public int FileLevelError { get; set; }
        public int RecordLevelInfo { get; set; }
        public int RecordLevelWarning { get; set; }
        public int RecordLevelError { get; set; }

    }
}
