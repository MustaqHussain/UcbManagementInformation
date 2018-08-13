using System;
using System.Reflection;
using System.Windows;
using System.Windows.Data;
using System.Globalization;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    internal class PropertyBindingBehavior : AuthorizationBehavior
    {
        private const string DependencyPropertySuffix = "Property";

        private readonly string _propertyName;

        public PropertyBindingBehavior(string propertyName)
        {
            if (string.IsNullOrEmpty(propertyName))
            {
                throw new ArgumentNullException("propertyName");
            }
            this._propertyName = propertyName;
        }

        private static DependencyProperty GetDependencyProperty(object target, string propertyName)
        {
            string dependencyPropertyName = propertyName + PropertyBindingBehavior.DependencyPropertySuffix;

            FieldInfo field = target.GetType().GetField(
                dependencyPropertyName,
                BindingFlags.Public | BindingFlags.Static | BindingFlags.FlattenHierarchy);

            if ((field == null) || (field.FieldType != typeof(DependencyProperty)))
            {
                throw new InvalidOperationException(string.Format(
                    CultureInfo.CurrentCulture,
                    "There is no dependency property with the name '{0}' on the target of type '{1}' to bind to.",
                    dependencyPropertyName,
                    target.GetType()));
            }

            return (DependencyProperty)field.GetValue(null);
        }

        public string PropertyName
        {
            get { return this._propertyName; }
        }

        public override void AddBehavior(object target, AuthorizationSource source)
        {
            DependencyObject targetDo = target as DependencyObject;
            if (targetDo != null)
            {
                BindingOperations.SetBinding(
                    targetDo,
                    PropertyBindingBehavior.GetDependencyProperty(target, this.PropertyName),
                    new Binding("Result") { Source = source, Converter = new AuthorizationConverter() });
            }
        }

        public override void RemoveBehavior(object target)
        {
            DependencyObject targetDo = target as DependencyObject;
            if (targetDo != null)
            {
                targetDo.SetValue(
                    PropertyBindingBehavior.GetDependencyProperty(target, this.PropertyName),
                    DependencyProperty.UnsetValue);
            }
        }
    }
}
