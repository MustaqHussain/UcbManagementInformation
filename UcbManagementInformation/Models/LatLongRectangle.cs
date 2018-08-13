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
    public class LatLongRectangle
    {
        public LatLong Northeast { get; set; }
        public LatLong Southeast { get; set; }
        public LatLong Northwest { get; set; }
        public LatLong Southwest { get; set; }

    }
}
