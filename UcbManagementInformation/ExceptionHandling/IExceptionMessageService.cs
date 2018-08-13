namespace UcbManagementInformation.ExceptionHandling
{
    using System;

    public interface IExceptionMessageService
    {
        void ShowExceptionMessage(string message, Exception exception);
    }
}