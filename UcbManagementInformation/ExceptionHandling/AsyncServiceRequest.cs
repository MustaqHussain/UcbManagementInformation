namespace UcbManagementInformation.ExceptionHandling
{
    using System;

    public class AsyncServiceRequest<T> : IAsyncServiceRequest
        where T : ServiceResult, new()
    {
        public AsyncServiceRequest()
        {
        }

        public AsyncServiceRequest(Action<T> callback, object userState = null)
        {
            this.SetRequestCompleteCallback(callback);
            this.UserState = userState;
        }

        public event EventHandler<AsyncServiceRequestOnCallbackEventArgs> OnCallback = delegate { };

        public object UserState { get; set; }

        public static AsyncServiceRequest<T> RestoreFromUserState(object userData)
        {
            if (userData == null) throw new ArgumentNullException("userData");

            var asr = userData as AsyncServiceRequest<T>;
            if (asr == null)
                throw new InvalidOperationException("Could not convert userData to AsyncServiceRequest");
            return asr;
        }

        public ServiceResult ReturnValidationErrors(ValidationErrorException validationErrorException)
        {
            var result = new T();
            result.Error = validationErrorException;
            result.SetValidationErrors(validationErrorException.ValidationResults);
            return ReturnResult(result);
        }

        private Action<T> requestCompleteCallback;

        private void SetRequestCompleteCallback(Action<T> value)
        {
            requestCompleteCallback = value;
        }

        public ServiceResult ReturnResult(T result)
        {
            if (result == null)
            {
                result = new T();
                result.SetEmptyResult();
            }
            if (result.UserState == null)
            {
                result.UserState = this.UserState;
            }

            var handler = this.OnCallback;
            if (handler != null)
            {
                handler(this, new AsyncServiceRequestOnCallbackEventArgs { Result = result });
            }

            requestCompleteCallback(result);
            return result;
        }

        public static implicit operator AsyncServiceRequest<T>(Action<T> requestCompleteCallback)
        {
            return FromAction(requestCompleteCallback);
        }

        public static AsyncServiceRequest<T> FromAction(Action<T> requestCompleteCallback)
        {
            return new AsyncServiceRequest<T>(requestCompleteCallback);
        }

        ServiceResult IAsyncServiceRequest.ReturnResult(object result)
        {
            return ReturnResult((T)result);
        }

        public ServiceResult ReturnError(Exception exception)
        {
            var result = new T();
            result.SetError(exception);
            return ReturnResult(result);
        }

        public ServiceResult ReturnEmpty()
        {
            var result = new T();
            result.SetEmptyResult();
            return ReturnResult(result);
        }
    }
}