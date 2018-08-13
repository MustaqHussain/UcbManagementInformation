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
using System.Windows.Media.Imaging;
using System.IO;

namespace UcbManagementInformation
{
    public class StringToImageConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string valueAsString = (string)value;
            switch (valueAsString)
            {
                case "Red":
                    return RedFolder;
                case "Yellow":
                    return GreenFolder;
                case "Gray":
                default:
                    return GrayFolder;
            }

          
        }
      
        static BitmapImage RedFolder = new BitmapImage(new Uri("/UcbManagementInformation;component/Assets/Images/redfolder.PNG", UriKind.Relative));
        static BitmapImage GreenFolder = new BitmapImage(new Uri("/UcbManagementInformation;component/Assets/Images/greenfolder.PNG", UriKind.Relative));
        static BitmapImage GrayFolder = new BitmapImage(new Uri("/UcbManagementInformation;component/Assets/Images/grayfolder.PNG", UriKind.Relative));
        //static Uri RedFolder = new Uri(@"/Assets/Images/redfolder.PNG", UriKind.Relative);
        //static Uri GreenFolder = new Uri(@"/Assets/Images/greenfolder.PNG", UriKind.Relative);
        //static Uri GrayFolder = new Uri(@"/Assets/Images/grayfolder.PNG", UriKind.Relative);
        //static string RedFolder = @"Assets/Images/redfolder.PNG";
        //static string GreenFolder = @"Assets/Images/greenfolder.PNG";
        //static string GrayFolder = @"Assets/Images/grayfolder.PNG";
        
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
