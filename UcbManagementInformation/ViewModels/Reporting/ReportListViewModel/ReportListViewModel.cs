using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq.Expressions;
using System.Linq;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Models;
using System.Collections.ObjectModel;
using UcbManagementInformation.Web.Models;
using UcbManagementInformation.Web.Server;
using System.ServiceModel.DomainServices.Client;
using System.Collections.Generic;
using UcbManagementInformation.Helpers;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Commanding;
using System.Windows.Browser;
using System.Text;
using UcbManagementInformation.Controls;
using UcbManagementInformation.ServiceLocation;
using UcbManagementInformation.Web.Services;

namespace UcbManagementInformation.ViewModels
{
    public class ReportListViewModel : ViewModel, IReportListViewModel,INavigable
    {
        private ReportWizardContext myContext;// = new ReportWizardContext();
        //private UcbPublishContext myPublishContext;
        private IModalDialogService modalDialogService;
        private IMessageBoxService messageBoxService;
        

        #region constructors

        
        public ReportListViewModel()
            : this(new ReportWizardContext(), SimpleServiceLocator.Instance.Get<IMessageBoxService>(), SimpleServiceLocator.Instance.Get<IModalDialogService>())
        {
            
        }
        //public ReportListViewModel(ReportWizardContext context,  IMessageBoxService messageBoxServiceItem, IModalDialogService modalDialogServiceItem) :
        //    this(context,  messageBoxServiceItem, modalDialogServiceItem)
        //{ }
        public ReportListViewModel(ReportWizardContext context, IMessageBoxService messageBoxServiceItem, IModalDialogService modalDialogServiceItem)
        {
            messageBoxService = messageBoxServiceItem;
            modalDialogService = modalDialogServiceItem;
            //set the internal reference to the context
            myContext = context;
            //myPublishContext = publishContext;
            //initialize the root report group collection
            rootGroups = new ObservableCollection<ReportGroupFolder>();
           
            //Set up the Relay commands
            InitializeCommand = new RelayCommand(cmd => Initialize(cmd));
            ViewReportCommand = new RelayCommand(cmd => ViewReport(cmd), exe => CanViewReport(exe));
            EditReportCommand = new RelayCommand(cmd => EditReport(cmd), exe => CanEditReport(exe));
            DeleteReportCommand = new RelayCommand(cmd =>
            {
                var result = this.messageBoxService.Show("Are you sure you want to delete this report?",
                   "Please confirm", GenericMessageBoxButton.OkCancel);
                if (result == GenericMessageBoxResult.Ok)
                {
                    DeleteReport(cmd);
                }
            }, exe => CanDeleteReport(exe));
            PublishReportCommand = new RelayCommand(cmd => PublishReport(cmd), exe => CanPublishReport(exe));
            RenameFolderCommand = new RelayCommand(cmd => RenameFolder(cmd), exe => CanRenameFolder(exe));
            RenamingCompleteCommand = new RelayCommand(cmd => RenamingComplete(cmd), exe => CanRenamingComplete(exe));
            DeleteFolderCommand = new RelayCommand(cmd =>
            {
                var result = this.messageBoxService.Show("Are you sure you want to delete this folder?",
                   "Please confirm", GenericMessageBoxButton.OkCancel);
                if (result == GenericMessageBoxResult.Ok)
                {
                    DeleteFolder(cmd);
                }
            }, exe => CanDeleteFolder(exe));
            ChangeFolderPermissionsCommand = new RelayCommand(cmd => ChangeFolderPermissions(cmd), exe => CanChangeFolderPermissions(exe));
            NewFolderCommand = new RelayCommand(cmd => NewFolder(cmd), exe => CanAddChildFolder(exe));
            
            //Listener required for property changed
            PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(SelectReportGroupViewModel_PropertyChanged);
            
        }
        #endregion

        #region Initialize - Called via a Relay command from the loaded event in the view
        private void Initialize(object item)
        {
            if (WebContext.Current.User.IsAuthenticated)
            {
                PrepareReportGroups();
            }
        }
        #endregion

        #region property changed logic
        void SelectReportGroupViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedReportGroup":
                    if (selectedReportGroup != null)
                    {
                        if (selectedReportGroup.AccessLevel >= ReportGroupAccessLevelType.ReadOnly)
                        {
                            if (!selectedReportGroup.IsJustAdded)
                            {
                                IsSubmittingContext = true;
                                IsRetreivingReports = true;
                                myContext.Load<Report>(myContext.GetAllReportsForReportGroupQuery(selectedReportGroup.Code), GetReportsForReportGroupCallback, null);
                            }

                            else
                            {
                                selectedReportGroup.IsJustAdded = false;
                                selectedReportGroup.IsRenaming = true;
                            }
                        }
                    }
                    ViewReportCommand.UpdateCanExecuteCommand();
                    EditReportCommand.UpdateCanExecuteCommand();
                    break;
                case "SelectedReport":
                            ViewReportCommand.UpdateCanExecuteCommand();
                            EditReportCommand.UpdateCanExecuteCommand();
                            DeleteReportCommand.UpdateCanExecuteCommand();
                            PublishReportCommand.UpdateCanExecuteCommand();
                     break;
                    
            }
        }
        #endregion
        private void LoadedPage(object item)
        {
            //if (WebContext.Current.User.IsAuthenticated)
            //{
            //    PrepareReportGroups();
            //}
   
            //PrepareReportGroups();
            
        }

       

        private void PrepareReportGroups()
        {
            IsRetreivingReportGroups = true;
            IsSubmittingContext = true;
            myContext.Load<ReportGroup>(myContext.GetAllReportGroupsForUserQuery(), GetReportGroupsForUserCallBack, null);
            //myContext.GetAllReportGroupsForUser(GetReportGroupsForUserCallBack, null);
        }
        #region viewreport command delegates
        private void ViewReport(object item)
        {
            myContext.PutReportInSession(selectedReportGroup.FullPath + selectedReport.Name, PutReportInSessionCallback, null);
        }

        private void PutReportInSessionCallback(object o)
        {
            Uri reportUri = new Uri(App.Current.Host.Source, @"../ReportViewer2010.aspx");
            HtmlPage.Window.Eval("var win = window.open('" + reportUri.AbsoluteUri + "', '_blank', 'toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes');");
        }
        private bool CanViewReport(object item)
        {
            return selectedReport != null && selectedReportGroup != null &&selectedReportGroup.AccessLevel > ReportGroupAccessLevelType.None;
        }
        #endregion

        #region edit report command delegates
        private void EditReport(object item)
        {
            App.Session[SessionKey.CurrentReportCode] = selectedReport.Code;
            this.NavigationService.Navigate("/Reporting/ReportWizard/ReportWizard");
        }
        private bool CanEditReport(object item)
        {
            ReportGroup reportsReportGroup = null;
            if (selectedReport != null)
            {
                reportsReportGroup = (from x in myContext.ReportGroups
                                                  where x.Code == selectedReport.GroupCode
                                                  select x).FirstOrDefault();
            }
            return selectedReport != null && reportsReportGroup != null && !selectedReport.IsStandard && reportsReportGroup.AccessLevel > ReportGroupAccessLevelType.ReadOnly;
        }
        #endregion
        #region publish report command delegates
        private void PublishReport(object item)
        {
            myContext.Load<ReportCategory>(myContext.GetReportCategorysQuery(), LoadReportQueriesCompleted, null);

            App.Session[SessionKey.CurrentReportCode] = selectedReport.Code;
            
        }

        private void LoadReportQueriesCompleted(LoadOperation<ReportCategory> lo)
        {
            var ReportCategories = new ObservableCollection<ReportCategory>(lo.Entities);
            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>("PublishToUcbDialog");
            this.modalDialogService = SimpleServiceLocator.Instance.Get<IModalDialogService>();
            this.modalDialogService.ShowDialog(dialog, new PublishToUcbViewModel(ReportCategories,SelectedReport != null ? SelectedReport.Name : null, SelectedReport != null ? SelectedReport.Description : null),
            returnedViewModelInstance =>
            {
                if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                {
                    StandardReport spToPublish = new StandardReport();
                    spToPublish.IsExportable = returnedViewModelInstance.IsExportAllowed;
                    spToPublish.IsPrintable = returnedViewModelInstance.IsPrintAllowed;
                    spToPublish.ReportCategoryCode = returnedViewModelInstance.SelectedCategory.Code;
                    spToPublish.ReportToPublishCode = selectedReport.Code;
                    spToPublish.ReportName = returnedViewModelInstance.Name;
                    spToPublish.ReportDescription = returnedViewModelInstance.Description;
                    spToPublish.Code = Guid.NewGuid();
                    IsPublishing = true;
                    IsSubmittingContext = true;
                    myContext.StandardReports.Add(spToPublish);
                    myContext.SubmitChanges(ReportPublishedCallback, spToPublish);
                }

            }); 
        }
        private bool CanPublishReport(object item)
        {
            ReportGroup reportsReportGroup = null;
            if (selectedReport != null)
            {
                reportsReportGroup = (from x in myContext.ReportGroups
                                      where x.Code == selectedReport.GroupCode
                                      select x).FirstOrDefault();
            }
            return selectedReport != null && reportsReportGroup != null && reportsReportGroup.AccessLevel > ReportGroupAccessLevelType.ReadOnly;
        }

        private void ReportPublishedCallback(SubmitOperation so)
        {
            StandardReport spToPublish = so.UserState as StandardReport;
            
            //DeleteFolderCommand.UpdateCanExecuteCommand();
            IsPublishing = false;
            IsSubmittingContext = false;

        }
        #endregion
        #region delete report command delegates
        private void DeleteReport(object item)
        {
            IsSubmittingContext = true;
                        
            myContext.Reports.Remove(selectedReport);
            myContext.SubmitChanges(
                so =>
                {
                    if (!so.HasError)
                    {
                        Reports.Remove(selectedReport);
                    }
                    IsSubmittingContext = false;
                }
            , null);
        }
        private bool CanDeleteReport(object item)
        {
            ReportGroup reportsReportGroup = null;
            if (selectedReport != null)
            {
                reportsReportGroup = (from x in myContext.ReportGroups
                                      where x.Code == selectedReport.GroupCode
                                      select x).FirstOrDefault();
            }
            return selectedReport != null && reportsReportGroup != null && !selectedReport.IsStandard && reportsReportGroup.AccessLevel >= ReportGroupAccessLevelType.Delete;
        }
        #endregion
        #region RenameFolderCommand delegates
        private bool CanRenameFolder(object item)
        {
            ReportGroupFolder folderToRename = item as ReportGroupFolder;
            if (folderToRename != null)
            {
                return folderToRename.AccessLevel >= ReportGroupAccessLevelType.Update;
            }
            return false;
        }
        private void RenameFolder(object item)
        {
            ReportGroupFolder folderToRename = item as ReportGroupFolder;
            if (folderToRename != null)
            {
                folderToRename.IsRenaming = true;
            }
        }
        #endregion
        #region RenamingCompleteCommand delegates
        private bool CanRenamingComplete(object item)
        {
            ReportGroupFolder folderToRename = item as ReportGroupFolder;
            if (folderToRename != null)
            {
                return folderToRename.AccessLevel >= ReportGroupAccessLevelType.Update;
            }
            return false;
        }
        private void RenamingComplete(object item)
        {
            if (!myContext.IsSubmitting)
            {
                IsSubmittingContext = true;
                        
                IsUpdatingReportGroups = true;
                ReportGroupFolder folderToRename = item as ReportGroupFolder;
                if (folderToRename != null)
                {
                    //Adjust name for duplicates in same folder.
                    folderToRename = AdjustName(folderToRename, false);
                    int depth = 0;
                    if (folderToRename.Parent == null)
                    {
                        depth = 0;
                    }
                    else
                    {
                        depth = folderToRename.Parent.FullPath.Split('/').Count() - 1;
                    }
                     var reportToUpdate = myContext.ReportGroups.First(rg => rg.Code == folderToRename.Code);
                    reportToUpdate.PathName = folderToRename.FullPath;
                    reportToUpdate.Name = folderToRename.Name; 
                    UpdateChildPaths(folderToRename, folderToRename.Code, folderToRename.Name, depth);//, updateList);
                    
                    myContext.SubmitChanges(FolderRenamedCallback, folderToRename);
                }
            }
        }
        
        private void FolderRenamedCallback(SubmitOperation so)
        {
            ReportGroupFolder folderToRename = so.UserState as ReportGroupFolder;
            TreeViewHelper.GetAccessLevels(WebContext.Current.User.Name, myContext.ReportGroups);
            if (so.HasError)
            {


            }
            else
            {
                
            }
            IsSubmittingContext = false;
            folderToRename.IsRenaming = false;
            IsUpdatingReportGroups = false;
        }
        #endregion
        #region private utility methods
        private ReportGroupFolder AdjustName(ReportGroupFolder folderToAdjust,bool isAdd)
        {
            int i = 0;
            if (folderToAdjust.Parent == null)
            {
                i = RootGroups.Count();
            }
            else
            {
               i = folderToAdjust.Parent.Children.Where(x => x.Name == folderToAdjust.Name).Count();
            } 
            if ((i > 0 && isAdd) || (i > 1 && !isAdd))
            {
                folderToAdjust.Name = folderToAdjust.Name + "(" + (i).ToString() + ")";
                folderToAdjust.FullPath = folderToAdjust.Parent.FullPath + folderToAdjust.Name + "/";
                folderToAdjust = AdjustName(folderToAdjust,isAdd);
            }
            return folderToAdjust;
        }
        private void UpdateChildPaths(ReportGroupFolder group, Guid parentCode, String reportGroupName, int depth)//, List<ReportGroupForUser> updatedList)
        {
            IEnumerable<ReportGroupFolder> childGroups = group.Children.Where(x => x.Parent.Code == parentCode);
            foreach (ReportGroupFolder currentChild in childGroups)
            {
                string[] folderHierarchy = currentChild.FullPath.Split('/');
                folderHierarchy[depth] = reportGroupName;
                StringBuilder pathBuilder = new StringBuilder();
                foreach (string currentSegment in folderHierarchy)
                {
                    if (!string.IsNullOrEmpty(currentSegment))
                    {
                        pathBuilder.Append(currentSegment + "/");
                    }
                }
                currentChild.FullPath = pathBuilder.ToString();
                
                //Update the underlying entities
                var reportToUpdate = myContext.ReportGroups.First(rg => rg.Code == currentChild.Code);
                reportToUpdate.PathName = currentChild.FullPath;
                reportToUpdate.ParentPath = currentChild.Parent.FullPath;
                reportToUpdate.Name = currentChild.Name;
        
                UpdateChildPaths(currentChild, currentChild.Code, reportGroupName, depth);//,updatedList);
            }
           
        }
        #endregion
        #region NewFolderCommand delegates
        private bool CanAddChildFolder(object item)
        {
            ReportGroupFolder folderToAddChild = item as ReportGroupFolder;
            if (folderToAddChild != null)
            {
                return folderToAddChild.AccessLevel >= ReportGroupAccessLevelType.Update;
            }
            return false;
        }
        private void NewFolder(object item)
        {
            IsUpdatingReportGroups = true;
            IsSubmittingContext = true;
                
            ReportGroupFolder folderToAddChild = item as ReportGroupFolder;
            if (folderToAddChild != null)
            {
                
                folderToAddChild.IsExpanded = true;
                //Do add child folder
                ReportGroupFolder foldertoAdd = new ReportGroupFolder() { Code = Guid.NewGuid(),AccessLevel = ReportGroupAccessLevelType.Admin, Color = "Yellow", FullPath = folderToAddChild.FullPath + "New Folder/", IsExpanded = true, IsTemp = false, Name = "New Folder", Parent = folderToAddChild };
                //Adjust name for duplicates in same folder.
                foldertoAdd = AdjustName(foldertoAdd,true);
                
                //Do not add child to tree here or busyindicator will stop focus setting.
                //folderToAddChild.Children.Add(foldertoAdd);

                //sort reporting services;
                myContext.ReportGroups.Add(new ReportGroup() { Code = foldertoAdd.Code,ReportType="A", AccessLevel = ReportGroupAccessLevelType.Admin, Name = foldertoAdd.Name, ParentCode = foldertoAdd.Parent.Code, ParentPath = foldertoAdd.Parent.FullPath, PathName = foldertoAdd.Parent.FullPath + foldertoAdd.Name + "/" });
                myContext.SubmitChanges(NewFolderAddedCallback, foldertoAdd);
                //myContext.AddNewReportGroupForUser(new ReportGroupForUser() { Code = foldertoAdd.Code, AccessLevel = ReportGroupAccessLevelType.Admin, Name = foldertoAdd.Name, ParentCode = foldertoAdd.Parent.Code, ParentPath = foldertoAdd.Parent.FullPath, PathName = foldertoAdd.Parent.FullPath + foldertoAdd.Name + "/" }, NewFolderAddedCallback, foldertoAdd);
            }
        }

        private void NewFolderAddedCallback(SubmitOperation so)
        {
            //turn busy indicator off
            IsUpdatingReportGroups = false;
            TreeViewHelper.GetAccessLevels(WebContext.Current.User.Name, myContext.ReportGroups);
            
            ReportGroupFolder folderToAdd = so.UserState as ReportGroupFolder;
            if (!so.HasError)
            {
                folderToAdd.Parent.Children.Add(folderToAdd);
            }
            else
            {
            }
            IsSubmittingContext = false;
            folderToAdd.IsJustAdded = true;
            
        }

        #endregion
        #region DeleteFolderCommand delegates
        private bool CanDeleteFolder(object item)
        {
            ReportGroupFolder folderToDelete = item as ReportGroupFolder;
            if (folderToDelete != null)
            {
                var deletingFolder = (from x in myContext.ReportGroups
                                      where x.Code == folderToDelete.Code
                                      select x).Single();
                return deletingFolder.AccessLevel >= ReportGroupAccessLevelType.Delete && !IsRetreivingReports && (folderToDelete.Children == null || folderToDelete.Children.Count == 0) && deletingFolder.Reports.Count==0;
            }
            return false;
        }
        private void DeleteFolder(object item)
        {
            IsUpdatingReportGroups = true;
            IsSubmittingContext = true;
                
            ReportGroupFolder folderToDelete = item as ReportGroupFolder;
            if (folderToDelete != null)
            {
                if (CanDeleteFolder(folderToDelete))
                {
                    var rgToDelete = myContext.ReportGroups.First(rg => rg.Code == folderToDelete.Code);
                    myContext.ReportGroups.Remove(rgToDelete);
                    myContext.SubmitChanges(FolderDeletedCallback, folderToDelete);
                    // myContext.DeleteReportGroupForUser(new ReportGroupForUser() { Code = folderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Admin, Name = folderToDelete.Name, ParentCode = folderToDelete.Parent.Code, ParentPath = folderToDelete.Parent.FullPath, PathName = folderToDelete.Parent.FullPath + folderToDelete.Name + "/" }, FolderDeletedCallback, folderToDelete);
         
                }
            }
        }

        private void FolderDeletedCallback(SubmitOperation so)
        {
            ReportGroupFolder folderToDelete = so.UserState as ReportGroupFolder;
            if (!so.HasError)
            {
                if (!(folderToDelete.Parent == null))
                {
                    folderToDelete.Parent.Children.Remove(folderToDelete);
                }
                else
                {
                    this.RootGroups.Remove(folderToDelete);
                }
            }
            else
            {
                
            }
            //DeleteFolderCommand.UpdateCanExecuteCommand();
            IsUpdatingReportGroups = false;
            IsSubmittingContext = false;
                
        }
        #endregion
        #region ChangeFolderPermissionsCommand delegates
        private bool CanChangeFolderPermissions(object item)
        {
            ReportGroupFolder folderToChange = item as ReportGroupFolder;
            if (folderToChange != null)
            {
                return folderToChange.AccessLevel >= ReportGroupAccessLevelType.Admin;
            }
            return false;
        }
        private void ChangeFolderPermissions(object item)
        {
            ReportGroupFolder folderToChange = item as ReportGroupFolder;
            if (folderToChange != null)
            {
                var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>("ReportGroupPermissions");
                this.modalDialogService.ShowDialog(dialog, new ReportGroupPermissionsViewModel(folderToChange),
                    returnedViewModelInstance =>
                    {
                        if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                        {
                            //SelectedDataModel = returnedViewModelInstance.SelectedDataModel;

                        }
                    });
            }
        }
        #endregion

        public RelayCommand InitializeCommand { get; set; }

        public RelayCommand ViewReportCommand { get; set; }
        public RelayCommand EditReportCommand { get; set; }
        public RelayCommand DeleteReportCommand { get; set; }
        public RelayCommand PublishReportCommand { get; set; }
        
        public RelayCommand RenameFolderCommand { get; set; }
        public RelayCommand RenamingCompleteCommand { get; set; }
        public RelayCommand NewFolderCommand { get; set; }
        public RelayCommand DeleteFolderCommand { get; set; }
        public RelayCommand ChangeFolderPermissionsCommand { get; set; }
        
      
        #region callback functions for report groups and reports retreival
        private void GetReportGroupsForUserCallBack(LoadOperation<ReportGroup> lo)
        {
            TreeViewHelper.GetAccessLevels(WebContext.Current.User.Name, lo.Entities);
            TreeViewHelper.AssembleTreeView(RootGroups,ReportGroupAccessLevelType.ReadOnly,new List<ReportGroup>(lo.Entities),true);
            IsRetreivingReportGroups = false;
            IsSubmittingContext = false;
                        
        }
        private void GetReportsForReportGroupCallback(LoadOperation<Report> lo)
        {
            Reports = new ObservableCollection<Report>(lo.Entities);
            IsRetreivingReports = false;
            IsSubmittingContext = false;
            //DeleteFolderCommand.UpdateCanExecuteCommand();
        }

        #endregion

        #region public properties
        private ObservableCollection<Report> reports;
        public ObservableCollection<Report> Reports
        {
            get { return reports; }
            set { reports = value; OnPropertyChanged("Reports"); }
        }
        private Report selectedReport;
        public Report SelectedReport
        {
            get { return selectedReport; }
            set { selectedReport = value; OnPropertyChanged("SelectedReport"); }
        }
        private ReportGroupFolder selectedReportGroup;
        public ReportGroupFolder SelectedReportGroup
        {
            get { return selectedReportGroup; }
            set { selectedReportGroup = value; OnPropertyChanged("SelectedReportGroup"); }
        }
        private ObservableCollection<ReportGroupFolder> rootGroups;

        public ObservableCollection<ReportGroupFolder> RootGroups
        {
            get { return rootGroups; }
            set { rootGroups = value; OnPropertyChanged("RootGroups"); }
        }
        private INavigationService navigationService;
        public INavigationService NavigationService
        {
            get
            {
                return navigationService;
            }
            set
            {
                navigationService=value;OnPropertyChanged("NavigationService");
            }
        }
        private bool isRetreivingReports;
        public bool IsRetreivingReports
        {
            get
            {
                return isRetreivingReports;
            }
            set
            {
                isRetreivingReports = value; OnPropertyChanged("IsRetreivingReports");
            }
        }
        private bool isRetreivingReportGroups;
 
        public bool IsRetreivingReportGroups
        {
            get
            {
                return isRetreivingReportGroups;
            }
            set
            {
                isRetreivingReportGroups = value; OnPropertyChanged("IsRetreivingReportGroups");
            }
        }
        private bool isSubmittingContext;
        public bool IsSubmittingContext
        {
            get
            {
                return isSubmittingContext;
            }
            set
            {
                isSubmittingContext = value; OnPropertyChanged("IsSubmittingContext");
            }
        }
        private bool isUpdatingReportGroups;
        public bool IsUpdatingReportGroups
        {
            get
            {
                return isUpdatingReportGroups;
            }
            set
            {
                isUpdatingReportGroups = value; OnPropertyChanged("IsUpdatingReportGroups");
            }
        }
        private bool isPublishing;
        public bool IsPublishing
        {
            get
            {
                return isPublishing;
            }
            set
            {
                isPublishing = value; OnPropertyChanged("IsPublishing");
            }
        }
        #endregion
    }
}
