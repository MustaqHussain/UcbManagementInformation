using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using UcbManagementInformation.Web.ExceptionHandling;
using UcbManagementInformation.Web.Services;
using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
using System.ServiceModel.DomainServices.Server;
using UcbManagementInformation.Web.Models;
using System.Threading;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Web.MIFileUpload.JobQueue;
using UcbManagementInformation.Web.MIFileUpload.JobQueue.Jobs;
using System.Configuration;

namespace UcbManagementInformation.Web
{
    public class Global : System.Web.HttpApplication
    {
        //UploadContainer container;
        private Timer jobQueueTimer;
        JobQueue jc;
        IUcbManagementInformationRepository<UploadJobQueue> uploadJobQueueRepository;
        IUcbManagementInformationRepository<MCSystemParameter> systemParameterRepository;
        protected void Application_Start(object sender, EventArgs e)
        {
             DomainService.Factory = new UcbManagementInformationDomainServiceFactory(); 
            // Initialise IoC container and store at app level for app lifetime
            BootStrapper.InitializeIoc();

            
        }

       
       
        protected void Application_BeginRequest(object sender, EventArgs e)
        {

        }

        protected void Application_AuthenticateRequest(object sender, EventArgs e)
        {

        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception LastException = Server.GetLastError().GetBaseException();

            var exceptionManager = EnterpriseLibraryContainer.Current.GetInstance<ExceptionManager>();
            exceptionManager.HandleException(LastException, ExceptionHandlingPolicies.UnhandledException);
        }

       

        protected void Application_End(object sender, EventArgs e)
        {
            //jc.Stop();
        }
    }
}