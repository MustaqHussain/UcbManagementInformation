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
using System.Linq;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web.Services;
using UcbManagementInformation.Helpers;
using UcbManagementInformation.Server.DataAccess;
using System.ServiceModel.DomainServices.Client;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using System.Collections.ObjectModel;
using System.Windows.Browser;

namespace UcbManagementInformation.ViewModels
{
    public class UploadDecisionViewModel : ViewModel,IUploadDecisionViewModel,INavigable
    {
        MIUploadContext myContext;
        Guid uploadHistoryCode;
        string providerKey;
        public UploadDecisionViewModel() : this(new MIUploadContext())
        {
            //uploadHistoryCode = (Guid)App.Session[SessionKey.FileUploadHistoryCode];
            providerKey = (string)App.Session[SessionKey.UploadProviderKey];
        }
        
        public UploadDecisionViewModel(MIUploadContext context)
        { 
            myContext = context;
            DecisionLevel = -1;
            ViewAllMessagesCommand = new RelayCommand(cmd => ShowMessages(cmd,null,null), exe => CanShowMessages(exe));
            ViewRecordErrorMessagesCommand = new RelayCommand(cmd => ShowMessages(cmd, "2", "Record"), exe => CanShowMessages(exe));
            ViewRecordWarningMessagesCommand = new RelayCommand(cmd => ShowMessages(cmd, "1", "Record"), exe => CanShowMessages(exe));
            ViewRecordInformationalMessagesCommand = new RelayCommand(cmd => ShowMessages(cmd, "0", "Record"), exe => CanShowMessages(exe));
            ViewFileErrorMessagesCommand = new RelayCommand(cmd => ShowMessages(cmd, "2", "File"), exe => CanShowMessages(exe));
            ViewFileWarningMessagesCommand = new RelayCommand(cmd => ShowMessages(cmd, "1", "File"), exe => CanShowMessages(exe));
            ViewFileInformationalMessagesCommand = new RelayCommand(cmd => ShowMessages(cmd, "0", "File"), exe => CanShowMessages(exe));

            LoadCommand = new RelayCommand(cmd => LoadRecords(cmd), exe => CanLoadRecords(exe));
            CancelCommand = new RelayCommand(cmd => CancelLoad(cmd), exe => CanCancelLoad(exe));
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(UploadDecisionViewModel_PropertyChanged);
        }

        void UploadDecisionViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "IsValidAndInformationalAndWarning":
                    if (IsValidAndInformationalAndWarning)
                    {
                        DecisionLevel = 2;
                    }
                    break;
                case "IsValidAndInformational":
                    if (IsValidAndInformational)
                    {
                        DecisionLevel = 1;
                    }
                    break;
                case "IsValidOnly":
                    if (IsValidOnly)
                    {
                        DecisionLevel = 0;
                    }
                    break;
                case "DecisionLevel":
                    LoadCommand.UpdateCanExecuteCommand();
                    break;
                case "Last2Files":
                    LoadCommand.UpdateCanExecuteCommand();
                    CancelCommand.UpdateCanExecuteCommand();
                    break;
            }
        }

        public override void Initialize()
        {
            base.Initialize();
            myContext.Load(myContext.GetInputFileHistoryAndPreviousByProviderKeyQuery(providerKey), FileHistoriesLoadedCallback, null);
            
        }
        public void FileSummaryLoadedCallback(LoadOperation<InputFileErrorSummary> lo)
        {
            if (!lo.HasError)
            {
                ErrorSummary = lo.Entities.First();

            }
        }
        public void FileHistoriesLoadedCallback(LoadOperation<InputFileHistory> lo)
        {
            if (!lo.HasError)
            {
                Last2Files = new ObservableCollection<InputFileHistory>(lo.Entities);
                if (Last2Files.Count() > 0)
                {
                    uploadHistoryCode = Last2Files[0].Code;
                    myContext.Load(myContext.GetInputFileErrorsSummaryQuery(uploadHistoryCode), FileSummaryLoadedCallback, null);
                }
            }
        }

        private void ShowMessages(object item,string errorLevel,string errorType)
        {
            string UsernameParam = "&" + "UserName" + "=" + WebContext.Current.User.Name;
            string InputFileHistoryCodeParam = "&InputFileHistoryCode=" + uploadHistoryCode;
            string ErrorLevelParam = "";
            if (errorLevel != null && Convert.ToInt32(errorLevel) >= 0)
            {
                ErrorLevelParam = "&ErrorLevel=" + (Convert.ToInt32(errorLevel)+1).ToString();
            }
            string ErrorTypeParam = "";
            if (errorType != null &&
                errorType != "")
            {
                ErrorTypeParam = "&ErrorType=" + errorType;
            }
            Uri reportUri = new Uri(App.Current.Host.Source, @"../ReportViewer2010.aspx?report=" + "ESF2007MIReports" + @"/" + "File Upload Error Report" + UsernameParam + InputFileHistoryCodeParam + ErrorLevelParam+ErrorTypeParam);
            
            HtmlPage.Window.Eval("var win = window.open('" + reportUri.AbsoluteUri + "', '_blank', 'toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes');");
            
        }
        private bool CanShowMessages(object item)
        {
            return true;
        }

        private void LoadRecords(object item)
        {
            myContext.LoadRecords(Last2Files[0].Code, Last2Files[0].ProviderOrganisationKeyValue, DecisionLevel.ToString(),LoadRecordsCallback,null);

        }
        private void LoadRecordsCallback(InvokeOperation<Guid> io)
        {
            if (!io.HasError)
            {
                App.Session.Add(SessionKey.JobCode,io.Value);
                NavigationService.Navigate("/ParticipantMI/JobQueueMonitor");
            }
        }

        private bool CanLoadRecords(object item)
        {
            return DecisionLevel >= 0 && Last2Files[0].Status == "Business Validated";
        }
        private void CancelLoad(object item)
        {
            myContext.CancelUpload(Last2Files[0].Code, Last2Files[0].ProviderOrganisationKeyValue, CancelUploadCallback, null);
        }
        private void CancelUploadCallback(InvokeOperation<Guid> io)
        {
            if (!io.HasError)
            {
                App.Session.Add(SessionKey.JobCode, io.Value);
                NavigationService.Navigate("/ParticipantMI/JobQueueMonitor");
            }
        }
        private bool CanCancelLoad(object item)
        {
            return Last2Files != null && Last2Files.Count > 0 && Last2Files[0].Status == "Business Validated";
        }
        private ObservableCollection<InputFileHistory> last2Files; 
        public ObservableCollection<InputFileHistory> Last2Files 
        { 
            get {return last2Files;}
            set { last2Files = value; OnPropertyChanged("Last2Files"); }
        }
        private InputFileErrorSummary errorSummary;
        public InputFileErrorSummary ErrorSummary 
        { 
            get {return errorSummary;}
            set {errorSummary=value;OnPropertyChanged("ErrorSummary");}
        }
        private int decisionLevel;
        public int DecisionLevel 
        {
            get { return decisionLevel; }
            set { decisionLevel = value;OnPropertyChanged("DecisionLevel");}
        }
        private bool isValidOnly;
        public bool IsValidOnly
        {
            get { return isValidOnly; }
            set { isValidOnly = value; OnPropertyChanged("IsValidOnly"); }
        }
        private bool isValidAndInformational;
        public bool IsValidAndInformational
        {
            get { return isValidAndInformational; }
            set { isValidAndInformational = value; OnPropertyChanged("IsValidAndInformational"); }
        }
        private bool isValidAndInformationalAndWarning;
        public bool IsValidAndInformationalAndWarning
        {
            get { return isValidAndInformationalAndWarning; }
            set { isValidAndInformationalAndWarning = value; OnPropertyChanged("IsValidAndInformationalAndWarning"); }
        }

        public RelayCommand ViewAllMessagesCommand { get; set; }
        public RelayCommand ViewFileInformationalMessagesCommand { get; set; }
        public RelayCommand ViewFileWarningMessagesCommand { get; set; }
        public RelayCommand ViewFileErrorMessagesCommand { get; set; }
        public RelayCommand ViewRecordInformationalMessagesCommand { get; set; }
        public RelayCommand ViewRecordWarningMessagesCommand { get; set; }
        public RelayCommand ViewRecordErrorMessagesCommand { get; set; }

        public RelayCommand LoadCommand { get; set; }
        public RelayCommand CancelCommand { get; set; }

        private INavigationService navigationService;

        public INavigationService NavigationService
        {
            get { return navigationService; }
            set { navigationService = value; OnPropertyChanged("NavigationService"); }
        }
    }
}
