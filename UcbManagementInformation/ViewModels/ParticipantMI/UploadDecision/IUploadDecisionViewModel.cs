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
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;

namespace UcbManagementInformation.ViewModels
{
    public interface IUploadDecisionViewModel
    {
        ObservableCollection<InputFileHistory> Last2Files { get; set; }
        InputFileErrorSummary ErrorSummary { get; set; }
        
        int DecisionLevel { get; set; }

        RelayCommand ViewAllMessagesCommand { get; set; }
        RelayCommand ViewFileInformationalMessagesCommand { get; set; }
        RelayCommand ViewFileWarningMessagesCommand { get; set; }
        RelayCommand ViewFileErrorMessagesCommand { get; set; }
        RelayCommand ViewRecordInformationalMessagesCommand { get; set; }
        RelayCommand ViewRecordWarningMessagesCommand { get; set; }
        RelayCommand ViewRecordErrorMessagesCommand { get; set; }

        RelayCommand LoadCommand { get; set; }
        RelayCommand CancelCommand { get; set; }
    }
}
