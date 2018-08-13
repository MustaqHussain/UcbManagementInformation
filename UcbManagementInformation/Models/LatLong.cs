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

namespace UcbManagementInformation.Models
{
    public class LatLong
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
       
        public LatLong(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            
        }
        public override bool Equals(object obj)
        {
            return this.Latitude == ((LatLong)obj).Latitude && this.Longitude == ((LatLong)obj).Latitude;
        }
        
    }
}
