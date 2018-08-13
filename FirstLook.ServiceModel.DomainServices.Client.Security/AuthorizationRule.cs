using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public abstract class AuthorizationRule
    {
        private static readonly IEnumerable<AuthorizationAttribute> NoAttributes = new AuthorizationAttribute[0];
        private static readonly IEnumerable<AuthorizationBehavior> NoBehaviors = new AuthorizationBehavior[0];

        public virtual IEnumerable<AuthorizationAttribute> GetAuthorizationAttributes(object target)
        {
            return AuthorizationRule.NoAttributes;
        }

        public virtual IEnumerable<AuthorizationBehavior> GetAuthorizationBehaviors(object target)
        {
            return AuthorizationRule.NoBehaviors;    
        }
    }
}