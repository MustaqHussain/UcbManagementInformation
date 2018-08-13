namespace UcbManagementInformation.ExceptionHandling
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Runtime.Serialization;
    using Microsoft.Practices.EnterpriseLibrary.Validation;

    [DataContract]
    public class ServiceResult
    {
        public ServiceResult()
        {
            this.ValidationResults = Enumerable.Empty<ValidationResult>();
        }

        public ServiceResult(IEnumerable<ValidationResult> validationResults)
        {
            SetValidationErrors(validationResults);
        }

        public void SetValidationErrors(IEnumerable<ValidationResult> validationResults)
        {
            this.SetEmptyResult();
            ValidationResults = validationResults ?? Enumerable.Empty<ValidationResult>();
            this.HasError = ValidationResults.Any();
        }

        public ServiceResult(ValidationErrorException error)
        {
            this.ValidationResults = error.ValidationResults ?? Enumerable.Empty<ValidationResult>();
            this.Error = error;
            HasError = true;
        }

        public ServiceResult(Exception error)
        {
            SetError(error);
        }

        public void SetError(Exception error)
        {
            this.ValidationResults = Enumerable.Empty<ValidationResult>();
            this.Error = error;
            HasError = true;
        }

        public virtual void SetEmptyResult()
        {
            this.ValidationResults = Enumerable.Empty<ValidationResult>();
            this.Error = null;
            this.HasError = false;
        }


        public IEnumerable<ValidationResult> ValidationResults { get; private set; }

        public Exception Error { get; set; }

        public bool HasError { get; private set; }
        public bool IsHandled { get; set; }
        public object UserState { get; set; }




    }

    [DataContract]
    public class ServiceResult<T> : ServiceResult
    {
        public ServiceResult()
        {

        }
        public ServiceResult(T result)
        {
            Result = result;
        }

        public ServiceResult(IEnumerable<ValidationResult> validationResults)
            : base(validationResults)
        {
        }

        public ServiceResult(ValidationErrorException error)
            : base(error)
        {
        }

        public ServiceResult(Exception error)
            : base(error)
        {
        }

        public override void SetEmptyResult()
        {
            this.Result = default(T);
        }

        [DataMember]
        public T Result { get; set; }
    }

    public class ServiceListResult<T> : ServiceResult<IEnumerable<T>>
    {
        public ServiceListResult(IEnumerable<T> result)
            : base(result)
        {

        }

        public ServiceListResult(Exception error)
            : base(error)
        {

        }

        public ServiceListResult(IEnumerable<ValidationResult> validationResults)
            : base(validationResults)
        {
        }

        public ServiceListResult(ValidationErrorException error)
            : base(error)
        {
        }

        public ServiceListResult()
            : base(Enumerable.Empty<T>())
        {

        }

        public override void SetEmptyResult()
        {
            this.Result = Enumerable.Empty<T>();
        }
    }
}
