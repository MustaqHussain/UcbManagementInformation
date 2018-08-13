using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data;

namespace UcbManagementInformation.Web.MIFileUpload
{
    public interface IEnhancedDataReader : IDataReader
    {
        long CurrentPosition { get; set; }
        long FileLength { get; set; }

    }
}
