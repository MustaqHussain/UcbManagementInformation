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
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Models;
using System.Collections.ObjectModel;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public interface ISelectReportGroupViewModel
    {
        bool? DialogResult { get; set; }
        RelayCommand OKCommand { get; set; }
        string ReportName { get; set; }
        ObservableCollection<ReportGroupFolder> RootGroups { get; set; }
        ReportGroupFolder SelectedReportGroup { get; set; }
    }
}

