namespace UcbManagementInformation.ExceptionHandling
{
    using System;

    public interface IAsyncServiceRequest
    {
        ServiceResult ReturnResult(object result);
        ServiceResult ReturnError(Exception exception);
        ServiceResult ReturnEmpty();
        ServiceResult ReturnValidationErrors(ValidationErrorException validationErrorException);

        event EventHandler<AsyncServiceRequestOnCallbackEventArgs> OnCallback;
        object UserState { get; set; }
        
    }
}