using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Navigation;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    internal class AuthorizationNavigationManager : DependencyObject
    {
        /// <summary>
        /// The DependencyProperty for <c>Source</c>
        /// </summary>
        private static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register(
                "Source",
                typeof(Uri),
                typeof(AuthorizationNavigationManager),
                new PropertyMetadata(null, AuthorizationNavigationManager.SourcePropertyChanged));

        /// <summary>
        /// The DependencyProperty for <c>ContentLoader</c>
        /// </summary>
        private static readonly DependencyProperty ContentLoaderProperty =
            DependencyProperty.Register(
                "ContentLoader",
                typeof(INavigationContentLoader),
                typeof(AuthorizationNavigationManager),
                new PropertyMetadata(null, AuthorizationNavigationManager.ContentLoaderPropertyChanged));

        private readonly Frame _frame;

        private bool _isLoaded;
        private AuthorizationNavigationMode _navigationMode;

        public AuthorizationNavigationManager(Frame frame, AuthorizationNavigationMode navigationMode)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }
            this._frame = frame;
            this.NavigationMode = navigationMode;

            // Since there is no reliable way to determine if a control is loaded, we have to assume it
            // is loaded until told otherwise
            this.IsLoaded = true;

            this.InitializeBindings();
        }

        public AuthorizationNavigationMode NavigationMode
        {
            get
            {
                return this._navigationMode;
            }

            set
            {
                if (this._navigationMode != value)
                {
                    if (this.IsLoaded && (this._navigationMode != AuthorizationNavigationMode.None))
                    {
                        this.RemoveListeners();
                    }

                    this._navigationMode = value;

                    if (this.IsLoaded && (this._navigationMode != AuthorizationNavigationMode.None))
                    {
                        this.AddListeners();
                    }
                }
            }
        }

        private bool IsLoaded
        {
            get
            {
                return this._isLoaded;
            }

            set
            {
                if (this._isLoaded != value)
                {
                    if (this._isLoaded && (this.NavigationMode != AuthorizationNavigationMode.None))
                    {
                        this.RemoveListeners();
                    }

                    this._isLoaded = value;

                    if (this._isLoaded && (this.NavigationMode != AuthorizationNavigationMode.None))
                    {
                        this.AddListeners();
                    }
                }
            }
        }
        

        private void InitializeBindings()
        {
            this._frame.Loaded += this.Frame_Loaded;
            this._frame.Unloaded += this.Frame_Unloaded;

            BindingOperations.SetBinding(
                this,
                AuthorizationNavigationManager.SourceProperty,
                new Binding("Source") { Source = this._frame });
            BindingOperations.SetBinding(
                this,
                AuthorizationNavigationManager.ContentLoaderProperty,
                new Binding("ContentLoader") { Source = this._frame });
        }

        private static void SourcePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AuthorizationNavigationManager manager = (AuthorizationNavigationManager)sender;

            manager.SetDefaultSource((Uri)e.NewValue);
        }

        private static void ContentLoaderPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            AuthorizationNavigationManager manager = (AuthorizationNavigationManager)sender;

            if (!(e.NewValue is AuthorizationContentLoader))
            {
                manager._frame.ContentLoader = new AuthorizationContentLoader(
                    manager._frame,
                    (INavigationContentLoader)e.NewValue ?? new PageResourceContentLoader());
            }

            manager.SetDefaultSource(manager._frame.Source);
        }

        private Uri GetDefaultSource()
        {
            AuthorizationContentLoader loader = this._frame.ContentLoader as AuthorizationContentLoader;
            if (loader != null)
            {
                return loader.DefaultSource;
            }

            return null;
        }

        private void SetDefaultSource(Uri source)
        {
            AuthorizationContentLoader loader = this._frame.ContentLoader as AuthorizationContentLoader;
            if (loader != null)
            {
                if (loader.DefaultSource == null)
                {
                    loader.DefaultSource = source;
                }
            }
        }

        private void AddListeners()
        {
            WebContextBase.Current.Authentication.LoggedIn += this.Authentication_LoggedIn;
            WebContextBase.Current.Authentication.LoggedOut += this.Authentication_LoggedOut;
        }

        private void RemoveListeners()
        {
            WebContextBase.Current.Authentication.LoggedIn -= this.Authentication_LoggedIn;
            WebContextBase.Current.Authentication.LoggedOut -= this.Authentication_LoggedOut;
        }

        private void Frame_Loaded(object sender, RoutedEventArgs e)
        {
            this.IsLoaded = true;
        }

        private void Frame_Unloaded(object sender, RoutedEventArgs e)
        {
            this._isLoaded = false;
        }

        private void Authentication_LoggedIn(object sender, AuthenticationEventArgs e)
        {
            this.AuthorizeFrameContent();
        }

        private void Authentication_LoggedOut(object sender, AuthenticationEventArgs e)
        {
            this.AuthorizeFrameContent();
        }

        private void AuthorizeFrameContent()
        {
            if (!AuthorizationNavigationManager.IsAuthorized(this.GetCurrentUri(), this._frame.Content))
            {
                AuthorizationNavigationMode navigationMode = this.GetContentNavigationMode(this._frame.Content);

                if (WebContextBase.Current.Authentication.User.Identity.IsAuthenticated ||
                    (navigationMode == AuthorizationNavigationMode.Redirect))
                {
                    // If the user is authenticated but not authorized, he needs to be redirected
                    this.Redirect();
                }
                else if (navigationMode == AuthorizationNavigationMode.Prompt)
                {
                    if (Authorization.Prompter == null)
                    {
                        throw new InvalidOperationException("Authorization.Prompter cannot be null when the navigationMode is 'Prompt'.");
                    }
                    // If the user is not authenticated, we need to prompt for re-authentication or redirect
                    Authorization.Prompter.RequestAuthentication(this.HandleRequestAuthenticationCompleted, null);
                }
            }
        }

        private void HandleRequestAuthenticationCompleted(object userState)
        {
            if (!AuthorizationNavigationManager.IsAuthorized(this.GetCurrentUri(), this._frame.Content))
            {
                // If the user is still not authorized, he needs to be redirected
                this.Redirect();
            }
        }

        private AuthorizationNavigationMode GetContentNavigationMode(object content)
        {
            AuthorizationNavigationMode navigationMode = AuthorizationNavigationMode.None;

            DependencyObject contentDo = content as DependencyObject;
            if (contentDo != null)
            {
                navigationMode = Authorization.GetNavigationMode(contentDo);
            }

            if (navigationMode == AuthorizationNavigationMode.None)
            {
                navigationMode = Authorization.GetNavigationMode(this._frame);
            }

            return navigationMode;
        }

        private Uri GetCurrentUri()
        {
            Uri uri = this._frame.CurrentSource;

            // When it is possible, we'll want to return the mapped uri for authorization
            if ((uri != null) && (this._frame.UriMapper != null))
            {
                uri = this._frame.UriMapper.MapUri(uri);
            }

            return uri;
        }

        private static bool IsAuthorized(Uri uri, object content)
        {
            return ((uri == null) || (Authorization.Authorize(uri) == AuthorizationResult.Allowed)) &&
                   ((content == null) || (Authorization.Authorize(content) == AuthorizationResult.Allowed));
        }

        private void Redirect()
        {
            this.FlushCache();
            this._frame.Navigate(this.GetDefaultSource());
        }

        // TODO: make sure cache is flushed at all the necessary times
        private void FlushCache()
        {
            int oldCacheSize = this._frame.CacheSize;
            this._frame.CacheSize = 0;
            this._frame.CacheSize = oldCacheSize;
        }
    }
}
