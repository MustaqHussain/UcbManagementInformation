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
using UcbManagementInformation.Server.DataAccess;
using System.Collections.ObjectModel;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public class FakeUploadedParticipantViewModel : ViewModel,IUploadedFileMonitorViewModel
    {
        public FakeUploadedParticipantViewModel()
        {
            ParticipantProvidersList = new ObservableCollection<ProviderOrganisation>
            {
                new ProviderOrganisation()
                {
                    ID=0,
                    KeyValue="TESTYH",
                    Name="Test Name 1",
                    RegionID="YH",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 56,
                        UploadDecision = 1,
                        ValidatedLoadDate = DateTime.Now,
                        Status = "ValidatedLoaded 1",
                        NumberOfValidRecords = 56,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(90,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked"
                    
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(70,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(40,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(50,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(60,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(30,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(80,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = DateTime.Now.Subtract(new TimeSpan(10,0,0,0)),
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                },
                new ProviderOrganisation()
                {
                    ID=1,
                    KeyValue="TESTNE",
                    Name="Test Name 2",
                    RegionID="NE",
                    Status="Unlocked",
                    InputFileHistory = new InputFileHistory
                    {
                        NumberOfRecords = 74,
                        UploadDecision = 2,
                        ValidatedLoadDate = null,
                        Status = "ValidatedLoaded 2",
                        NumberOfValidRecords = 74,
                        FileType = "Participant",
                        LoadedDate = DateTime.Now,
                        
                    }
                    
                }
            };
        }

        private ObservableCollection<ProviderOrganisation> participantProvidersList;

        public ObservableCollection<ProviderOrganisation> ParticipantProvidersList
        {
            get { return participantProvidersList; }
            set
            {
                participantProvidersList = value; DateTime? newestFile = participantProvidersList.Max(
                   x => (x.InputFileHistory != null) ? (x.InputFileHistory.ValidatedLoadDate.HasValue ? x.InputFileHistory.ValidatedLoadDate.Value : DateTime.MinValue) : DateTime.MinValue);
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
                } OnPropertyChanged("ParticipantProvidersList");
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

    }
}
