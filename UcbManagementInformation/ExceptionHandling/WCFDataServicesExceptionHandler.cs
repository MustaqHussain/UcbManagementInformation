using System;

namespace UcbManagementInformation.ExceptionHandling
{
    using System.Collections.Generic;
    //using System.Xml.Linq;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.Validation;
    //using ModelSupport.Validation;

    public class WCFDataServicesExceptionHandler : IExceptionHandler
    {
        public Exception HandleException(Exception exception, Guid handlingInstanceId)
        {
            //ValidationResults validationResults;
            //if (DataServicesValidationHelper.TryGetValidationResults(exception, out validationResults))
            //{
            //    return new ValidationErrorException(validationResults);
            //}
            //else
            //{
                return exception;
            //}
        }
    }
}
