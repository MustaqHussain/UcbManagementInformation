using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public static class AuthorizationRuleManager
    {
        private static readonly object SyncLock = new object();
        private static readonly IDictionary<WeakReference, IList<AuthorizationRule>> Rules =
            new Dictionary<WeakReference, IList<AuthorizationRule>>(new WeakReferenceEqualityComparer());

        private static bool _useRollingInitialization;

        static AuthorizationRuleManager()
        {
            AuthorizationRuleManager._useRollingInitialization = false;

            AuthorizationRuleManager.AddAuthorizationRule(typeof(object), new MarkupAuthorizationRule());
            AuthorizationRuleManager.AddAuthorizationRule(typeof(UIElement), new MarkupAuthorizationRule("Visibility"));
            AuthorizationRuleManager.AddAuthorizationRule(typeof(Page), new MarkupAuthorizationRule("IsEnabled"));

            AuthorizationRuleManager._useRollingInitialization = true;
        }

        public static IEnumerable<AuthorizationAttribute> GetAuthorizationAttributes(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            List<AuthorizationAttribute> attributes = new List<AuthorizationAttribute>();
            foreach (AuthorizationRule rule in AuthorizationRuleManager.GetAuthorizationRules(target))
            {
                attributes.AddRange(rule.GetAuthorizationAttributes(target));
            }

            return attributes;
        }

        public static IEnumerable<AuthorizationBehavior> GetAuthorizationBehaviors(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            List<AuthorizationBehavior> binders = new List<AuthorizationBehavior>();
            foreach (AuthorizationRule rule in AuthorizationRuleManager.GetAuthorizationRules(target))
            {
                binders.AddRange(rule.GetAuthorizationBehaviors(target));
            }

            return binders;
        }

        public static IEnumerable<AuthorizationRule> GetAuthorizationRules(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            return AuthorizationRuleManager.GetRules(target).ToArray();
        }

        public static void AddAuthorizationRule(object target, AuthorizationRule rule)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            AuthorizationRuleManager.GetRules(target).Add(rule);
        }

        public static bool RemoveAuthorizationRule(object target, AuthorizationRule rule)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            if (rule == null)
            {
                throw new ArgumentNullException("rule");
            }
            return AuthorizationRuleManager.GetRules(target).Remove(rule);
        }

        public static void ClearAuthorizationRules(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }
            AuthorizationRuleManager.GetRules(target).Clear();
        }

        public static void PurgeOrphanedRules()
        {
            foreach (WeakReference reference in AuthorizationRuleManager.Rules.Keys.ToArray())
            {
                if (!reference.IsAlive)
                {
                    AuthorizationRuleManager.Rules.Remove(reference);
                }
            }
        }

        private static IList<AuthorizationRule> GetRules(object target)
        {
            WeakReference reference = new WeakReference(target);
            if (!AuthorizationRuleManager.Rules.ContainsKey(reference))
            {
                List<AuthorizationRule> rules = new List<AuthorizationRule>();
                AuthorizationRuleManager.InitializeRules(target, rules);

                lock (AuthorizationRuleManager.SyncLock)
                {
                    if (!AuthorizationRuleManager.Rules.ContainsKey(reference))
                    {
                        AuthorizationRuleManager.Rules[reference] = rules;
                    }
                }
            }
            return AuthorizationRuleManager.Rules[reference];
        }

        private static void InitializeRules(object target, List<AuthorizationRule> rules)
        {
            if (!AuthorizationRuleManager._useRollingInitialization)
            {
                return;
            }

            Type targetType = (target as Type) ?? target.GetType();
            if (targetType.BaseType == null)
            {
                return;
            }

            List<Type> types = new List<Type>();
            if (target is Type)
            {
                types.Add(targetType.BaseType);
                types.AddRange(targetType.GetInterfaces());
            }
            else
            {
                types.Add(targetType);
            }

            foreach (IList<AuthorizationRule> typeRules in types.Select(AuthorizationRuleManager.GetRules))
            {
                rules.AddRange(typeRules);
            }
        }

        #region Nested Classes

        private class WeakReferenceEqualityComparer : IEqualityComparer<WeakReference>
        {
            /// <summary>
            /// Determines whether the specified objects are equal.
            /// </summary>
            /// <returns>
            /// true if the specified objects are equal; otherwise, false.
            /// </returns>
            /// <param name="x">The first object of type <see cref="WeakReference"/> to compare.</param>
            /// <param name="y">The second object of type <see cref="WeakReference"/> to compare.</param>
            public bool Equals(WeakReference x, WeakReference y)
            {
                if (x == null)
                {
                    return (y == null);
                }
                return (x.IsAlive == y.IsAlive) && (x.IsAlive ? (x.Target == y.Target) : true);
            }

            /// <summary>
            /// Returns a hash code for the specified object.
            /// </summary>
            /// <returns>
            /// A hash code for the specified object.
            /// </returns>
            /// <param name="obj">The <see cref="WeakReference"/> for which a hash code is to be returned.</param>
            /// <exception cref="T:System.ArgumentNullException"> is thrown when <paramref name="obj"/> is null.</exception>
            public int GetHashCode(WeakReference obj)
            {
                if (obj == null)
                {
                    throw new ArgumentNullException("obj");
                }

                int hash = 23;
                hash = hash * 37 + (obj.IsAlive ? 1 : 0);
                hash = hash * 37 + (obj.IsAlive ? obj.Target.GetHashCode() : 0);
                return hash;
            }
        }

        #endregion
    }
}
