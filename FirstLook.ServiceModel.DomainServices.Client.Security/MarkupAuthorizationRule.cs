using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.ServiceModel.DomainServices.Server;
using System.Windows;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    internal class MarkupAuthorizationRule : AuthorizationRule
    {
        private readonly string _defaultTarget;

        public MarkupAuthorizationRule() : this(string.Empty)
        {
        }

        public MarkupAuthorizationRule(string defaultTarget)
        {
            this._defaultTarget = defaultTarget;
        }

        public string DefaultTarget
        {
            get { return this._defaultTarget; }
        }

        public override IEnumerable<AuthorizationAttribute> GetAuthorizationAttributes(object target)
        {
            List<AuthorizationAttribute> attributes = new List<AuthorizationAttribute>();

            // Custom Attributes
            attributes.AddRange(target.GetType().GetCustomAttributes(true).OfType<AuthorizationAttribute>());

            DependencyObject targetDo = target as DependencyObject;
            if (targetDo != null)
            {
                // Requires Authentication
                if (Authorization.GetRequiresAuthentication(targetDo))
                {
                    attributes.Add(new RequiresAuthenticationAttribute());
                }

                // Requires Roles
                if ((Authorization.GetRequiresRole(targetDo) != null) && Authorization.GetRequiresRole(targetDo).Any())
                {
                    attributes.Add(new RequiresRoleAttribute(Authorization.GetRequiresRole(targetDo).ToArray()));
                }

                // Custom Rule
                if (Authorization.GetRule(targetDo) != null)
                {
                    attributes.AddRange(Authorization.GetRule(targetDo).GetAuthorizationAttributes(target));
                }
            }

            return attributes;
        }

        public override IEnumerable<AuthorizationBehavior> GetAuthorizationBehaviors(object target)
        {
            List<AuthorizationBehavior> behaviors = new List<AuthorizationBehavior>();

            DependencyObject targetDo = target as DependencyObject;
            if (targetDo != null)
            {
                if (Authorization.GetTargetProperties(targetDo) != null)
                {
                    behaviors.AddRange(
                        Authorization.GetTargetProperties(targetDo).Select(
                            propertyName => (AuthorizationBehavior) new PropertyBindingBehavior(propertyName)));
                }
            }

            if ((behaviors.Count == 0) && !string.IsNullOrEmpty(this.DefaultTarget))
            {
                behaviors.Add(new PropertyBindingBehavior(this.DefaultTarget));
            }

            return behaviors;
        }
    }
}
