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
using UcbManagementInformation.Commanding;
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;

namespace UcbManagementInformation.ViewModels
{
    public class FakeUploadDecisionViewModel : ViewModel,IUploadDecisionViewModel
    {
        public FakeUploadDecisionViewModel()
        {
            Last2Files = new ObservableCollection<InputFileHistory>()
            {
                new InputFileHistory()
                {
                    Code = Guid.NewGuid(),
                    Filename = "c:\test",
                    FileType = "Participant",
                    LoadedBy = "Tester1",
                    LoadedDate = DateTime.Now,
                    NumberOfErrorRecords = 50,
                    NumberOfInformationalRecords = 0,
                    NumberOfRecords = 1000,
                    NumberOfValidRecords = 900,
                    NumberOfWarningRecords = 50,
                    ProviderOrganisationKeyValue = "TestYH",
                    Status = "Loaded",
                    TransferDate = DateTime.Now,
                    UploadDecision = -1
                },
                new InputFileHistory()
                {
                    Code = Guid.NewGuid(),
                    Filename = "c:\test2",
                    FileType = "Participant",
                    LoadedBy = "Tester2",
                    LoadedDate = DateTime.Now.AddDays(-1),
                    NumberOfErrorRecords = 25,
                    NumberOfInformationalRecords = 0,
                    NumberOfRecords = 500,
                    NumberOfValidRecords = 450,
                    NumberOfWarningRecords = 25,
                    ProviderOrganisationKeyValue = "TestYH",
                    Status = "Validated Loaded 2",
                    TransferDate = DateTime.Now.AddDays(-1),
                    UploadDecision = 2
                }

            };
            
            ErrorSummary = new InputFileErrorSummary()
            {
                FileLevelInfo = 1,
                FileLevelWarning = 1,
                FileLevelError = 1,

                RecordLevelInfo = 0,
                RecordLevelWarning = 50,
                RecordLevelError = 50
            };

            DecisionLevel = 2;
        }
        public ObservableCollection<InputFileHistory> Last2Files { get; set; }

        public InputFileErrorSummary ErrorSummary { get; set; }
        
        public int DecisionLevel { get; set; }

        public RelayCommand ViewAllMessagesCommand { get; set; }
        public RelayCommand ViewFileInformationalMessagesCommand { get; set; }
        public RelayCommand ViewFileWarningMessagesCommand { get; set; }
        public RelayCommand ViewFileErrorMessagesCommand { get; set; }
        public RelayCommand ViewRecordInformationalMessagesCommand { get; set; }
        public RelayCommand ViewRecordWarningMessagesCommand { get; set; }
        public RelayCommand ViewRecordErrorMessagesCommand { get; set; }

        public RelayCommand LoadCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        
    }
}
