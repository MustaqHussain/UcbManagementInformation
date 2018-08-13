namespace UcbManagementInformation
{
    using System;
    using System.Runtime.Serialization;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using System.Windows;
    using System.Windows.Controls;
    using UcbManagementInformation.Controls;
    using System.Collections.Generic;
    using FirstLook.ServiceModel.DomainServices.Client.Security;
    using UcbManagementInformation.LoginUI;
    using UcbManagementInformation.Helpers;

    using UcbManagementInformation.Helpers.ExceptionHandling;
    using UcbManagementInformation.Helpers.Configuration;

    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using System.Threading;
    using System.Globalization;
    using Microsoft.Practices.EnterpriseLibrary.Logging.Service;
    using System.Windows.Media;

    /// <summary>
    /// Main <see cref="Application"/> class.
    /// </summary>
    public partial class App : Application
    {

        #region Private variables
        private System.Windows.Controls.BusyIndicator busyIndicator;
        private bool enterpriseLibraryConfigured = false;
        AsyncXamlConfigMerger asyncXamlConfigMerger = new AsyncXamlConfigMerger();
        private static SessionManager<SessionKey, object> session = new SessionManager<SessionKey, object>();
        #endregion

        #region Properties
        public static SessionManager<SessionKey, object> Session
        {
            get { return session; }
            set { session = value; }
        }
        #endregion

        /// <summary>
        /// Creates a new <see cref="App"/> instance.
        /// </summary>
        public App()
        {
            Thread.CurrentThread.CurrentCulture = new CultureInfo("en-GB");
            Thread.CurrentThread.CurrentUICulture = new CultureInfo("en-GB");
            
            //this.Startup += this.Application_Startup;
            //this.Exit += this.Application_Exit;
            this.UnhandledException += this.Application_UnhandledException;

            InitializeComponent();

            // Create a WebContext and add it to the ApplicationLifetimeObjects
            // collection.  This will then be available as WebContext.Current.
            WebContext webContext = new WebContext();
            //webContext.Authentication = new FormsAuthentication();
            webContext.Authentication = new WindowsAuthentication();
            this.ApplicationLifetimeObjects.Add(webContext);
            Authorization.Prompter = new LoginRegistrationWindowPrompter();
        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {
            // When the application starts, we have to download the XAML configuration first. After that, we can continue to start the application. 
            string configurationVersion;
            e.InitParams.TryGetValue("configurationVersion", out configurationVersion);
            ConfigureEnterpriseLibraryAsynchronous(configurationVersion);

            Bootstrapper.InitializeIoc();
            // This will enable you to bind controls in XAML files to WebContext.Current
            // properties
            this.Resources.Add("WebContext", WebContext.Current);

            //Thread.Sleep(10000);

            // This will automatically authenticate a user when using windows authentication
            // or when the user chose "Keep me signed in" on a previous login attempt
            WebContext.Current.Authentication.LoadUser(this.Application_UserLoaded, null);
            // Show some UI to the user while LoadUser is in progress
            this.InitializeRootVisual();
        }

        /// <summary>
        /// Invoked when the <see cref="LoadUserOperation"/> completes. Use this
        /// event handler to switch from the "loading UI" you created in
        /// <see cref="InitializeRootVisual"/> to the "application UI"
        /// </summary>
        private void Application_UserLoaded(LoadUserOperation operation)
        {
            this.busyIndicator.IsBusy = false;
        }

        /// <summary>
        /// Initializes the <see cref="Application.RootVisual"/> property. The
        /// initial UI will be displayed before the LoadUser operation has completed
        /// (The LoadUser operation will cause user to be logged automatically if
        /// using windows authentication or if the user had selected the "keep
        /// me signed in" option on a previous login).
        /// </summary>
        protected virtual void InitializeRootVisual()
        {
            this.busyIndicator = new System.Windows.Controls.BusyIndicator ();
            this.busyIndicator.IsBusy = true;
            this.busyIndicator.Content = new MainPage();
            this.busyIndicator.HorizontalContentAlignment = HorizontalAlignment.Stretch;
            this.busyIndicator.VerticalContentAlignment = VerticalAlignment.Stretch;
            
            this.RootVisual = this.busyIndicator;
        }

        private void Application_UnhandledException(object sender, ApplicationUnhandledExceptionEventArgs e)
        {
            if (!enterpriseLibraryConfigured)
            {
                // If something goes wrong before Enterprise Library has been configured, we can't use enterprise library
                // for exeption handling. The only thing we can do here is to report this to show an error message
                // Since the UI also has not been loaded yet, we need to create the root visual as a blank page
                App.Current.RootVisual = new UserControl();
                ErrorWindow.CreateNew("An unknown problem has occurred while starting the application", e.ExceptionObject, Guid.Empty, true);
                e.Handled = true;
                return;
            }

            // Call the Unhandled exception policy. This should:
            // 1. Log the error
            // 2. Show a friendly error message
            // 3. Exit the application
            var exceptionManager = EnterpriseLibraryContainer.Current.GetInstance<ExceptionManager>();
            exceptionManager.HandleException(e.ExceptionObject, ExceptionHandlingPolicies.UnhandledException);
            
            e.Handled = true;
        }

        private void Application_Exit(object sender, EventArgs e)
        {

        }

        private void ConfigureEnterpriseLibraryAsynchronous(string configurationVersion)
        {
            // Use a version (does not need to be numeric or incremental) to force the
            // configuration files to re-download in case that the browser cache would serve a 
            // stale configuration file when a different querystring is not specified
            // (how the browser caches the config files also depends on the server configuration).

            // This version parameter comes from the HTML page used to serve the application, and needs
            // to be explicitly updated whenever the configuration is updated.
            // Look in Default.aspx, at the initParams property of the Silverlight object tag.
            string versionQueryString = !string.IsNullOrEmpty(configurationVersion)
                                            ? "?version=" + configurationVersion
                                            : null;

            asyncXamlConfigMerger.DownloadComplete += OnConfigurationCompleted;

            asyncXamlConfigMerger.AddXamlFilesToDownload(
                new[]
                    {
                        //new Uri("http://mcon-op-web1.esffiredom.co.uk/mconweb/Configuration/UcbManagementInformation.Silverlight.Config.xaml" + versionQueryString, UriKind.Absolute)
                        //new Uri("http://localhost:9999/Configuration/UcbManagementInformation.Silverlight.Config.xaml" + versionQueryString, UriKind.Absolute)
                        //new Uri("Configuration/UcbManagementInformation.Silverlight.Config.xaml" + versionQueryString, UriKind.Relative)
                        new Uri(App.Current.Host.Source,"../Configuration/UcbManagementInformation.Silverlight.Config.xaml" + versionQueryString)
                    });
        }

        void OnConfigurationCompleted(object sender, EnterpriseLibraryConfigurationCompletedEventArgs e)
        {
            if (!e.ConfiguredSuccessfully)
            {
                throw new InvalidOperationException("Configuration Failed", e.Error);
            }
            enterpriseLibraryConfigured = true;
        }

    }
}