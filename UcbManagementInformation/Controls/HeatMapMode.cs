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
    public class HeatMapMode : AerialMode
    {
        // Fields
        private Range<double> validLatitudeRange = new Range<double>(53, 53);
        private Range<double> validLongitudeRange = new Range<double>(-2, -2);
        private Range<double> zoomRange = new Range<double>(6.5, 6.5);

        public Range<double> ValidLatitudeRange
        {
            get { return validLatitudeRange; }
            set { validLatitudeRange = value; }
        }
        public Range<double> ValidLongitudeRange
        {
            get { return validLongitudeRange; }
            set { validLongitudeRange = value; }
        }
        public Range<double> ZoomRange
        {
            get { return zoomRange; }
            set { zoomRange = value; }
        }
        private double zoomLevelCurrent;

        public event ChangedEventHandler OnZoomLevelChanged;

        public double Resolution = 0;

        public HeatMapMode(double latitude, double longitude, double zoomLevel)
        {
            ValidLatitudeRange = new Range<double>(latitude, latitude);
            ValidLongitudeRange = new Range<double>(longitude, longitude);
            ZoomRange = new Range<double>(zoomLevel, zoomLevel);
        }
        public HeatMapMode()
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
