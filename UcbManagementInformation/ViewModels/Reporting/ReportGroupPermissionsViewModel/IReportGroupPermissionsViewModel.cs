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
using System.Collections.ObjectModel;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web.Models;

namespace UcbManagementInformation.ViewModels
{
    public interface IReportGroupPermissionsViewModel
    {
        bool? DialogResult { get; set; }

        ReportGroup CurrentReportGroup { get; set; }

        MCUser CurrentUser { get; set; }
        
        ReportGroupAccessLevelType CurrentPermission { get; set; }

        ObservableCollection<MCUser> Users { get; set; }

        MCUser SelectedUser { get; set; }

        ObservableCollection<string> Permissions { get; set; }

        string SelectedPermission { get; set; }

        RelayCommand ApplyButtonCommand { get; set; }
        RelayCommand OKButtonCommand { get; set; }

        RelayCommand CancelButtonCommand { get; set; }
    }
}
