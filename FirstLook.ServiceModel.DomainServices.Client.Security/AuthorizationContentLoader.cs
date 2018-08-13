using System;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Windows.Navigation;
using System.Windows.Controls;
using System.Windows;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public class AuthorizationContentLoader : INavigationContentLoader
    {
        private readonly Frame _frame;
        private readonly INavigationContentLoader _loader;

        public AuthorizationContentLoader(Frame frame) : this(frame, new PageResourceContentLoader()) { }

        public AuthorizationContentLoader(Frame frame, INavigationContentLoader loader)
        {
            if (frame == null)
            {
                throw new ArgumentNullException("frame");
            }
            if (loader == null)
            {
                throw new ArgumentNullException("loader");
            }
            this._frame = frame;
            this._loader = loader;
        }

        public Uri DefaultSource { get; set; }

        public IAsyncResult BeginLoad(Uri targetUri, Uri currentUri, AsyncCallback userCallback, object asyncState)
        {
            AuthorizationAsyncResult result = new AuthorizationAsyncResult(userCallback, asyncState);
            result.InnerAsyncResult = this._loader.BeginLoad(targetUri, currentUri, this.HandleLoadCompleted, result);
            return result;
        }

        private void HandleLoadCompleted(IAsyncResult asyncResult)
        {
            AuthorizationAsyncResult result = (AuthorizationAsyncResult)asyncResult.AsyncState;
            if (!result.CancellationRequested)
            {
                try
                {
                    result.LoadResult = this._loader.EndLoad(result.InnerAsyncResult);
                    AuthorizationNavigationMode navigationMode = this.GetContentNavigationMode(result.LoadResult.LoadedContent);

                    if (Authorization.Authorize(result.LoadResult.LoadedContent) == AuthorizationResult.Allowed)
                    {
                        // Direct the user to the requested page
                        result.Complete();
                    }
                    else if (WebContextBase.Current.Authentication.User.Identity.IsAuthenticated ||
                        (navigationMode == AuthorizationNavigationMode.Redirect))
                    {
                        // If the user is authenticated but not authorized, he needs to be redirected
                        result.LoadResult = new LoadResult(this.DefaultSource);
                        result.Complete();
                    }
                    else if (navigationMode == AuthorizationNavigationMode.Prompt)
                    {
                        if (Authorization.Prompter == null)
                        {
                            throw new InvalidOperationException("Authorization.Prompter cannot be null when the navigationMode is 'Prompt'.");
                        }
                        // If the user is not authenticated, we need to prompt for re-authentication or redirect
                        Authorization.Prompter.RequestAuthentication(this.HandleRequestAuthenticationCompleted, result);
                    }
                }
                catch (InvalidOperationException ex)
                {
                    result.Error = ex;
                    result.Complete();
                }
            }
        }

        private void HandleRequestAuthenticationCompleted(object userState)
        {
            AuthorizationAsyncResult result = (AuthorizationAsyncResult)userState;
            if (!result.CancellationRequested)
            {
                if (Authorization.Authorize(result.LoadResult.LoadedContent) == AuthorizationResult.Allowed)
                {
                    // Direct the user to the requested page
                    result.Complete();
                }
                else
                {
                    // If the user is still not authorized, he needs to be redirected
                    result.LoadResult = new LoadResult(this.DefaultSource);
                    result.Complete();
                }
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

        public bool CanLoad(Uri targetUri, Uri currentUri)
        {
            return (Authorization.Authorize(targetUri) == AuthorizationResult.Allowed) &&
                this._loader.CanLoad(targetUri, currentUri);
        }

        public void CancelLoad(IAsyncResult asyncResult)
        {
            AuthorizationAsyncResult result = AsyncResultBase.EndAsyncOperation<AuthorizationAsyncResult>(asyncResult, true);
            this._loader.CancelLoad(result.InnerAsyncResult);
        }

        public LoadResult EndLoad(IAsyncResult asyncResult)
        {
            AuthorizationAsyncResult result = AsyncResultBase.EndAsyncOperation<AuthorizationAsyncResult>(asyncResult);
            if (result.Error != null)
            {
                throw result.Error;
            }
            return result.LoadResult;
        }

        #region AuthorizationAsyncResult

        private class AuthorizationAsyncResult : AsyncResultBase
        {

            public AuthorizationAsyncResult(AsyncCallback userCallback, object asyncState) :
                base(userCallback, asyncState)
            {
            }

            public LoadResult LoadResult { get; set; }

            public Exception Error { get; set; }
        }

        #endregion
    }
}
