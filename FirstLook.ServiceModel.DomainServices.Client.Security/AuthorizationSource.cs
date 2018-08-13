using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.ComponentModel;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public class AuthorizationSource : INotifyPropertyChanged
    {
        private readonly IEnumerable<AuthorizationAttribute> _attributes;
        private AuthorizationResult _result;

        public AuthorizationSource(IEnumerable<AuthorizationAttribute> attributes)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }
            this._attributes = attributes.ToArray();

            this.Initialize();
        }

        private void Initialize()
        {
            if (this.Attributes.Any())
            {
                WebContextBase.Current.Authentication.LoggedIn += this.HandleAuthenticationEvent;
                WebContextBase.Current.Authentication.LoggedOut += this.HandleAuthenticationEvent;
            }
            this.Authorize();
        }

        private void HandleAuthenticationEvent(object sender, AuthenticationEventArgs e)
        {
            this.Authorize();
        }

        public AuthorizationResult Authorize()
        {
            this.Result = this.AuthorizeCore();
            return this.Result;
        }

        protected virtual AuthorizationResult AuthorizeCore()
        {
            // All authorization conditions must pass (AND)
            using (AuthorizationContext context = new AuthorizationContext(null))
            {
                foreach (AuthorizationAttribute attribute in this.Attributes)
                {
                    AuthorizationResult result =
                        attribute.Authorize(WebContextBase.Current.Authentication.User, context);
                    if (result != AuthorizationResult.Allowed)
                    {
                        return result;
                    }
                }
            }
            return AuthorizationResult.Allowed;
        }

        public AuthorizationResult Result
        {
            get
            {
                return this._result;
            }

            private set
            {
                if (this._result != value)
                {
                    this._result = value;
                    this.RaisePropertyChanged("Result");
                }
            }
        }

        protected IEnumerable<AuthorizationAttribute> Attributes
        {
            get { return this._attributes; }
        }

        protected void RaisePropertyChanged(string propertyName)
        {
            this.OnPropertyChanged(new PropertyChangedEventArgs(propertyName));
        }

        protected virtual void OnPropertyChanged(PropertyChangedEventArgs e)
        {
            PropertyChangedEventHandler handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
    }
}
