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
using UcbManagementInformation.Server.DataAccess;
using System.Collections.Generic;
using UcbManagementInformation.Web.Services;
using System.ServiceModel.DomainServices.Client;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web.Server;

namespace UcbManagementInformation.ViewModels
{
    public class PublishToUcbViewModel : ViewModel, IPublishToUcbViewModel
    {
        public PublishToUcbViewModel(IEnumerable<ReportCategory> passedCategories,string passedName, string passedDescription)
        {
            name = passedName;
            description = passedDescription;
            ReportCategories = new ObservableCollection<ReportCategory>(passedCategories);
            InitializeCommand = new RelayCommand(cmd => Initialize(cmd));
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(PublishToUcbViewModel_PropertyChanged);    
            
        }

        void PublishToUcbViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                case "Description":
                case "SelectedCategory":
                    SaveCommand.UpdateCanExecuteCommand();
                    break;
            }
        }
        private string name;
        private string description;
        private bool isPrintAllowed;
        private bool isExportAllowed;
        private bool? dialogResult;
        private bool isSaveButtonVisible;
        private bool isRetreivingCategories;
        private RelayCommand saveCommand;
        private RelayCommand cancelCommand; 
        ReportCategory selectedCategory;
        ObservableCollection<ReportCategory> reportCategories;
        private ReportWizardContext myContext = new ReportWizardContext();
        #region Initialize - Called via a Relay command from the loaded event in the view
        private void Initialize(object item)
        {
            if (WebContext.Current.User.IsAuthenticated)
            {
                SaveCommand = new RelayCommand(cmd => SaveClicked(cmd), can => canClickSave(can));
                CancelCommand = new RelayCommand(cmd => CancelClicked(cmd));
                //IsRetreivingCategories = true;
                //myContext.Load<ReportCategory>(myContext.GetReportCategorysQuery(), LoadReportQueriesCompleted, null);
            }
        }
        #endregion

        private void LoadReportQueriesCompleted(LoadOperation<ReportCategory> lo)
        {
            //ReportCategories = new ObservableCollection<ReportCategory>(lo.Entities);
            //IsRetreivingCategories = false;
           
        }
        #region Relay command 'execute' and 'canexecute' implementations

        private void SaveClicked(object item)
        {
            DialogResult = true;
        }

        //If Caption, Description or location are empty then cannot click save button
        private bool canClickSave(object item)
        {
            if (String.IsNullOrWhiteSpace(Name)
                || String.IsNullOrWhiteSpace(Description)
                || selectedCategory == null)
            {
                IsSaveButtonVisible = false;
                return false;
            }
            else
            {
                IsSaveButtonVisible = true;
                return true;
            }
        }

        private void CancelClicked(object item)
        {
            DialogResult = false;
        }


        #endregion

        /// <summary>
        /// Callback called when the save button has been clicked and the edited data item has been saved to the database
        /// </summary>
        /// <param name="lo">The data item that was updated/param>
        void SaveCLickedCallback(SubmitOperation lo)
        {
            if (!lo.HasError)
            {
                DialogResult = true;
            }
        }
      

        public bool IsRetreivingCategories
        {
            get
            {
                return isRetreivingCategories;
            }
            set
            {
                isRetreivingCategories = value; OnPropertyChanged("IsRetreivingCategories");
            }
        }
        public string Name
        {
            get
            {
                return name;
            }
            set
            {
                name = value; OnPropertyChanged("Name");
            }
        }
        public string Description
        {
            get
            {
                return description;
            }
            set
            {
                description = value; OnPropertyChanged("Description");
            }
        }
        public bool IsPrintAllowed
        {
            get
            {
                return isPrintAllowed;
            }
            set
            {
                isPrintAllowed = value; OnPropertyChanged("IsPrintAllowed");
            }
        }
        public bool IsExportAllowed
        {
            get
            {
                return isExportAllowed;
            }
            set
            {
                isExportAllowed = value; OnPropertyChanged("IsExportAllowed");
            }
        }

        public ReportCategory SelectedCategory
        {
            get
            {
                return selectedCategory;
            }
            set
            {
                selectedCategory = value; OnPropertyChanged("SelectedCategory");
            }
        }

        public ObservableCollection<ReportCategory> ReportCategories
        {
            get
            {
                return reportCategories;
            }
            set
            {
                reportCategories = value; OnPropertyChanged("ReportCategories");
            }
        }
        public bool? DialogResult
        {
            get { return dialogResult; }
            set { dialogResult = value; OnPropertyChanged("DialogResult"); }
        }

        public bool IsSaveButtonVisible
        {
            get { return isSaveButtonVisible; }
            set { isSaveButtonVisible = value; OnPropertyChanged("IsSaveButtonVisible"); }
        }

        public RelayCommand InitializeCommand{get;set;}
        /// <summary>
        /// Property containing the relay command for the Save button
        /// </summary>
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand;
            }
            set
            {
                saveCommand = value; OnPropertyChanged("SaveCommand");
            }
        }

        /// <summary>
        /// Property containing the relay command for the Cancel button
        /// </summary>
        public RelayCommand CancelCommand
        {
            get
            {
                return cancelCommand;
            }
            set
            {
                cancelCommand = value; OnPropertyChanged("CancelCommand");
            }
        }
    } 
}
