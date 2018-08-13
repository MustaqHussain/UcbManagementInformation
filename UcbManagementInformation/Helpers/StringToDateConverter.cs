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
using System.Globalization;

namespace UcbManagementInformation
{
    public class StringToDateConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value != null)
            {
                IFormatProvider dateFormat = new CultureInfo("en-GB");
                try
                {
                    DateTime date = DateTime.Parse(value.ToString(), dateFormat);
                    return date;
                }
                catch (FormatException ex)
                {
                    //if not a date return the string supplied.
                    return value;
                }
            }
            else
            {
                return string.Empty;
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            try
            {
                IFormatProvider dateFormat = new CultureInfo("en-GB");
                
                return DateTime.Parse(value.ToString(),dateFormat).ToShortDateString();
            }
            catch (FormatException ex)
            {
                //if not a date return the string supplied.
                return value;
            }
        }
    }
}
