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
    public class ImageUriToNavigationUriConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            string imageUriAsString = (string)value;
            if (imageUriAsString != null)
            {
                switch (imageUriAsString)
                {
                    case "Assets/Images/home.PNG":
                        return "/Home";
                    case "Assets/Images/report.PNG":
                        return "/Reporting/ReportWizard/ReportWizard";
                    case "Assets/Images/INES.PNG":
                        return "/INES/Charts/SelectionPanel";
                    case "Assets/Images/costanalysis.PNG":
                        return "/INES/Charts/AdminCostAnalysis";
                        
                    case "Assets/Images/maps.PNG":
                        return "/Geographic/Maps";
                        
                    case "Assets/Images/heatmap.PNG":
                        return "/Geographic/HeatMap";
                        
                    case "Assets/Images/fileupload.PNG":
                        return "/Home";
                        
                    default:
                        return null;
                        
                }
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException("Convert Back not implemented");
            /*
            string navigationUriAsString = (string)value;
            if (navigationUriAsString != null)
            {
                switch (navigationUriAsString)
                {
                    case "":
                        break;
                }
            }
            else
            {
                return null;
            }*/
        }
    }
}
