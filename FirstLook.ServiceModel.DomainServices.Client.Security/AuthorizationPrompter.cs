using System;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public abstract class AuthorizationPrompter
    {
        public abstract void RequestAuthentication(Action<object> completionCallback, object userState);
    }
}
