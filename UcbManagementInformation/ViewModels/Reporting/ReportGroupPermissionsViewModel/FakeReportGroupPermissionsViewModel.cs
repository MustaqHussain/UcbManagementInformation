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
using UcbManagementInformation.Web.Models;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public class FakeReportGroupPermissionsViewModel : ViewModel,IReportGroupPermissionsViewModel
    {
        public FakeReportGroupPermissionsViewModel()
        {
            OKButtonCommand = new RelayCommand(cmd => { });
            ApplyButtonCommand = new RelayCommand(cmd => { });
            CancelButtonCommand = new RelayCommand(cmd => { });

            permissions = new ObservableCollection<string> 
            {
                ReportGroupAccessLevelType.Admin.ToString(), 
                ReportGroupAccessLevelType.Delete.ToString(), 
                ReportGroupAccessLevelType.None.ToString(), 
                ReportGroupAccessLevelType.ReadOnly.ToString(), 
                ReportGroupAccessLevelType.Update.ToString(),
                "Inherit"
            };
            currentReportGroup = new ReportGroup { Name = "Dave Group 1", PathName = "RootGroup/Parent Group/Dave Group 1/" };
            currentUser = new MCUser { Name = "Ian Brown" };
            currentPermission = ReportGroupAccessLevelType.Admin;
            users = new ObservableCollection<MCUser>()
            {
                new MCUser{Name="dhays"},
                new MCUser{Name="jandrews"},
                new MCUser{Name="jbuxton"}
            };
            selectedUser = users[0];
            selectedPermission = "ReadOnly";
        }
        private bool? dialogResult;

        public bool? DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; OnPropertyChanged("DialogResult"); }
        }

        private ReportGroup currentReportGroup;

        public ReportGroup CurrentReportGroup 
        {
            get {return currentReportGroup;}
            set { currentReportGroup = value; OnPropertyChanged("CurrentReportGroup"); }
        }

        private MCUser currentUser;
        public MCUser CurrentUser
        {
            get { return currentUser; }
            set { currentUser = value; OnPropertyChanged("CurrentUser"); }
        }

        private ReportGroupAccessLevelType currentPermission;
        public ReportGroupAccessLevelType CurrentPermission
        {
            get { return currentPermission; }
            set { currentPermission = value; OnPropertyChanged("CurrentPermission"); }
        }
        private ObservableCollection<MCUser> users;
        public ObservableCollection<MCUser> Users
        {
            get { return users; }
            set { users = value; OnPropertyChanged("Users"); }
        }
        private MCUser selectedUser;
        public MCUser SelectedUser
        {
            get { return selectedUser; }
            set { selectedUser = value; OnPropertyChanged("SelectedUser"); }
        }
        private string selectedPermission;
        public string SelectedPermission
        {
            get { return selectedPermission; }
            set { selectedPermission = value; OnPropertyChanged("SelectedPermission"); }
        }
        private ObservableCollection<string> permissions;
        public ObservableCollection<string> Permissions
        {
            get { return permissions; }
            set { permissions = value; OnPropertyChanged("Permissions"); }
        }

        public RelayCommand ApplyButtonCommand { get; set; }
        public RelayCommand OKButtonCommand { get; set; }
        public RelayCommand CancelButtonCommand { get; set; }
    }
}
