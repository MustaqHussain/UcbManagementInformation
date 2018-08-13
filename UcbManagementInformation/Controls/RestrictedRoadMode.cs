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

namespace UcbManagementInformation.Maps
{
    public class RestrictedRoadMode : Microsoft.Maps.MapControl.Core.MercatorMode
    {
        // Fields
        private Range<double> validLatitudeRange = new Range<double>(49.8, 56.2);
        private Range<double> validLongitudeRange = new Range<double>(-5.8, 1.8);
        private Range<double> zoomRange = new Range<double>(6, 14);

        private double zoomLevelCurrent;

        public event ChangedEventHandler OnZoomLevelChanged;

        public double Resolution = 0;

        public RestrictedRoadMode()
        { }

        private double RangeOf(Range<double> rangeItem)
        {
            return Math.Abs((rangeItem.To - rangeItem.From));
        }

        
        // This method is called when the map view changes on Keyboard 
        // and Navigation Bar events.
        public override bool ConstrainView(Location center, ref double zoomLevel, ref double heading, ref double pitch)
        {
            bool isChanged = base.ConstrainView(center, ref zoomLevel, ref heading, ref pitch);

            double newLatitude = center.Latitude;
            double newLongitude = center.Longitude;

            // If the map view is outside the valid longitude range,
            // move the map back within range.
            if (center.Longitude > validLongitudeRange.To)
            {
                newLongitude = validLongitudeRange.To;
            }
            else if (center.Longitude < validLongitudeRange.From)
            {
                newLongitude = validLongitudeRange.From;
            }

            // If the map view is outside the valid latitude range,
            // move the map back within range.
            if (center.Latitude > validLatitudeRange.To)
            {
                newLatitude = validLatitudeRange.To;
            }
            else if (center.Latitude < validLatitudeRange.From)
            {
                newLatitude = validLatitudeRange.From;
            }

            // The new map view location.
            if (newLatitude != center.Latitude || newLongitude != center.Longitude)
            {
                center.Latitude = newLatitude;
                center.Longitude = newLongitude;
                isChanged = true;
            }

            // The new zoom level.
            Range<double> range = GetZoomRange(center);
            if (zoomLevel > range.To)
            {
                zoomLevel = range.To;
                isChanged = true;
            }
            else if (zoomLevel < range.From)
            {
                zoomLevel = range.From;
                isChanged = true;
            }
            double scale = this.Scale;
            
            

            return isChanged;
        }

       



        protected override Range<double> GetZoomRange(Location center)
        {
            return this.zoomRange;
        }

        public override void OnMouseWheel(MapMouseWheelEventArgs e)
        {
            if ((e.WheelDelta > 0.0) && (this.ZoomLevel >= this.zoomRange.To))
            {
                e.Handled = true;
            }
            else if ((e.WheelDelta < 0.0) && (this.ZoomLevel <= this.zoomRange.From))
            {
                e.Handled = true;
            }
            else
            {
                base.OnMouseWheel(e);
            }
        }
        public delegate void ChangedEventHandler(object sender, EventArgs e);

    }
}
