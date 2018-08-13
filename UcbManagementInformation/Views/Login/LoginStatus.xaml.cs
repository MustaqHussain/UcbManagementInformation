namespace UcbManagementInformation.LoginUI
{
    using System.Globalization;
    using System.ServiceModel.DomainServices.Client.ApplicationServices;
    using System.Windows;
    using System.Windows.Controls;
    using System.ComponentModel;

    /// <summary>
    /// <see cref="UserControl"/> class that shows the current login status and allows login and logout.
    /// </summary>
    public partial class LoginStatus : UserControl
    {
        private readonly AuthenticationService authService = WebContext.Current.Authentication;

        /// <summary>
        /// Creates a new <see cref="LoginStatus"/> instance.
        /// </summary>
        public LoginStatus()
        {
            this.InitializeComponent();
            if (DesignerProperties.IsInDesignTool)
            {
                VisualStateManager.GoToState(this, "loggedOut", false);
            }
            else
            {
                this.DataContext = WebContext.Current;
                this.welcomeText.SetBinding(TextBlock.TextProperty, WebContext.Current.CreateOneWayBinding("User.DisplayName", new StringFormatValueConverter(ApplicationStrings.WelcomeMessage)));
                this.authService.LoggedIn += this.Authentication_LoggedIn;
                this.authService.LoggedOut += this.Authentication_LoggedOut;
                this.UpdateLoginState();
            }
        }

        private void LoginButton_Click(object sender, RoutedEventArgs e)
        {
            LoginRegistrationWindow loginWindow = new LoginRegistrationWindow();
            loginWindow.Show();
        }

        private void LogoutButton_Click(object sender, RoutedEventArgs e)
        {
            this.authService.Logout(logoutOperation =>
            {
                if (logoutOperation.HasError)
                {
                    ErrorWindow.CreateNew(logoutOperation.Error);
                    logoutOperation.MarkErrorAsHandled();
                }
            }, /* userState */ null);
        }

        private void Authentication_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            this.UpdateLoginState();
        }

        private void Authentication_LoggedOut(object sender, AuthenticationEventArgs e)
        {
            this.UpdateLoginState();
        }

        private void UpdateLoginState()
        {
            if (WebContext.Current.Authentication.User.Identity.IsAuthenticated)
            {
                VisualStateManager.GoToState(this, (WebContext.Current.Authentication is WindowsAuthentication) ? "windowsAuth" : "loggedIn", true);
            }
            else
            {
                VisualStateManager.GoToState(this, "loggedOut", true);
            }
        }
    }
}
