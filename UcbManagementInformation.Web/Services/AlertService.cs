
namespace UcbManagementInformation.Web.Services
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.EntityFramework;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using UcbManagementInformation.Server.DataAccess;


    // Implements application logic using the ESF2007Entities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    public class AlertService : DomainService
    {

        IUcbManagementInformationRepository<Alert> alertRepository;
        IUcbManagementInformationRepository<AlertUser> alertUserRepository;
        IUnitOfWork uow;
        public AlertService(IUcbManagementInformationRepository<Alert> alertRepositoryItem, IUcbManagementInformationRepository<AlertUser> alertUserRepositoryItem, IUnitOfWork uowItem)
        {
            alertRepository = alertRepositoryItem;
            alertUserRepository = alertUserRepositoryItem;
            uow = uowItem;
        }
        public IQueryable<Alert> GetMyAlerts()
        {
            DateTime OneWeekAgo = DateTime.Now.AddDays(-7);
            return alertRepository.Find(x=>x.AlertUsers.Select(y=>y.User.Name == this.ServiceContext.User.Identity.Name).Count()>=1 && x.Status=="Completed" && x.CompletedDate > OneWeekAgo ,"AlertUsers","AlertUsers.User").OrderBy(x=>x.CompletedDate).AsQueryable();
        }

        public IQueryable<Alert> GetAlerts()
        {
            return alertRepository.GetAll().AsQueryable();
        }

        public void InsertAlert(Alert alertItem)
        {
            alertRepository.Add(alertItem);
        }

        public void UpdateAlert(Alert alertItem)
        {
            alertRepository.Update(alertItem);
        }

        public void DeleteAlert(Alert alertItem)
        {
            alertRepository.Delete(alertItem);
        }
        public IQueryable<AlertUser> GetAlertUsers()
        {
            return alertUserRepository.GetAll().AsQueryable();
        }

        public void InsertAlertUser(AlertUser alertUserItem)
        {
            alertUserRepository.Add(alertUserItem);
        }

        public void UpdateAlertUser(AlertUser alertUserItem)
        {
            alertUserRepository.Update(alertUserItem);
        }

        public void DeleteAlertUser(AlertUser alertUserItem)
        {
            alertUserRepository.Delete(alertUserItem);
        }
        protected override bool PersistChangeSet()
        {
            uow.Commit();
            return base.PersistChangeSet();
             
        }
    }
}


