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
using System.Collections.ObjectModel;
using Microsoft.Maps.MapControl;

namespace UcbManagementInformation.Models
{
    public class BingPin
    {
        public double Latitude { get; set; }
        public double Longitude { get; set; }
        public Location PinLocation { get; set; }
        
        public BingPin(double latitude, double longitude)
        {
            this.Latitude = latitude;
            this.Longitude = longitude;
            this.PinLocation = new Location(Latitude, Longitude);
            
        }

    }

    

}
