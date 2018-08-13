using System;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Helpers;
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Models;

namespace UcbManagementInformation.ViewModels
{
    public interface IReportListViewModel
    {
        RelayCommand EditReportCommand { get; set; }
        bool IsRetreivingReportGroups { get; set; }
        bool IsRetreivingReports { get; set; }
        INavigationService NavigationService { get; set; }
        ObservableCollection<Report> Reports { get; set; }
        ObservableCollection<ReportGroupFolder> RootGroups { get; set; }
        Report SelectedReport { get; set; }
        ReportGroupFolder SelectedReportGroup { get; set; }
        RelayCommand ViewReportCommand { get; set; }

    }
}
