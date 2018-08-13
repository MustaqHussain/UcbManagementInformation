using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.RDL2003Engine
{
    interface IDataModelAware
    {
        DataModel CurrentDataModel { get; set; }
    }
}
