using System;
using FirstLook.ServiceModel.DomainServices.Client.Security;

namespace UcbManagementInformation.LoginUI
{
    public class LoginRegistrationWindowPrompter : AuthorizationPrompter
    {
        public override void RequestAuthentication(Action<object> completionCallback, object userState)
        {
            LoginRegistrationWindow window = new LoginRegistrationWindow();
            window.Show();
            window.Closed += (sender, e) => completionCallback(userState);
        }
    }
}
