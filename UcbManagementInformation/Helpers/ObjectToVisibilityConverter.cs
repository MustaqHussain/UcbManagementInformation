using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Data;

namespace UcbManagementInformation
{
    public class ObjectToVisibilityConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            
            bool valueAsBool = true;
            if (value == null)
            {
                valueAsBool = false;
            }
            else
            {
                try
                {
                    if (Convert.ToDecimal(value) == 0)
                    {
                        valueAsBool = false;
                    }
                }
                catch (FormatException)
                {
                }
            }
            return valueAsBool ? "Visible" : "Collapsed";
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueAsString = (string)value;
            return valueAsString == "Visible" ? true : false;
        }
    }
}
