namespace UcbManagementInformation.ExceptionHandling
{
    using System;
    //using Caching;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    public static class ExceptionManagerExtensions
    {
        public static void HandleAsyncWcfServiceError(
            this ExceptionManager exceptionManager,
            Exception exception,
            IAsyncServiceRequest asyncServiceRequest,
            string policy)
        {
            // Handle the exception using the supplied exception message
            Exception errorToThrow;
            var rethrow = exceptionManager.HandleException(exception, policy, out errorToThrow);

            if (rethrow)
            {
                // The exception is not swallowed. That means the UI needs to be 
                // notified of the exception. 
                errorToThrow = errorToThrow ?? exception;

                ServiceResult result;

                if (errorToThrow is ValidationErrorException)
                {
                    result = asyncServiceRequest.ReturnValidationErrors(errorToThrow as ValidationErrorException);
                }
                else
                {
                    // Notify the UI of the problem. 
                    result = asyncServiceRequest.ReturnError(errorToThrow);
                }

                if (!result.IsHandled)
                {
                    // The UI didn't handle the exception. That means we need to rethrow the exception
                    // this will trigger the global exception handling logic in the App.xaml.cs
                    throw errorToThrow;
                }
            }
            else
            {
                // The exception is swallowed by the exception handling block. However, you will still need to notify
                // the UI that the exception was handled
                asyncServiceRequest.ReturnEmpty();
            }
        }

        public static void HandleAsyncWcfServiceError(
            this ExceptionManager exceptionManager,
            Exception exception,
            //IDownloadRequest downloadRequest,
            string policy)
        {
            //// Handle the exception using the supplied exception message
            //Exception errorToThrow;
            //var rethrow = exceptionManager.HandleException(exception, policy, out errorToThrow);

            //if (rethrow)
            //{
            //    // The exception is not swallowed. That means the UI needs to be 
            //    // notified of the exception. 
            //    errorToThrow = errorToThrow ?? exception;

            //    // Notify the UI of the problem. 
            //    var result = downloadRequest.ReturnError(errorToThrow);

            //    if (!result.IsHandled)
            //    {
            //        // The UI didn't handle the exception. That means we need to rethrow the exception
            //        // this will trigger the global exception handling logic in the App.xaml.cs
            //        throw errorToThrow;
            //    }
            //}
            //else
            //{
            //    // The exception is swallowed by the exception handling block. However, you will still need to notify
            //    // the UI that the exception was handled
            //    downloadRequest.ReturnEmpty();
            //}
        }
    }
}
