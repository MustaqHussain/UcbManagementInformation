using System;
using System.Collections.Generic;
using System.Data.Objects;
using System.Linq;
using System.Web;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using UcbManagementInformation.Server.RDL2003Engine;
using UcbManagementInformation.Web.Reporting;
namespace UcbManagementInformation.Web
{
    public class BootStrapper
    {
        public static void InitializeIoc()
        {
            // Create and store instance of ServiceLocator
            // Call this method at App level e.g. Global.asax to ensure object referenced for app lifetime.
            SimpleServiceLocator.SetServiceLocatorProvider(new UnityServiceLocator());

            // UnitOfWork
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IUnitOfWork, UnitOfWork>("UcbManagementInformation",new object[]{"UcbManagementInformation"});
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IUnitOfWork, UnitOfWork>("Ucb", new object[] { "Ucb" });
            
            // Register Context
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IObjectContext, UcbManagementInformationEntities>("UcbManagementInformation",new object[]{});
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IObjectContext, AdepUcbEntities>("Ucb", new object[] { });
           
            //Register Generic Repositories
            SimpleServiceLocator.Instance.Register(typeof(IUcbManagementInformationRepository<>), typeof(UcbManagementInformationRepository<>));
            SimpleServiceLocator.Instance.Register(typeof(IUcbRepository<>), typeof(UcbRepository<>));
            
            //Reporting Services 2010
            SimpleServiceLocator.Instance.Register(typeof(IReportingServices), typeof(ReportingServices));
            
            //Register Specialised UcbManagementInformation Repositories
            SimpleServiceLocator.Instance.Register<IDataItemRepository, DataItemRepository>();
            SimpleServiceLocator.Instance.Register<IDataTableJoinRepository, DataTableJoinRepository>();
            SimpleServiceLocator.Instance.Register<IDataTableRelationshipJoinRepository, DataTableRelationshipJoinRepository>();
            SimpleServiceLocator.Instance.Register<IDataTableRelationshipRepository, DataTableRelationshipRepository>();
            SimpleServiceLocator.Instance.Register<IDataTableRepository, DataTableRepository>();
            SimpleServiceLocator.Instance.Register<IDataModelRepository, DataModelRepository>();
            SimpleServiceLocator.Instance.Register<IMCUserRepository, MCUserRepository>();
           
            SimpleServiceLocator.Instance.Register<IReportGroupRepository, ReportGroupRepository>();
            //Register Specialised MI Repositories
            

            //ReportEngine
            //SimpleServiceLocator.Instance.Register<IReportingServicesStyle, ReportingServicesStyle>();// ("ReportingServicesStyle");
            //SimpleServiceLocator.Instance.Register<IReportingServicesStyle, ReportingServicesStyle>("ReportingServicesStyleRight");
            //SimpleServiceLocator.Instance.Register<ITableHeaderTextBox, TableHeaderTextBox>();
           // SimpleServiceLocator.Instance.Register<ITableHeaderTextBox, TableHeaderTextBox>();
            
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IReportingServicesStyle, ReportingServicesStyle>
                ("ReportingServicesStyle", new object[] { ReportingServicesStyle.TextBoxStyle.TableHeaderTextBox });
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IReportingServicesStyle, ReportingServicesStyle>
                ("ReportingServicesStyleRight", new object[] { ReportingServicesStyle.TextBoxStyle.TableHeaderTextBox, "Right" });
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<ITableHeaderTextBox, TableHeaderTextBox>
                ("TableHeaderTextBox", "","" );
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<ITableHeaderTextBox, TableHeaderTextBox>
                ("TableHeaderTextBoxRight", "","",true );

        }
    }
}