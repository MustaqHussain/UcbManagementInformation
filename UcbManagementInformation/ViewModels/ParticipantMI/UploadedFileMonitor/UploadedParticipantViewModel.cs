using System;
using System.Net;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.MVVM;
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Helpers;
using UcbManagementInformation.Web.Services;

namespace UcbManagementInformation.ViewModels
{
    public class UploadedParticipantViewModel : ViewModel, IUploadedFileMonitorViewModel, INavigable
    {
        private MIUploadContext myContext;// = new ReportWizardContext();
        
        public UploadedParticipantViewModel()
            : this(new MIUploadContext())
        {
            
        }

        public UploadedParticipantViewModel(MIUploadContext context)
        {
            myContext = context;

            UploadFileCommand = new RelayCommand(cmd => UploadFile(cmd), exe => CanUploadFile(exe));
            ViewHistoryCommand = new RelayCommand(cmd => ViewHistory(cmd), exe => CanViewHistory(exe));
            LockedCommand = new RelayCommand(cmd => Unlock(cmd), exe => CanUnlock(exe));
            DecisionCommand = new RelayCommand(cmd => Decide(cmd), exe => CanDecide(exe));

        }

        public override void  Initialize()
        {
 	        //Retreive and populate the provider organisations
            myContext.Load<ProviderOrganisation>(myContext.GetProviderOrganisationsQuery(),
                (lo) =>
                {
                    if (!lo.HasError)
                    {
                        ParticipantProvidersList = new ObservableCollection<ProviderOrganisation>(lo.Entities);
                    }
                }
                , null);
        }

        private INavigationService navigationService;

        public INavigationService NavigationService
        {
            get { return navigationService; }
            set { navigationService = value; OnPropertyChanged("NavigationService"); }
        }
        private ObservableCollection<ProviderOrganisation> participantProvidersList;

        public ObservableCollection<ProviderOrganisation> ParticipantProvidersList
        {
            get { return participantProvidersList; }
            set 
            {
                participantProvidersList = value;
                
                DateTime? newestFile = participantProvidersList.Max(
                    x=>(x.InputFileHistory!=null)?(x.InputFileHistory.ValidatedLoadDate.HasValue?x.InputFileHistory.ValidatedLoadDate.Value:DateTime.MinValue):DateTime.MinValue);
                if (newestFile.HasValue)
                {
                    foreach (ProviderOrganisation currentPO in participantProvidersList)
                    {
                        if (currentPO.InputFileHistory != null)
                        {
                            if (currentPO.InputFileHistory.ValidatedLoadDate.HasValue)
                            {
                                currentPO.InputFileHistory.DaysAfterNewestFile = newestFile.Value.Subtract(currentPO.InputFileHistory.ValidatedLoadDate.Value).Days;
                            }
                            else
                            {
                                currentPO.InputFileHistory.DaysAfterNewestFile = -1;
                            }
                        }
                    }
                }
                OnPropertyChanged("ParticipantProvidersList"); 
            }
        }
        private ProviderOrganisation selectedProvider;

        public ProviderOrganisation SelectedProvider
        {
            get { return selectedProvider; }
            set { selectedProvider = value; OnPropertyChanged("SelectedProvider"); }
        }

        public RelayCommand UploadFileCommand { get; set; }
        public RelayCommand ViewHistoryCommand { get; set; }
        public RelayCommand LockedCommand { get; set; }
        public RelayCommand DecisionCommand { get; set; }

        private bool CanUploadFile(object item)
        {
            ProviderOrganisation providerForUpload = item as ProviderOrganisation;
            return providerForUpload != null && providerForUpload.Status !="Locked";
        }

        private void UploadFile(object item)
        {
            ProviderOrganisation providerForUpload = item as ProviderOrganisation;
            if (providerForUpload != null)
            {
                App.Session[SessionKey.UploadFileType] = providerForUpload.FileType;
                App.Session[SessionKey.UploadProviderKey] = providerForUpload.KeyValue;
                NavigationService.Navigate("/ParticipantMI/UploadFile");
            }
        }
        private bool CanViewHistory(object item)
        {
            ProviderOrganisation providerForView = item as ProviderOrganisation;
            return providerForView != null;
        }

        private void ViewHistory(object item)
        {
            ProviderOrganisation providerForView = item as ProviderOrganisation;
            if (providerForView != null)
            {
                App.Session[SessionKey.UploadFileType] = providerForView.FileType;
                App.Session[SessionKey.UploadProviderKey] = providerForView.KeyValue;
                NavigationService.Navigate("/ParticipantMI/UploadHistory");
            }
        }
        private bool CanUnlock(object item)
        {
            ProviderOrganisation providerForUnlock = item as ProviderOrganisation;
            if (providerForUnlock != null)
            {
                return providerForUnlock.Status == "Locked";
            }

            else
            {
                return false;
            }
        }
        private void Unlock(object item)
        {
            //reset lock flag.
        }
        private bool CanDecide(object item)
        {
            ProviderOrganisation providerForDecision = item as ProviderOrganisation;
            if (providerForDecision != null)
            {
                return providerForDecision.Status == "Locked";
            }

            else
            {
                return false;
            }
        }
        private void Decide(object item)
        {
            ProviderOrganisation providerForDecision = item as ProviderOrganisation;
            //Store value in session and navigate.
            App.Session.Add(SessionKey.UploadProviderKey, providerForDecision.KeyValue);
            NavigationService.Navigate("/ParticipantMI/UploadDecision");
        }

        
    }
}
