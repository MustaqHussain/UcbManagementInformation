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
    public class BoolToOpacityConverter : IValueConverter
    {

        object IValueConverter.Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            bool valueAsBool = (bool)value;
            if (parameter != null)
            {
                bool invert = bool.Parse(parameter.ToString());
                if (invert)
                {
                    valueAsBool = !valueAsBool;
                }
            }
            return valueAsBool ? 1 : 0.5;
        }

        object IValueConverter.ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueAsString = (string)value;
            bool ReturnValueIfVisible = true;
            if (parameter != null)
            {
                bool invert = bool.Parse(parameter.ToString());
                if (invert)
                {
                    ReturnValueIfVisible = !ReturnValueIfVisible;
                }
            }
            return valueAsString == "1" ? ReturnValueIfVisible : !ReturnValueIfVisible;
        }
    }
}
