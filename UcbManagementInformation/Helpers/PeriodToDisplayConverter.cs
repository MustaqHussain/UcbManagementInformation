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
    public class PeriodToDisplayConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            //if(value.ToString() == "All")
            //{
            //    return null;
            //}
            return value;
        }

        public static DateTime ConvertPeriodToDate(string period, bool endOfMonth)
        {
            DateTime FormattedDate = new DateTime();
            if (period != null && period != "")
            {
                // Set the culture to GB English
                IFormatProvider FormatProvider = new System.Globalization.CultureInfo("en-GB");

                //Convert date to string using specified English datetime format
                try
                {
                    string DateToConvert = "01/" + period.Substring(4, 2) + "/" + period.Substring(0, 4);
                    FormattedDate = DateTime.Parse(DateToConvert, FormatProvider);

                    if (endOfMonth)
                    {
                        //add a month and subtract a day to get the last day of the month
                        FormattedDate = (FormattedDate.AddMonths(1)).AddDays(-1);
                    }
                }
                catch (ArgumentOutOfRangeException)
                {
                    // Invalid date entered by user
                }
                catch (FormatException)
                {
                    // Invalid date entered by user
                }
                catch (ArgumentNullException)
                {
                    // null date entered by user
                }
            }
            return FormattedDate;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (null == value ) return null;
            if (value == "All") return "All";

            string ReturnValue = "";

            //Convert period from yyyyMM to MMM yyyyy
            DateTime PeriodDate = ConvertPeriodToDate(value.ToString(), false);
            ReturnValue = PeriodDate.ToString("MMM") + " " + PeriodDate.ToString("yyyy");

            return ReturnValue;
            //throw new NotSupportedException();  
        }

        
    }
}
