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
using Microsoft.Maps.MapControl;
using System.Windows.Data;
using UcbManagementInformation.Models;

namespace UcbManagementInformation
{
    public class LocationToLatLongConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            LatLong valueAsLatLong = (LatLong)value;
            if (valueAsLatLong != null && valueAsLatLong.Latitude != null && valueAsLatLong.Longitude != null)
            {
                return new Location(valueAsLatLong.Latitude, valueAsLatLong.Longitude);
            }
            else
            {
                return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            Location valueAsLocation = (Location)value;
            if (valueAsLocation !=null && valueAsLocation.Latitude != null && valueAsLocation.Longitude != null)
            {
                return new LatLong(valueAsLocation.Latitude, valueAsLocation.Longitude);
            }
            else
            {
                return null;
            }
        }
    }
}
