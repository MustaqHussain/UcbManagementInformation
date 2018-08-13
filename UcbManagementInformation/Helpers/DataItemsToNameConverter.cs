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
using System.Collections.ObjectModel;

namespace UcbManagementInformation
{
    public class DataItemsToNameConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            ObservableCollection<DataItem> valueAsDataItem = (ObservableCollection<DataItem>)value;
            if (valueAsDataItem != null && valueAsDataItem.Count != 0)
            {
                return valueAsDataItem[0].Name;
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
