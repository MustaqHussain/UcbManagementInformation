using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel;
using System.ServiceModel.Activation;

namespace UcbManagementInformation.Web.WCFServices
{
    [ServiceBehavior(AddressFilterMode=AddressFilterMode.Any)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Allowed)]
    public class LoggingService : Microsoft.Practices.EnterpriseLibrary.Logging.Service.LoggingService
    {
    }
}