
namespace UcbManagementInformation.ExceptionHandling
{
    using System.Collections.Generic;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration;
    using Microsoft.Practices.EnterpriseLibrary.Common.Configuration.ContainerModel;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling;
    using Microsoft.Practices.EnterpriseLibrary.ExceptionHandling.Configuration;

    public class DisplayErrorMessageExceptionHandlerData : ExceptionHandlerData
    {
        /// <summary>
        /// Get the set of <see cref="TypeRegistration"/> objects needed to
        /// register the call handler represented by this config element and its associated objects.
        /// </summary>
        /// <param name="nameSuffix">A suffix for the names in the generated type registration objects.</param>
        /// <returns>The set of <see cref="TypeRegistration"/> objects.</returns>
        public override IEnumerable<TypeRegistration> GetRegistrations(string nameSuffix)
        {
            yield return
                new TypeRegistration<IExceptionHandler>(
                    () =>
                        new DisplayErrorMessageExceptionHandler(ErrorMessageType))
                {
                    Name = BuildName(nameSuffix),
                    Lifetime = TypeRegistrationLifetime.Transient
                };
        }

        public ErrorMessageType ErrorMessageType { get; set; }
    }
}
