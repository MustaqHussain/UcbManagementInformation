using System;
using System.Windows;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Security.Principal;

namespace UcbManagementInformation.Silverlight.UnitTest.Helpers
{
    public class FakeAuthentication : AuthenticationService
    {
        public static IPrincipal FakeUser { get; set; }

        private readonly IPrincipal _defaultUser;

        public FakeAuthentication(IPrincipal defaultUser)
        {
            this._defaultUser = defaultUser;
        }

        protected override IPrincipal CreateDefaultUser()
        {
            return this._defaultUser;
        }

        protected override IAsyncResult BeginLoadUser(AsyncCallback callback, object state)
        {
            IAsyncResult result = new FakeAsyncResult(state);
            Deployment.Current.Dispatcher.BeginInvoke(() => callback(new FakeAsyncResult(state)));
            return result;
        }

        protected override LoadUserResult EndLoadUser(IAsyncResult asyncResult)
        {
            return new LoadUserResult(FakeAuthentication.FakeUser);
        }


        protected override IAsyncResult BeginLogin(LoginParameters parameters, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        protected override LoginResult EndLogin(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        protected override IAsyncResult BeginSaveUser(IPrincipal user, AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        protected override SaveUserResult EndSaveUser(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

        protected override IAsyncResult BeginLogout(AsyncCallback callback, object state)
        {
            throw new NotSupportedException();
        }

        protected override LogoutResult EndLogout(IAsyncResult asyncResult)
        {
            throw new NotSupportedException();
        }

    }
}
