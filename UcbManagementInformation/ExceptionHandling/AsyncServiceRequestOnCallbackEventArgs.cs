namespace UcbManagementInformation.ExceptionHandling
{
    using System;

    public class AsyncServiceRequestOnCallbackEventArgs : EventArgs
    {
        public ServiceResult Result { get; set; }
    }
}