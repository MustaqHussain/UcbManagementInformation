using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.ObjectModel;
using UcbManagementInformation.Web.Models;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web.Server;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using UcbManagementInformation.Models;

namespace UcbManagementInformation.ViewModels
{
    public class ReportGroupPermissionsViewModel : ViewModel,IReportGroupPermissionsViewModel
    {
        private ReportWizardContext myContext = new ReportWizardContext();
        public ReportGroupPermissionsViewModel(ReportGroupFolder currentGroup)
        {
            OKButtonCommand = new RelayCommand(cmd => OKButton(cmd),exe=>CanOKButton(exe));
            ApplyButtonCommand = new RelayCommand(cmd => ApplyButton(cmd),exe=>CanApplyButton(exe));
            CancelButtonCommand = new RelayCommand(cmd => CancelButton(cmd));

            currentUser = WebContext.Current.User;
            currentPermission = currentGroup.AccessLevel;
            permissions = new ObservableCollection<string>();
            if (ReportGroupAccessLevelType.Admin <= currentPermission)
            {
                permissions.Add(ReportGroupAccessLevelType.Admin.ToString());
            }
            if (ReportGroupAccessLevelType.Delete <= currentPermission)
            {
                permissions.Add(ReportGroupAccessLevelType.Delete.ToString());
            }
            if (ReportGroupAccessLevelType.Update <= currentPermission)
            {
                permissions.Add(ReportGroupAccessLevelType.Update.ToString());
            }
            if (ReportGroupAccessLevelType.ReadOnly <= currentPermission)
            {
                permissions.Add(ReportGroupAccessLevelType.ReadOnly.ToString());
            }
            if (ReportGroupAccessLevelType.None <= currentPermission)
            {
                permissions.Add(ReportGroupAccessLevelType.None.ToString());
            }
            permissions.Add("Inherit");

            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportGroupPermissionsViewModel_PropertyChanged);

            myContext.Load<MCUser>(myContext.GetAllMCUserQuery(), LoadBehavior.MergeIntoCurrent, GetAllUserCallback, null);
            myContext.Load<ReportGroup>(myContext.GetReportGroupByCodeQuery(currentGroup.Code), LoadBehavior.MergeIntoCurrent, GetReportGroupCallback, null);
        }

        void ReportGroupPermissionsViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedUser":
                    if (selectedUser != null)
                    {
                        var userReportGroup = (from x in myContext.UserReportGroups
                                               where x.UserCode == selectedUser.Code && x.ReportGroupCode == currentReportGroup.Code
                                               select x).FirstOrDefault();
                        if (!(userReportGroup == null))
                        {
                            SelectedPermission = ((ReportGroupAccessLevelType)userReportGroup.AccessLevel).ToString();
                        }
                        else
                        {
                            SelectedPermission = "Inherit";
                        }
                    }
                    OKButtonCommand.UpdateCanExecuteCommand();
                    ApplyButtonCommand.UpdateCanExecuteCommand();
                    break;
                case "SelectedPermission":
                   OKButtonCommand.UpdateCanExecuteCommand();
                   ApplyButtonCommand.UpdateCanExecuteCommand();
                   break;
            }
        }

        #region ApplyButton Delegates
        private bool CanApplyButton(object item)
        {
            return selectedPermission != null && selectedUser != null;
        }
        private void ApplyButton(object item)
        {
            UpdatePermission("Apply");
        }
        #endregion
        void UpdatePermission(string typeOfUpdate)
        {
            var userReportGroup = (from x in myContext.UserReportGroups
                                   where x.UserCode == selectedUser.Code && x.ReportGroupCode == currentReportGroup.Code
                                   select x).FirstOrDefault();
            if (selectedPermission == "Inherit")
            {
                if (userReportGroup != null)
                {
                    myContext.UserReportGroups.Remove(userReportGroup);
                }
            }
            else
            {
                if (userReportGroup != null)
                {
                    userReportGroup.AccessLevel = (int)(ReportGroupAccessLevelType)Enum.Parse(typeof(ReportGroupAccessLevelType), selectedPermission, false);
                }
                else
                {
                    myContext.UserReportGroups.Add(new UserReportGroup()
                    {
                        AccessLevel = (int)(ReportGroupAccessLevelType)Enum.Parse(typeof(ReportGroupAccessLevelType), selectedPermission, false),
                        UserCode = selectedUser.Code,
                        ReportGroupCode = currentReportGroup.Code,
                        Code = Guid.NewGuid(),
                        RowIdentifier = new byte[0]
                    });
                }
            }
            myContext.SubmitChanges(SubmitCallback,typeOfUpdate);
        }
        void SubmitCallback(SubmitOperation so)
        {
            if (!so.HasError)
            {
                if ((string)so.UserState == "OK")
                {
                    DialogResult = true;
                }
            }
        }
        #region OKButton Delegates
        private bool CanOKButton(object item)
        {
            return selectedPermission != null && selectedUser != null;
        }
        private void OKButton(object item)
        {
            UpdatePermission("OK");
        }
        #endregion
        #region CancelButton Delegates
       private void CancelButton(object item)
        {
            DialogResult = false;
        }
        #endregion
        private void GetAllUserCallback(LoadOperation<MCUser> lo)
        {
            if (!lo.HasError)
            {
                Users = new ObservableCollection<MCUser>(lo.Entities);
                var userItem = (from x in Users
                                where x.Code == currentUser.Code
                                select x).FirstOrDefault();
                if (userItem != null)
                {
                    Users.Remove(userItem);
                }
            }
        }

        private void GetReportGroupCallback(LoadOperation<ReportGroup> lo)
        {
            if (!lo.HasError)
            {
                CurrentReportGroup = new List<ReportGroup>(lo.Entities)[0];
            }
        }

        private ReportGroup currentReportGroup;

        public ReportGroup CurrentReportGroup 
        {
            get {return currentReportGroup;}
            set { currentReportGroup = value; OnPropertyChanged("CurrentReportGroup"); }
        }
        private bool? dialogResult;

        public bool? DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; OnPropertyChanged("DialogResult"); }
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
        

        public RelayCommand OKButtonCommand { get; set; }
        public RelayCommand ApplyButtonCommand { get; set; }
        public RelayCommand CancelButtonCommand { get; set; }
    }

    
}
