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
    public class IntToFillColorConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int ValueAsInt = (int)value; 
            double red = 255;
            double green = 255 - (ValueAsInt * 2.55);
            double blue = 255 - (ValueAsInt * 2.55);
            var color = Color.FromArgb(255, (byte)red, (byte)green, (byte)blue);

            return new SolidColorBrush(color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
