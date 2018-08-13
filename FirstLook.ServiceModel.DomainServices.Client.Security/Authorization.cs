using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Windows;
using System.Windows.Controls;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public static class Authorization
    {
        #region RequiresAuthentication

        /// <summary>
        /// The <see cref="DependencyProperty"/> for <c>RequiresAuthentication</c>.
        /// </summary>
        public static readonly DependencyProperty RequiresAuthenticationProperty =
            DependencyProperty.RegisterAttached(
                "RequiresAuthentication",
                typeof(bool),
                typeof(Authorization),
                new PropertyMetadata(false, Authorization.RequiresAuthenticationPropertyChanged));

        private static void RequiresAuthenticationPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Authorization.Authorize(sender);
        }

        public static bool GetRequiresAuthentication(DependencyObject target)
        {
            return (bool)target.GetValue(Authorization.RequiresAuthenticationProperty);
        }

        public static void SetRequiresAuthentication(DependencyObject target, bool value)
        {
            target.SetValue(Authorization.RequiresAuthenticationProperty, value);
        }

        #endregion

        #region RequiresRole

        /// <summary>
        /// The <see cref="DependencyProperty"/> for <c>RequiresRole</c>.
        /// </summary>
        public static readonly DependencyProperty RequiresRoleProperty =
            DependencyProperty.RegisterAttached(
                "RequiresRole",
                typeof(IEnumerable<string>),
                typeof(Authorization),
                new PropertyMetadata(new string[0], Authorization.RequiresRolePropertyChanged));

        private static void RequiresRolePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Authorization.Authorize(sender);
        }

        [TypeConverter(typeof(StringArrayConverter))]
        public static IEnumerable<string> GetRequiresRole(DependencyObject target)
        {
            return (IEnumerable<string>)target.GetValue(Authorization.RequiresRoleProperty);
        }

        public static void SetRequiresRole(DependencyObject target, IEnumerable<string> value)
        {
            target.SetValue(Authorization.RequiresRoleProperty, value);
        }

        #endregion

        #region Rule

        /// <summary>
        /// The <see cref="DependencyProperty"/> for <c>Rule</c>.
        /// </summary>
        public static readonly DependencyProperty RuleProperty =
            DependencyProperty.RegisterAttached(
                "Rule",
                typeof(AuthorizationRule),
                typeof(Authorization),
                new PropertyMetadata(null, Authorization.RulePropertyChanged));

        private static void RulePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Authorization.Authorize(sender);
        }

        public static AuthorizationRule GetRule(DependencyObject target)
        {
            return (AuthorizationRule)target.GetValue(Authorization.RuleProperty);
        }

        public static void SetRule(DependencyObject target, AuthorizationRule value)
        {
            target.SetValue(Authorization.RuleProperty, value);
        }

        #endregion

        #region TargetProperties

        /// <summary>
        /// The <see cref="DependencyProperty"/> for <c>TargetProperties</c>.
        /// </summary>
        public static readonly DependencyProperty TargetPropertiesProperty =
            DependencyProperty.RegisterAttached(
                "TargetProperties",
                typeof(IEnumerable<string>),
                typeof(Authorization),
                new PropertyMetadata(new string[0], Authorization.TargetPropertiesPropertyChanged));

        private static void TargetPropertiesPropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            Authorization.Authorize(sender);
        }

        [TypeConverter(typeof(StringArrayConverter))]
        public static IEnumerable<string> GetTargetProperties(DependencyObject target)
        {
            return (IEnumerable<string>)target.GetValue(Authorization.TargetPropertiesProperty);
        }

        public static void SetTargetProperties(DependencyObject target, IEnumerable<string> value)
        {
            target.SetValue(Authorization.TargetPropertiesProperty, value);
        }

        #endregion

        #region BehaviorManager

        /// <summary>
        /// The <see cref="DependencyProperty"/> for <c>BehaviorManager</c>.
        /// </summary>
        private static readonly DependencyProperty BehaviorManagerProperty =
            DependencyProperty.RegisterAttached(
                "BehaviorManager",
                typeof(AuthorizationBehaviorManager),
                typeof(Authorization),
                new PropertyMetadata(null));

        private static AuthorizationBehaviorManager GetBehaviorManager(DependencyObject target)
        {
            return (AuthorizationBehaviorManager)target.GetValue(Authorization.BehaviorManagerProperty);
        }

        private static void SetBehaviorManager(DependencyObject target, AuthorizationBehaviorManager value)
        {
            target.SetValue(Authorization.BehaviorManagerProperty, value);
        }

        #endregion

        #region NavigationMode

        /// <summary>
        /// The <see cref="DependencyProperty"/> for <c>NavigationMode</c>.
        /// </summary>
        public static readonly DependencyProperty NavigationModeProperty =
            DependencyProperty.RegisterAttached(
                "NavigationMode",
                typeof(AuthorizationNavigationMode),
                typeof(Authorization),
                new PropertyMetadata(AuthorizationNavigationMode.None, Authorization.NavigationModePropertyChanged));

        private static void NavigationModePropertyChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (DesignerProperties.IsInDesignTool)
            {
                return;
            }

            Frame frame = sender as Frame;
            if (frame != null)
            {
                if (Authorization.GetNavigationManager(frame) == null)
                {
                    Authorization.SetNavigationManager(frame,
                        new AuthorizationNavigationManager(frame, (AuthorizationNavigationMode)e.NewValue));
                }
                else
                {
                    Authorization.GetNavigationManager(frame).NavigationMode = (AuthorizationNavigationMode)e.NewValue;
                }
            }
        }

        public static AuthorizationNavigationMode GetNavigationMode(DependencyObject target)
        {
            return (AuthorizationNavigationMode)target.GetValue(Authorization.NavigationModeProperty);
        }

        public static void SetNavigationMode(DependencyObject target, AuthorizationNavigationMode value)
        {
            target.SetValue(Authorization.NavigationModeProperty, value);
        }

        #endregion

        #region NavigationManager

        /// <summary>
        /// The <see cref="DependencyProperty"/> for <c>NavigationManager</c>.
        /// </summary>
        private static readonly DependencyProperty NavigationManagerProperty =
            DependencyProperty.RegisterAttached(
                "NavigationManager",
                typeof(AuthorizationNavigationManager),
                typeof(Authorization),
                new PropertyMetadata(null));

        private static AuthorizationNavigationManager GetNavigationManager(DependencyObject target)
        {
            return (AuthorizationNavigationManager)target.GetValue(Authorization.NavigationManagerProperty);
        }

        private static void SetNavigationManager(DependencyObject target, AuthorizationNavigationManager value)
        {
            target.SetValue(Authorization.NavigationManagerProperty, value);
        }

        #endregion

        private static AuthorizationPrompter _prompter;
        private static AuthorizationSourceFactory _sourceFactory = new MarkupSourceFactory();

        public static AuthorizationPrompter Prompter
        {
            get { return Authorization._prompter; }
            set { Authorization._prompter = value; }
        }

        public static AuthorizationSourceFactory SourceFactory
        {
            get { return Authorization._sourceFactory; }
            set { Authorization._sourceFactory = value; }
        }

        public static AuthorizationResult Authorize(object target)
        {
            if (target == null)
            {
                throw new ArgumentNullException("target");
            }

            if (DesignerProperties.IsInDesignTool)
            {
                return AuthorizationResult.Allowed;
            }

            AuthorizationSource source =
                Authorization.SourceFactory.CreateSource(AuthorizationRuleManager.GetAuthorizationAttributes(target));
            if (source == null)
            {
                throw new InvalidOperationException("Factory cannot return a null source.");
            }

            DependencyObject targetDo = target as DependencyObject;
            if (targetDo != null)
            {
                if (Authorization.GetBehaviorManager(targetDo) == null)
                {
                    Authorization.SetBehaviorManager(targetDo, new AuthorizationBehaviorManager(targetDo));
                }
                Authorization.GetBehaviorManager(targetDo).SetBehaviors(AuthorizationRuleManager.GetAuthorizationBehaviors(target), source);
            }

            return source.Result;
        }

        public static AuthorizationResult Authorize(object target, AuthorizationRule rule)
        {
            AuthorizationRuleManager.AddAuthorizationRule(target, rule);
            return Authorization.Authorize(target);
        }

        public static AuthorizationResult Authorize(object target, AuthorizationAttribute attribute)
        {
            return Authorization.Authorize(target, new DefaultAuthorizationRule(attribute));
        }

        public static AuthorizationResult Authorize(object target, IEnumerable<AuthorizationAttribute> attributes)
        {
            return Authorization.Authorize(target, new DefaultAuthorizationRule(attributes));
        }

        #region Nested Classes

        private class StringArrayConverter : TypeConverter
        {
            public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
            {
                return (sourceType == typeof(string));
            }

            public override object ConvertFrom(ITypeDescriptorContext context, System.Globalization.CultureInfo culture, object value)
            {
                string valueString = value as string;
                return string.IsNullOrEmpty(valueString) ?
                    new string[0] :
                    valueString.Split(',').Select(s => s.Trim()).ToArray();
            }
        }

        private class MarkupSourceFactory : AuthorizationSourceFactory
        {
            public override AuthorizationSource CreateSource(IEnumerable<AuthorizationAttribute> attributes)
            {
                return new AuthorizationSource(attributes);
            }
        }

        #endregion
    }
}
