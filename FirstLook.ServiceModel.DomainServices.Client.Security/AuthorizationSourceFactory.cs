using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public abstract class AuthorizationSourceFactory
    {
        public abstract AuthorizationSource CreateSource(IEnumerable<AuthorizationAttribute> attributes);
    }
}
