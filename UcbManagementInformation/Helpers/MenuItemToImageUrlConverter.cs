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
using System.Windows.Media.Imaging;
using UcbManagementInformation.Models;

namespace UcbManagementInformation
{
    public class MenuItemToImageUrlConverter : System.Windows.Data.IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            MainMenuItem item = value as MainMenuItem;
            if (item != null)
            {
                return (new BitmapImage(new Uri(item.ImageUrl,UriKind.Relative)));
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
