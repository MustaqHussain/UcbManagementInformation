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
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public class FakePublishToUcbViewModel : ViewModel,IPublishToUcbViewModel
    {
        public FakePublishToUcbViewModel()
        {
            ReportCategories = new ObservableCollection<ReportCategory>
            { 
                new ReportCategory(){Description="Incidents By Year"},
                new ReportCategory(){Description="Incidents By Site"},

            };
            Name = "ReportName12";
            Description = "A Report To be published";
            IsExportAllowed = true;
            IsPrintAllowed = true;
            PublishCommand = new RelayCommand(cmd => { });
            CancelCommand = new RelayCommand(cmd => { });
            DialogResult = true;
            IsSaveButtonVisible = true;
            SelectedCategory = ReportCategories[0];
        }

        private string name;
        private string description;
        private bool isPrintAllowed;
        private bool isExportAllowed;
        private bool? dialogResult;
        private bool isSaveButtonVisible;
        ReportCategory selectedCategory;
        ObservableCollection<ReportCategory> reportCategories;

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
            set { dialogResult = value; }
        }

        public bool IsSaveButtonVisible
        {
            get { return isSaveButtonVisible; }
            set { isSaveButtonVisible = value; }
        }
        RelayCommand PublishCommand { get; set; }
        RelayCommand CancelCommand { get; set; }
    }
}
