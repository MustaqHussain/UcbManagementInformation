namespace UcbManagementInformation.ExceptionHandling
{
    using System;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;

    public class DisplayErrorMessageExceptionHandler : IExceptionHandler
    {
        private readonly ErrorMessageType errorMessageType;

        public DisplayErrorMessageExceptionHandler(ErrorMessageType errorMessageType)
        {
            this.errorMessageType = errorMessageType;
        }

        public Exception HandleException(Exception exception, Guid handlingInstanceId)
        {            
            // TODO: May need to consider executing this on the UI thread as per reference implementation.
            ErrorWindow.CreateNew(exception, handlingInstanceId, errorMessageType == ErrorMessageType.TechnicalErrorMessage);
            return exception;
        }
    }
}
