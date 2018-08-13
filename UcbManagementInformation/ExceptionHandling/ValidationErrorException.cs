namespace UcbManagementInformation.ExceptionHandling
{
    using System;
    using System.Collections.Generic;
    using System.Runtime.Serialization;
    using Microsoft.Practices.EnterpriseLibrary.Validation;

    [DataContract]
    public class ValidationErrorException : Exception
    {
        [DataMember]
        public string Code { get; set; }

        [DataMember]
        public IEnumerable<ValidationResult> ValidationResults { get; private set; }

        public ValidationErrorException(IEnumerable<ValidationResult> validationResults) : base("Validation errors have occurred")
        {
            ValidationResults = validationResults;
        }

        public ValidationErrorException(IEnumerable<ValidationResult> validationResults, Exception inner, string code) : base(string.Empty, inner)
        {
            Code = code;
            ValidationResults = validationResults;
        }
    }
}