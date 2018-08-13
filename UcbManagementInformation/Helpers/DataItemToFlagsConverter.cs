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
using UcbManagementInformation.Server.DataAccess;
using System.Text;

namespace UcbManagementInformation
{
    public class DataItemToFlagsConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            DataItem valueAsDataItem = (DataItem)value;
            if (valueAsDataItem != null)
            {
                StringBuilder Flags = new StringBuilder();
                if (valueAsDataItem.IsCommonTableGrouping)
                {
                    Flags.Append("U");
                }
                if (valueAsDataItem.IsGroup)
                {
                    Flags.Append("G");
                }
                if (valueAsDataItem.IsSummable)
                {
                    Flags.Append("S");
                }
                if (!valueAsDataItem.IsCommonTableGrouping && !valueAsDataItem.IsGroup && !valueAsDataItem.IsSummable)
                {
                    Flags.Append("F");
                }
                return Flags.ToString();
            }
            else
            {
                return null;

            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
