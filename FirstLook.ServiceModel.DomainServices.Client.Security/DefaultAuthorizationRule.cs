using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public class DefaultAuthorizationRule : AuthorizationRule
    {
        private readonly IEnumerable<AuthorizationAttribute> _attributes;
        private readonly IEnumerable<AuthorizationBehavior> _behaviors;

        public DefaultAuthorizationRule(AuthorizationAttribute attribute)
            : this(new[] { attribute })
        {
        }

        public DefaultAuthorizationRule(IEnumerable<AuthorizationAttribute> attributes)
            : this(attributes, new AuthorizationBehavior[0])
        {
        }

        public DefaultAuthorizationRule(IEnumerable<AuthorizationAttribute> attributes, IEnumerable<AuthorizationBehavior> behaviors)
        {
            if (attributes == null)
            {
                throw new ArgumentNullException("attributes");
            }
            if (attributes.Any(a => a == null))
            {
                throw new ArgumentException("attributes cannot contain null values", "attributes");
            }
            if (behaviors == null)
            {
                throw new ArgumentNullException("behaviors");
            }
            if (behaviors.Any(b => b == null))
            {
                throw new ArgumentException("behaviors cannot contain null values", "behaviors");
            }
            this._attributes = attributes.ToArray();
            this._behaviors = behaviors.ToArray();
        }

        public override IEnumerable<AuthorizationAttribute> GetAuthorizationAttributes(object target)
        {
            return this._attributes;
        }

        public override IEnumerable<AuthorizationBehavior> GetAuthorizationBehaviors(object target)
        {
            return this._behaviors;
        }
    }
}
