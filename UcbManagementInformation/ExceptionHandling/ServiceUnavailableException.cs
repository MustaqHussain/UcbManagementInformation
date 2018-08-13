namespace UcbManagementInformation.ExceptionHandling
{
    using System;
    using System.Runtime.Serialization;

    [DataContract]
    public class ServiceUnavailableException : Exception
    {
        private readonly string stackTrace;

        public ServiceUnavailableException()
        {
        }

        public ServiceUnavailableException(string message) : base(message)
        {
        }

        public ServiceUnavailableException(string message, Exception inner) : base(message, inner)
        {
        }

        public ServiceUnavailableException(string message, string stackTrace, Exception inner)
            : base(message, inner)
        {
            this.stackTrace = stackTrace;
        }

        public override string StackTrace
        {
            get { return this.stackTrace; }
        }
    }
}