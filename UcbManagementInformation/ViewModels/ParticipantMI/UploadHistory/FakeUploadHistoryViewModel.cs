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
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.ObjectModel;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public class FakeUploadHistoryViewModel:ViewModel,IUploadHistoryViewModel
    {
        public FakeUploadHistoryViewModel()
        {
            ProviderKey = "TESTNE";
               

            HistoryList = new ObservableCollection<InputFileHistory>() 
            {
                new InputFileHistory 
                {
                    Filename="File1",
                    FileType="Participant",
                    LoadedBy="Tester1",
                    LoadedDate=DateTime.Now,
                    NumberOfErrorRecords=10,
                    NumberOfInformationalRecords=10,
                    NumberOfRecords=5,
                    NumberOfValidRecords=10,
                    NumberOfWarningRecords=12,
                    ProviderOrganisationKeyValue="TESTNE",
                    Status="ValidatedLoaded 2",
                    TransferDate=DateTime.Now,
                    UploadDecision=2,
                    ValidatedLoadDate=DateTime.Now
                },
                new InputFileHistory 
                {
                    Filename="File1",
                    FileType="Participant",
                    LoadedBy="Tester1",
                    LoadedDate=DateTime.Now.AddDays(-1),
                    NumberOfErrorRecords=10,
                    NumberOfInformationalRecords=10,
                    NumberOfRecords=5,
                    NumberOfValidRecords=10,
                    NumberOfWarningRecords=12,
                    ProviderOrganisationKeyValue="TESTNE",
                    Status="ValidatedLoaded 2",
                    TransferDate=DateTime.Now,
                    UploadDecision=2,
                    ValidatedLoadDate=DateTime.Now
                },
            };
            SelectedInputHistory = HistoryList[1];
        }

        private string providerKey;

        public string ProviderKey
        {
            get { return providerKey; }
            set { providerKey = value; OnPropertyChanged("ProviderKey"); }
        }

        private InputFileHistory selectedInputHistory;

        public InputFileHistory SelectedInputHistory
        {
            get { return selectedInputHistory; }
            set { selectedInputHistory = value; OnPropertyChanged("SelectedInputHistory"); }
        }

        private ObservableCollection<InputFileHistory> historyList;

        public ObservableCollection<InputFileHistory> HistoryList
        {
            get { return historyList; }
            set { historyList = value; OnPropertyChanged("HistoryList"); }
        }

        public RelayCommand ErrorReportCommand { get; set; }
        public RelayCommand SummaryCommand { get; set; }

     
    }
}
