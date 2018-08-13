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
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;
using UcbManagementInformation.Web.Server;
using UcbManagementInformation.Web.Models;
using System.ServiceModel.DomainServices.Client;
using UcbManagementInformation.Helpers;
using System.Collections.Generic;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.ViewModels
{
    public class SelectReportGroupViewModel : ViewModel,ISelectReportGroupViewModel
    {
        private ReportWizardContext myContext;
        private ReportGroupFolder selectedReportGroup;
        private string reportName;
        private RelayCommand oKCommand;
        private RelayCommand cancelCommand;
        private bool? dialogResult;
        private Guid _reportGroupCode = Guid.Empty;
        
        public SelectReportGroupViewModel():this(null)
        {  
        }
        public SelectReportGroupViewModel(Guid reportGroupCode, string reportName)
            : this(null,reportGroupCode,reportName)
        {
        }
        public SelectReportGroupViewModel(ReportWizardContext context,Guid reportGroupCode,string reportNamePassed)
        {
            _reportGroupCode = reportGroupCode;
            ReportName = reportNamePassed;
            if (context == null)
            {
                myContext = new ReportWizardContext();
            }
            else
            {
                myContext = context;
            }
            
            rootGroups = new ObservableCollection<ReportGroupFolder>();
            oKCommand = new RelayCommand(cmd => OKPressed(cmd), exe => CanOKPressed(exe));
            CancelCommand = new RelayCommand(cmd => CancelPressed(cmd));
            //myContext.GetAllReportGroupsForUser(GetReportGroupsForUserCallBack, null);
            myContext.Load<ReportGroup>(myContext.GetAllReportGroupsForUserQuery(), GetReportGroupsForUserCallBack, null);
        }
        public SelectReportGroupViewModel(ReportWizardContext context) : this(context, Guid.Empty, null)
        {
            
        }
        private void GetReportGroupsForUserCallBack(LoadOperation<ReportGroup> lo)
        {
            TreeViewHelper.GetAccessLevels(WebContext.Current.User.Name, lo.Entities);
            TreeViewHelper.AssembleTreeView(RootGroups,ReportGroupAccessLevelType.Update,new List<ReportGroup>(lo.Entities),true,_reportGroupCode);
            
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectReportGroupViewModel_PropertyChanged);
        }

        void SelectReportGroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedReportGroup":
                case "ReportName":
                    OKCommand.UpdateCanExecuteCommand();
                    break;
            }
        }
        private bool CanOKPressed(object item)
        {
            if (!string.IsNullOrWhiteSpace(ReportName) && SelectedReportGroup != null && SelectedReportGroup.AccessLevel >= ReportGroupAccessLevelType.Update)
            { return true; }
            else
            { return false; }
        }

        private void OKPressed(object item)
        {
            DialogResult = true;
        }

        private void CancelPressed(object item)
        {
            DialogResult = false;
        }

        private ObservableCollection<ReportGroupFolder> rootGroups;
        public ReportGroupFolder SelectedReportGroup
        {
            get { return selectedReportGroup; }
            set { selectedReportGroup = value; OnPropertyChanged("SelectedReportGroup"); }
        }
        public ObservableCollection<ReportGroupFolder> RootGroups
        {
            get { return rootGroups; }
            set { rootGroups = value; OnPropertyChanged("RootGroups"); }
        }
        public string ReportName
        {
            get { return reportName; }
            set { reportName = value;OnPropertyChanged("ReportName");}
        }
        public RelayCommand OKCommand
        {
            get { return oKCommand; }
            set { oKCommand = value; OnPropertyChanged("OKCommand"); }
        }
        public RelayCommand CancelCommand
        {
            get { return cancelCommand; }
            set { cancelCommand = value; OnPropertyChanged("CancelCommand"); }
        }
        public bool? DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; OnPropertyChanged("DialogResult"); }
        }
    }
}
