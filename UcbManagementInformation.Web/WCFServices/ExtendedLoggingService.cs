using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Practices.EnterpriseLibrary.Logging;
using Microsoft.Practices.EnterpriseLibrary.Logging.Service;

namespace UcbManagementInformation.Web.WCFServices
{
    [ServiceContract]
    public class ExtendedLoggingService : LoggingService
    {
        //protected override void CollectInformation(LogEntry entry)
        //{
        //    // Get the user IP address and add them as an extended property. 
        //    // NOTE. This information can easily be spoofed by the caller. Use this information only
        //    // for diagnostic puposes, not for authentication or auditing purposes. 
        //    var messageProperties = OperationContext.Current.IncomingMessageProperties;
        //    var endpoint = messageProperties[RemoteEndpointMessageProperty.Name] as RemoteEndpointMessageProperty;
        //    string clientAddress = endpoint != null ? endpoint.Address : string.Empty;

        //    entry.ExtendedProperties.Add("ClientIPAddress", clientAddress);

        //    // Also add the client host address as a category. This allows you to turn on server side logging for a single client
        //    if (!string.IsNullOrEmpty(clientAddress))
        //    {
        //        entry.Categories.Add(clientAddress);
        //    }

        //    base.CollectInformation(entry);
        //}
    }
}