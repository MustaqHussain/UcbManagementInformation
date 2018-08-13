
namespace UcbManagementInformation.Web.Services
{/*
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;

    using Microsoft.Practices.EnterpriseLibrary.Logging;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Service;

    [EnableClientAccess()]
    public class LoggingDomainService : DomainService
    {
        private MconLoggingService loggingService;


        // Summary:
        //     Initializes a new instance of the Microsoft.Practices.EnterpriseLibrary.Logging.Service.LoggingService
        //     class.
        public LoggingDomainService()
        {
            loggingService = new MconLoggingService();
        }


        //
        // Summary:
        //     Initializes a new instance of the Microsoft.Practices.EnterpriseLibrary.Logging.Service.LoggingService
        //     class.
        //
        // Parameters:
        //   logWriter:
        //     The log sink where to store incoming entries.
        public LoggingDomainService(LogWriter logWriter)
        {
            loggingService = new MconLoggingService(logWriter);
        }


        // Summary:
        //     Adds log entries into to the server log.
        //
        // Parameters:
        //   entries:
        //     The client log entries to log in the server.


        public void MAdd(LogEntryMessage[] entries)
        {
            loggingService.Add(entries);
        }


        //
        // Summary:
        //     Used to collect more information or customize the incoming log entry before
        //     logging it.
        //
        // Parameters:
        //   entry:
        //     The log entry coming from the client.
        protected virtual void CollectInformation(LogEntry entry)
        {
            loggingService.CollectInformation(entry);
        }


        //
        // Summary:
        //     Translates the incoming Microsoft.Practices.EnterpriseLibrary.Logging.Service.LogEntryMessage
        //     into a Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry.
        //
        // Parameters:
        //   entry:
        //     The log entry coming from the client.
        //
        // Returns:
        //     A Microsoft.Practices.EnterpriseLibrary.Logging.LogEntry instance that can
        //     be stored in the log.
        protected virtual LogEntry Translate(LogEntryMessage entry)
        {
            return loggingService.Translate(entry);
        }

        private class MconLoggingService : LoggingService
        {
            public MconLoggingService() : base() { }

            public MconLoggingService(LogWriter logWriter) : base(logWriter) { }

            public void Add(LogEntryMessage[] entries)
            {
                base.Add(entries);
            }
 
            public new LogEntry Translate(LogEntryMessage entry)
            {
                return base.Translate(entry);
            }

            public new void CollectInformation(LogEntry entry)
            {
                base.CollectInformation(entry);
            }
        }
    }*/
}


