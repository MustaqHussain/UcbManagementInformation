using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.ServiceLocation;
using UcbManagementInformation.ViewModels;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Controls;
using UcbManagementInformation.Views.Reporting;
using UcbManagementInformation.Helpers;


namespace UcbManagementInformation
{
    public class Bootstrapper
    {
        public static void InitializeIoc()
        {
            SimpleServiceLocator.SetServiceLocatorProvider(new UnityServiceLocator());
            SimpleServiceLocator.Instance.Register<IReportWizardViewModel, ReportWizardViewModel>();
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IReportListViewModel,ReportListViewModel>(new object[] { });
            SimpleServiceLocator.Instance.Register<IParticipantCsvToXmlConverterViewModel, ParticipantCsvToXmlConverterViewModel>();
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IUploadedFileMonitorViewModel, UploadedParticipantViewModel>(new object[] { });
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IUploadFileViewModel, UploadFileViewModel>(new object[] { });
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<ITransferPostCodeFilesViewModel, TransferPostCodeFilesViewModel>(new object[] { });
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IJobQueueMonitorViewModel, JobQueueMonitorViewModel>(new object[] { });
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IUploadDecisionViewModel, UploadDecisionViewModel>(new object[] { });
            SimpleServiceLocator.Instance.RegisterWithConstructorParameters<IUploadHistoryViewModel, UploadHistoryViewModel>(new object[] { });
            SimpleServiceLocator.Instance.Register<ILogoTimeSpinnerViewModel, LogoTimeSpinnerViewModel>();
            SimpleServiceLocator.Instance.Register<ISelectReportGroupViewModel, SelectReportGroupViewModel>();
            SimpleServiceLocator.Instance.Register<ISelectReportModelDialogViewModel, SelectReportModelDialogViewModel>();
            SimpleServiceLocator.Instance.Register<IAdvancedJoinViewModel, AdvancedJoinViewModel>();
            SimpleServiceLocator.Instance.Register<IESFTickerViewModel, FakeESFTickerViewModel>();
            SimpleServiceLocator.Instance.Register<IEditDataItemViewModel, EditDataItemViewModel>();
            SimpleServiceLocator.Instance.Register<IPublishToUcbViewModel, PublishToUcbViewModel>();
            SimpleServiceLocator.Instance.Register<IReportGroupPermissionsViewModel, ReportGroupPermissionsViewModel>();
            SimpleServiceLocator.Instance.Register<IMainPageViewModel, MainPageViewModel>();
            //SimpleServiceLocator.Instance.Register<ITimelineViewModel, TimelineViewModel>();
            
            SimpleServiceLocator.Instance.Register<IModalDialogService, ModalDialogService>();
            SimpleServiceLocator.Instance.Register<INavigationService, PageNavigationService>();
            SimpleServiceLocator.Instance.Register<IMessageBoxService, MessageBoxService>();
            SimpleServiceLocator.Instance.Register<IModalWindow, SelectReportGroupDialog>("SelectReportGroupDialog");
            SimpleServiceLocator.Instance.Register<IModalWindow, AdvancedJoinDialog>("AdvancedJoinDialog");
            SimpleServiceLocator.Instance.Register<IModalWindow, EditDataItemDialog>("EditDataItemDialog");
            SimpleServiceLocator.Instance.Register<IModalWindow, PublishToUcbDialog>("PublishToUcbDialog");
            SimpleServiceLocator.Instance.Register<IModalWindow, ReportGroupPermissions>("ReportGroupPermissions");
                                                                                        
            SimpleServiceLocator.Instance.Register<IModalWindow, SelectReportModelDialog>("SelectReportModelDialog");
            
        }  
    }
    
}
