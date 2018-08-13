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
using UcbManagementInformation.Helpers;
using UcbManagementInformation.Web.Services;
using System.Windows.Browser;

namespace UcbManagementInformation.ViewModels
{
    public class UploadHistoryViewModel : ViewModel, IUploadHistoryViewModel, INavigable
    {
        private IModalDialogService modalDialogService;

        public UploadHistoryViewModel()
            : this(new MIUploadContext())
        {
            
        }
        MIUploadContext myContext;
        public UploadHistoryViewModel(MIUploadContext context)
        {
            myContext = context;
            ErrorReportCommand = new RelayCommand(cmd => ErrorReport(cmd), exe => CanErrorReport(exe));
            SummaryCommand = new RelayCommand(cmd => SummaryNavigate(cmd), exe => CanSummaryNavigate(exe));
            this.ProviderKey = (string)App.Session[SessionKey.UploadProviderKey];

            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UploadHistoryViewModel_PropertyChanged);
            
        }

        void UploadHistoryViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
 
            }
        }

        public override void Initialize()
        {
            //Retreive and populate the provider organisations
            myContext.Load<InputFileHistory>(myContext.GetInputFileHistoriesByProviderKeyQuery(providerKey),
                (lo) =>
                {
                    if (!lo.HasError)
                    {
                        HistoryList = new ObservableCollection<InputFileHistory>(lo.Entities);
                    }
                }
                , null);
        }

        private void ErrorReport(object item)
        {
            InputFileHistory uploadHistory = item as InputFileHistory;
            if (uploadHistory != null)
            {
                string UsernameParam = "&" + "UserName" + "=" + WebContext.Current.User.Name;
                string InputFileHistoryCodeParam = "&InputFileHistoryCode=" + uploadHistory.Code.ToString();
                string ErrorLevelParam = "";
                //if (errorLevel != null && Convert.ToInt32(errorLevel) >= 0)
                //{
                //    ErrorLevelParam = "&ErrorLevel=" + (Convert.ToInt32(errorLevel) + 1).ToString();
                //}
                string ErrorTypeParam = "";
                //if (errorType != null &&
                //    errorType != "")
                //{
                //    ErrorTypeParam = "&ErrorType=" + errorType;
                //}
                Uri reportUri = new Uri(App.Current.Host.Source, @"../ReportViewer2010.aspx?report=" + "ESF2007MIReports" + @"/" + "File Upload Error Report" + UsernameParam + InputFileHistoryCodeParam + ErrorLevelParam + ErrorTypeParam);

                HtmlPage.Window.Eval("var win = window.open('" + reportUri.AbsoluteUri + "', '_blank', 'toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes');");
            }
        }

        private bool CanErrorReport(object item)
        {
            return true;
        }

        private void SummaryNavigate(object item)
        {

        }

        private bool CanSummaryNavigate(object item)
        {
            //turned off until changes to decision screen is made.
            return false;
        }
        private INavigationService navigationService;

        public INavigationService NavigationService
        {
            get { return navigationService; }
            set { navigationService = value; OnPropertyChanged("NavigationService"); }
        }

        private string providerKey;

        public string ProviderKey
        {
            get { return providerKey; }
            set { providerKey = value; OnPropertyChanged("ProviderKey"); }
        }

        private InputFileHistory selectedInputHistory;

        public InputFileHistory SelectedInputHistory
        {
            get { return selectedInputHistory; }
            set { selectedInputHistory = value; OnPropertyChanged("SelectedInputHistory"); }
        }

        private ObservableCollection<InputFileHistory> historyList;

        public ObservableCollection<InputFileHistory> HistoryList
        {
            get { return historyList; }
            set { historyList = value; OnPropertyChanged("HistoryList"); }
        }

        public RelayCommand ErrorReportCommand { get; set; }
        public RelayCommand SummaryCommand { get; set; }
    }
}
