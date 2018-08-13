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
    public class DaysToBrushConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            int numberOfDaysSinceLoad = (int)value;
            if (numberOfDaysSinceLoad == -1 || numberOfDaysSinceLoad > 100)
            {
                return new SolidColorBrush(Color.FromArgb(255, 255, 100, 100));    // 255,100,100 Red
                                    // 255,255,100 Yellow
                                    // 100,255,100 Green
            }
            else
            {
                if (numberOfDaysSinceLoad > 50)
                {
                    int redColour = System.Convert.ToInt32(System.Math.Abs(1.55 * 2 * (numberOfDaysSinceLoad-50)));
                    int negativeRefColour = 255 - redColour;

                    return new SolidColorBrush(Color.FromArgb(255, 255, (Byte)negativeRefColour, 100));
                }
                else
                {
                    int RedColour = System.Convert.ToInt32(System.Math.Abs(1.55 * 2 * numberOfDaysSinceLoad));
                    return new SolidColorBrush(Color.FromArgb(255, (Byte)(RedColour + 100),255, 0));
                }
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
