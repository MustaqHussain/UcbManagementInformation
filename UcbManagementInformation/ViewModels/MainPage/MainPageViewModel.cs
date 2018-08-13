using System;
using System.Net;
using System.Linq;
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
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Helpers;
using UcbManagementInformation.Models;
using System.Windows.Threading;
using UcbManagementInformation.Web.Services;
using UcbManagementInformation.Server.DataAccess;
using System.ServiceModel.DomainServices.Client;

namespace UcbManagementInformation.ViewModels 
{
    public class MainPageViewModel : ViewModel, IMainPageViewModel, INavigable
    {
        #region constructor

        private AlertContext myContext;

        public MainPageViewModel()
        {
            myContext = new AlertContext();

            hostUri = App.Current.Host.Source.AbsoluteUri;   
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(MainPageViewModel_PropertyChanged);
            level1MenuList = new ObservableCollection<MainMenuItem>() 
            {
                //new MainMenuItem 
                //{
                //    Name = "Geographic",
                //    Url = "",
                //    ImageUrl = "Assets/Images/maps.PNG",
                //    MenuLevel = 1,
                //    Children= new ObservableCollection<MainMenuItem>
                //    {
                //        new MainMenuItem { Name = "Participant Map", Url = "/Geographic/Maps", ImageUrl = "Assets/Images/maps.PNG", MenuLevel = 2 } ,
                //        new MainMenuItem { Name = "Heat Maps", Url = "/Geographic/HeatMap", ImageUrl = "Assets/Images/heatmap.PNG", MenuLevel = 2 } 
                //    }

                //},
                //new MainMenuItem 
                //{
                //    Name = "File Upload",
                //    Url = "",
                //    ImageUrl = "Assets/Images/fileupload.PNG",
                //    MenuLevel = 1,
                //    Children = new ObservableCollection<MainMenuItem>
                //    {
                //        new MainMenuItem { Name = "Uploaded File Monitor", Url = "/ParticipantMI/UploadedFileMonitor", ImageUrl = "Assets/Images/fileupload.PNG", MenuLevel = 2 } ,
                //        new MainMenuItem { Name = "Job Queue Monitor", Url = "/ParticipantMI/JobQueueMonitor", MenuLevel = 2 } ,
                //        new MainMenuItem { Name = "Postcode Upload", Url = "/PostCodes/TransferPostCodeFiles", MenuLevel = 2 } ,
                //        new MainMenuItem { Name = "File Csv-Xml Converter", Url = "/ParticipantMI/ParticipantCsvToXmlConverter", MenuLevel = 2 } ,
                //    }

                //},
                new MainMenuItem 
                { 
                    Name = "Reporting", 
                    Url = "", 
                    ImageUrl = "Assets/Images/report.PNG", 
                    MenuLevel = 1,
                    Children = new ObservableCollection<MainMenuItem>
                    {
                        new MainMenuItem { Name = "Ad Hoc Reports", Url = "/Reporting/ReportWizard/ReportWizard", ImageUrl = "Assets/Images/report.PNG", MenuLevel = 2 } ,
                        new MainMenuItem { Name = "Report List", Url = "/Reporting/ReportList", ImageUrl = "Assets/Images/report.PNG", MenuLevel = 2 } ,
                    }
                },
                //new MainMenuItem { Name = "INES", Url = "", ImageUrl = "Assets/Images/INES.PNG", MenuLevel = 1,
                //    Children = new ObservableCollection<MainMenuItem>
                //    {
                //        new MainMenuItem { Name = "Select Agreement", Url = "/INES/Charts/SelectionPanel", ImageUrl = "Assets/Images/INES.PNG", MenuLevel = 2 } ,
                //        new MainMenuItem { Name = "Cost Analysis", Url = "/INES/Charts/AdminCostAnalysis", ImageUrl = "Assets/Images/costanalysis.PNG", MenuLevel = 2 } ,
                //    }
                //},
                new MainMenuItem { Name = "About", Url = "/About", ImageUrl = "Assets/Images/about.PNG", MenuLevel = 1 },
                new MainMenuItem { Name = "Home", Url = "/Home", ImageUrl = "Assets/Images/home.PNG", MenuLevel = 1 },
                //new MainMenuItem 
                //{ 
                //    Name = "Timelines", 
                //    Url = "", 
                //    ImageUrl = "Assets/Images/Timeline.PNG", 
                //    MenuLevel = 1, 
                //    Children = new ObservableCollection<MainMenuItem>
                //    {
                //        new MainMenuItem { Name = "Claim/Payment Timeline", Url = "/Timeline/Timeline", ImageUrl = "Assets/Images/Timeline.PNG", MenuLevel = 2 }
                //    }
                //}

            };

            currentUrl = "/Home";
            alertItems = new ObservableCollection<Alert>();
            TopLevelSelectedCommand = new RelayCommand(cmd => TopLevelSelected(cmd));
            AlertOKCommand = new RelayCommand(cmd => AlertOK(cmd));
            //alertTimer = new DispatcherTimer();
            //alertTimer.Interval = TimeSpan.FromSeconds(30);
            //alertTimer.Tick += new EventHandler(alertTimer_Tick);
            //alertTimer.Start();
        }

        void alertTimer_Tick(object sender, EventArgs e)
        {
            myContext.Load<Alert>(myContext.GetMyAlertsQuery(),
                (lo) =>
                {
                    if (!lo.HasError)
                    {
                        foreach (Alert alertItem in lo.Entities)
                        {
                            AlertUser alertUserItem = alertItem.AlertUsers.FirstOrDefault<AlertUser>(x => x.User.Name == App.Current.ApplicationLifetimeObjects.OfType<WebContext>().First().User.Name);
                            if (alertUserItem != null)
                            {
                                if (!alertItems.Contains(alertItem))
                                {
                                    alertItems.Insert(0,alertItem);
                                }
                            }
                        }
                        
                    }
                    if (TopUnAcknowledgedAlertItem == null || TopUnAcknowledgedAlertItem.AlertUsers.First<AlertUser>(x => x.User.Name == App.Current.ApplicationLifetimeObjects.OfType<WebContext>().First().User.Name).UserMessageStatus == "Received")
                    {
                        SetUnAcknowledgedAlert();
                    }
                }, null);
        }
        #endregion

        #region PropertyChanged Handler
        void MainPageViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "CurrentLevel2Menu":
                    SecondLevelSelected(CurrentLevel2Menu);
                    break;
            }
        }
        #endregion

        #region Relay command actions
        
        private void TopLevelSelected(object item)
        {
            MainMenuItem passedItem = item as MainMenuItem;
            if (passedItem != null)
            {
                CurrentLevel1Menu = passedItem;
                Level2MenuList = passedItem.Children;
                if (!String.IsNullOrEmpty(passedItem.Url))
                {
                    CurrentUrl = passedItem.Url;
                }
                
            }

        }

        private void AlertOK(object item)
        {
            Alert alertItem = item as Alert;
            if (alertItem != null)
            {
                AlertUser alertUserItem = alertItem.AlertUsers.FirstOrDefault<AlertUser>(x => x.User.Name == App.Current.ApplicationLifetimeObjects.OfType<WebContext>().First().User.Name);
                    
                Alert alertItemInContext = myContext.Alerts.FirstOrDefault(x => x.Code == alertItem.Code);
                if (alertItemInContext != null)
                {
                    AlertUser alertUserItemContext = alertItemInContext.AlertUsers.FirstOrDefault<AlertUser>(x => x.User.Name == App.Current.ApplicationLifetimeObjects.OfType<WebContext>().First().User.Name);
                    if (alertUserItem != null)
                    {
                        alertUserItemContext.UserMessageStatus = "Received";
                        alertUserItem.UserMessageStatus = "Received";
                        myContext.SubmitChanges(AlertOKCallback, null);
                        
                    }
                }

            }
        }
        private void AlertOKCallback(SubmitOperation so)
        {
            if (so.HasError)
            { }
            SetUnAcknowledgedAlert();
        }

        private void SetUnAcknowledgedAlert()
        {
            var alertItem = (from x in AlertItems
                             where x.AlertUsers.Any(y => y.User.Name == App.Current.ApplicationLifetimeObjects.OfType<WebContext>().First().User.Name
                                 && y.UserMessageStatus != "Received")
                             select x).FirstOrDefault();

            
                TopUnAcknowledgedAlertItem = alertItem;
            
        }

        private void SecondLevelSelected(MainMenuItem item)
        {
            if (item != null)
            {
                CurrentUrl = item.Url;
            }
        }

        #endregion
        #region Private fields
        
        private DispatcherTimer alertTimer;

        #endregion
        #region ViewModel Properties

        private string hostUri;

        public string HostUri
        {
            get { return hostUri; }
            set { hostUri = value; OnPropertyChanged("HostUri"); }
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
                navigationService = value; OnPropertyChanged("NavigationService");
            }
        }

        private string currentUrl;

        public string CurrentUrl
        {
            get { return currentUrl; }
            set { currentUrl = value; OnPropertyChanged("CurrentUrl"); }
        }

        private ObservableCollection<Alert> alertItems;

        public ObservableCollection<Alert> AlertItems
        {
            get { return alertItems; }
            set { alertItems = value; OnPropertyChanged("AlertItems"); }
        }
        private Alert topUnAcknowledgedAlertItem;

        public Alert TopUnAcknowledgedAlertItem
        {
            get { return topUnAcknowledgedAlertItem; }
            set { topUnAcknowledgedAlertItem = value; OnPropertyChanged("TopUnAcknowledgedAlertItem"); }
        }

        private ObservableCollection<MainMenuItem> level1MenuList;

        public ObservableCollection<MainMenuItem> Level1MenuList
        {
            get { return level1MenuList; }
            set { level1MenuList = value; }
        }
        private MainMenuItem currentLevel1Menu;

        public MainMenuItem CurrentLevel1Menu
        {
            get { return currentLevel1Menu; }
            set { currentLevel1Menu = value; OnPropertyChanged("CurrentLevel1Menu"); }
        }
        private MainMenuItem currentLevel2Menu;

        public MainMenuItem CurrentLevel2Menu
        {
            get { return currentLevel2Menu; }
            set { currentLevel2Menu = value; OnPropertyChanged("CurrentLevel2Menu"); }
        }
        private ObservableCollection<MainMenuItem> level2MenuList;

        public ObservableCollection<MainMenuItem> Level2MenuList
        {
            get { return level2MenuList; }
            set { level2MenuList = value; OnPropertyChanged("Level2MenuList"); }
        }

        private RelayCommand topLevelSelectedCommand;

        public RelayCommand TopLevelSelectedCommand
        {
            get { return topLevelSelectedCommand; }
            set { topLevelSelectedCommand = value; }
        }

        private RelayCommand secondLevelSelectedCommand;

        public RelayCommand SecondLevelSelectedCommand
        {
            get { return secondLevelSelectedCommand; }
            set { secondLevelSelectedCommand = value; }
        }
        private RelayCommand alertOKCommand;
        public RelayCommand AlertOKCommand
        {
            get { return alertOKCommand; }
            set { alertOKCommand = value; }
        }
        #endregion

    }
    
}
