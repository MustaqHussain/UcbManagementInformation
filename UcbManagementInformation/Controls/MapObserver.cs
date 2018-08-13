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

namespace UcbManagementInformation.Controls
{
    public class MapObserver
    {

        #region " Observe "

        public static bool GetObserve(Map elem)
        {
            return (bool)elem.GetValue(ObserveProperty);
        }

        public static void SetObserve(
          Map elem, bool value)
        {
            elem.SetValue(ObserveProperty, value);
        }

        public static readonly DependencyProperty ObserveProperty =
            DependencyProperty.RegisterAttached("Observe", typeof(bool), typeof(MapObserver),
            new PropertyMetadata(false, OnObserveChanged));

        static void OnObserveChanged(
          DependencyObject depObj, DependencyPropertyChangedEventArgs e)
        {
            Map elem = depObj as Map;
            if (elem == null)
                return;

            if (e.NewValue is bool == false)
                return;

            if ((bool)e.NewValue)
                elem.ViewChangeEnd += elem_ViewChangeEnd;
            else
                elem.ViewChangeEnd -= elem_ViewChangeEnd;
        }

        static void elem_ViewChangeEnd(object sender, MapEventArgs e)
        {
            //if (!Object.ReferenceEquals(sender, e.OriginalSource))
            //    return;

            //FrameworkElement elem = e.OriginalSource as FrameworkElement;
            if (sender != null)
            {
                SetObservedBoundingRectangle(sender as Map, ((Map)sender).BoundingRectangle);
                Location CrossLocation = ((Map)sender).Mode.ViewportPointToLocation(new Point(425, 325));
                Location InnerRectLocationNW = ((Map)sender).Mode.ViewportPointToLocation(new Point(150, 50));
                Location InnerRectLocationSE = ((Map)sender).Mode.ViewportPointToLocation(new Point(550, 550));
                LocationRect rect = new LocationRect(InnerRectLocationNW, InnerRectLocationSE);
                Location RadiusLocation = ((Map)sender).Mode.ViewportPointToLocation(new Point(700, 325));
                var R = 6371; // Radius of the earth in km
                var dLat = ToRadians(CrossLocation.Latitude - RadiusLocation.Latitude);  // Javascript functions in radians
                var dLon = ToRadians((CrossLocation.Longitude - RadiusLocation.Longitude));//.toRad();
                var a = Math.Sin(dLat / 2) * Math.Sin(dLat / 2) +
                        Math.Cos(ToRadians(RadiusLocation.Latitude)) * Math.Cos(ToRadians(CrossLocation.Latitude)) *
                        Math.Sin(dLon / 2) * Math.Sin(dLon / 2);
                var c = 2 * Math.Atan2(Math.Sqrt(a), Math.Sqrt(1 - a));
                var d = R * c; // Distance in km
                var d1 = d * 0.621371192; // distance in miles from center
                var d2 = 2 * d1; //distance along side of square
                SetObservedCrossLocation(sender as Map, CrossLocation);
                SetObservedRadius(sender as Map, d2);
                SetObservedInnerRectangle(sender as Map, rect);
            }
        }
        private static double ToRadians(double degrees)
        {
            return ((2 * Math.PI) / 360) * degrees;
        }
        #endregion

        #region " ObservedBoundingRectangle "

        public static LocationRect GetObservedBoundingRectangle(Map obj)
        {
            return (LocationRect)obj.GetValue(ObservedBoundingRectangleProperty);
        }

        public static void SetObservedBoundingRectangle(Map obj, LocationRect value)
        {
            obj.SetValue(ObservedBoundingRectangleProperty, value);
        }

        // Using a DependencyProperty as the backing store for ObservedWidth.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty ObservedBoundingRectangleProperty =
            DependencyProperty.RegisterAttached("ObservedBoundingRectangle", typeof(LocationRect), typeof(MapObserver), new PropertyMetadata(new LocationRect(0, 0, 0, 0)));

        #endregion

        #region " ObservedInnerRectangle "

        public static LocationRect GetObservedInnerRectangle(Map obj)
        {
            return (LocationRect)obj.GetValue(ObservedInnerRectangleProperty);
        }

        public static void SetObservedInnerRectangle(Map obj, LocationRect value)
        {
            obj.SetValue(ObservedInnerRectangleProperty, value);
        }

        // Using a DependencyProperty as the backing store for ObservedInnerRectangle.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty ObservedInnerRectangleProperty =
            DependencyProperty.RegisterAttached("ObservedInnerRectangle", typeof(LocationRect), typeof(MapObserver), new PropertyMetadata(new LocationRect(0, 0, 0, 0)));

        #endregion


        #region " ObservedCrossLocation " For the participantometer

        public static 
            Location GetObservedCrossLocation(Map obj)
        {
            return (Location)obj.GetValue(ObservedCrossLocationProperty);
        }

        public static void SetObservedCrossLocation(Map obj, Location value)
        {
            obj.SetValue(ObservedCrossLocationProperty, value);
        }

        // Using a DependencyProperty as the backing store for ObservedWidth.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty ObservedCrossLocationProperty =
            DependencyProperty.RegisterAttached("ObservedCrossLocation", typeof(Location), typeof(MapObserver), new PropertyMetadata(new Location(0, 0)));

        #endregion
        #region " ObservedRadius " For the participantometer

        public static double GetObservedRadius(Map obj)
        {
            return (double)obj.GetValue(ObservedRadiusProperty);
        }

        public static void SetObservedRadius(Map obj, double value)
        {
            obj.SetValue(ObservedRadiusProperty, value);
        }

        // Using a DependencyProperty as the backing store for ObservedWidth.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty ObservedRadiusProperty =
            DependencyProperty.RegisterAttached("ObservedRadius", typeof(double), typeof(MapObserver), new PropertyMetadata(0.0));

        #endregion

    }


}
