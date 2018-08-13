using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.ServiceModel.DomainServices.Server;
using UcbManagementInformation.Server;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Web.Server;
using UcbManagementInformation.Web.Reporting;

namespace UcbManagementInformation.Web.Services
{
    public class UcbManagementInformationDomainServiceFactory : IDomainServiceFactory
    {
        public DomainService CreateDomainService(Type domainServiceType, DomainServiceContext context)
        {
            DomainService domainService;

            if (typeof(ReportWizardService) == domainServiceType)
            {
                #region ReportWizardService

                IUnitOfWork uow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("UcbManagementInformation");
                IUnitOfWork UcbUow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("Ucb");
                
                domainService = new ReportWizardService(SimpleServiceLocator.Instance.Get<IReportingServices>(),
                uow,UcbUow,
                DataAccessUtilities.RepositoryLocator<IDataTableRelationshipRepository>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<DataCategory>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IDataTableRepository>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<Report>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<MCUser>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IReportGroupRepository>(uow.ObjectContext),
                //second repository for read
                DataAccessUtilities.RepositoryLocator<IReportGroupRepository>(),
                DataAccessUtilities.RepositoryLocator<IDataItemRepository>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UserReportGroup>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IDataModelRepository>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IDataTableJoinRepository>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IDataTableRelationshipJoinRepository>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<Filter>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ReportChart>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ReportChartSery>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ReportDataTableJoin>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ReportItem>>(uow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbRepository<ReportCategory>>(UcbUow.ObjectContext),
                DataAccessUtilities.RepositoryLocator<IUcbRepository<StandardReport>>(UcbUow.ObjectContext));
                
                #endregion
            }
      
            else if (typeof(AlertService) == domainServiceType)
            {
                 IUnitOfWork uow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("UcbManagementInformation");

                 domainService = new AlertService(DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<Alert>>(uow.ObjectContext),
                 DataAccessUtilities.RepositoryLocator < IUcbManagementInformationRepository < AlertUser> > (uow.ObjectContext), uow);
               
            }
            else if (typeof(MIUploadService) == domainServiceType)
            {
                IUnitOfWork uow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("UcbManagementInformation");

                domainService = new MIUploadService(uow,
                    DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<ProviderOrganisation>>(uow.ObjectContext),
                    DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<InputFileHistory>>(uow.ObjectContext),
                    DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<Alert>>(uow.ObjectContext),
                    DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<AlertUser>>(uow.ObjectContext),
                    DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<MCUser>>(uow.ObjectContext),
                    DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<UploadJobQueue>>(uow.ObjectContext)
                    );

            }
            //else if (typeof(UcbPublishService) == domainServiceType)
            //{
            //    IUnitOfWork UcbUow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("Ucb");
            //    domainService = new UcbPublishService(SimpleServiceLocator.Instance.Get<IReportingServices>(), UcbUow,
            //    DataAccessUtilities.RepositoryLocator<IUcbRepository<ReportCategory>>(UcbUow.ObjectContext),
            //    DataAccessUtilities.RepositoryLocator<IUcbRepository<StandardReport>>(UcbUow.ObjectContext)
            //    );
              
            //}
            else
            {
                domainService = (DomainService)Activator.CreateInstance(domainServiceType);
            }

            domainService.Initialize(context);
            return domainService;
        }

        public void ReleaseDomainService(DomainService domainService)
        {
            domainService.Dispose();
        }
    }
}