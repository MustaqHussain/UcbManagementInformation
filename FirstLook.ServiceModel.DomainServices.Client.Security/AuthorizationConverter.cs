using System;
using System.ComponentModel.DataAnnotations;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace FirstLook.ServiceModel.DomainServices.Client.Security
{
    public class AuthorizationConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            AuthorizationResult result = (AuthorizationResult)value;

            if (targetType == typeof(AuthorizationResult))
            {
                return result;
            }
            if (targetType == typeof(object))
            {
                return result;
            }
            if (targetType == typeof(string))
            {
                return (result == AuthorizationResult.Allowed) ? "Allowed" : result.ErrorMessage;
            }
            if (targetType == typeof(bool))
            {
                return (result == AuthorizationResult.Allowed);
            }
            if (targetType == typeof(Visibility))
            {
                return (result == AuthorizationResult.Allowed) ? Visibility.Visible : Visibility.Collapsed;
            }

            throw new NotSupportedException(string.Format(
                culture,
                "An AuthorizationResult cannot be converted to the type {0}.",
                targetType));
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotSupportedException();
        }
    }
}
