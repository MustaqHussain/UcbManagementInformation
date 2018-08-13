using System;
using System.Collections.Generic;
using System.Windows;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    internal class AuthorizationBehaviorManager
    {
        private readonly DependencyObject _target;
        private IEnumerable<AuthorizationBehavior> _behaviors;

        public AuthorizationBehaviorManager(DependencyObject target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            this._target = target;
        }

        public IEnumerable<AuthorizationBehavior> GetBehaviors()
        {
            return this._behaviors;
        }

        public void SetBehaviors(IEnumerable<AuthorizationBehavior> behaviors, AuthorizationSource source)
        {
            if (this._behaviors != behaviors)
            {
                if (this._behaviors != null)
                {
                    foreach (AuthorizationBehavior behavior in this._behaviors)
                    {
                        behavior.RemoveBehavior(this._target);
                    }
                }

                this._behaviors = behaviors;

                if (this._behaviors != null)
                {
                    foreach (AuthorizationBehavior behavior in this._behaviors)
                    {
                        behavior.AddBehavior(this._target, source);
                    }
                }
            }
        }
    }
}
