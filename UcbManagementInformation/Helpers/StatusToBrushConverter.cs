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
using UcbManagementInformation.Models;
using UcbManagementInformation.Web.MIFileUpload.JobQueue;

namespace UcbManagementInformation
{
    public class StatusToBrushConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            JobStepStatus status = (JobStepStatus)value;
            switch (status)
            {
                case JobStepStatus.NotStarted:
                    return new SolidColorBrush(Colors.White);
                    break;
                case JobStepStatus.Running:
                    return new SolidColorBrush(Color.FromArgb(255,255,255,100));
                    break;
                case JobStepStatus.Succeeded:
                    return new SolidColorBrush(Color.FromArgb(255,100,255,100));
                    break;
                case JobStepStatus.Failed:
                    return new SolidColorBrush(Color.FromArgb(255, 255, 100, 100));
                    break;
            }
            return null;
        }
        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
