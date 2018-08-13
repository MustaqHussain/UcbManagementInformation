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
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public interface ISelectReportModelDialogViewModel
    {
        ObservableCollection<DataModel> DataModels { get; set; }
        DataModel SelectedDataModel { get; set; }
        RelayCommand OKCommand { get; set; }
        RelayCommand CancelCommand { get; set; }
        bool? DialogResult { get; set; }

    }
}
