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
    public class BoundingRectangleObserver
    {

        #region " Observe "

        public static bool GetObserve(FrameworkElement elem)
        {
            return (bool)elem.GetValue(ObserveProperty);
        }

        public static void SetObserve(
          FrameworkElement elem, bool value)
        {
            elem.SetValue(ObserveProperty, value);
        }

        public static readonly DependencyProperty ObserveProperty =
            DependencyProperty.RegisterAttached("Observe", typeof(bool), typeof(BoundingRectangleObserver),
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
            }
        }



        #endregion

        #region " ObservedWidth "

        public static double GetObservedBoundingRectangle(DependencyObject obj)
        {
            return (double)obj.GetValue(ObservedBoundingRectangleProperty);
        }

        public static void SetObservedBoundingRectangle(DependencyObject obj, LocationRect value)
        {
            obj.SetValue(ObservedBoundingRectangleProperty, value);
        }

        // Using a DependencyProperty as the backing store for ObservedWidth.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty ObservedBoundingRectangleProperty =
            DependencyProperty.RegisterAttached("ObservedBoundingRectangle", typeof(LocationRect), typeof(BoundingRectangleObserver), new PropertyMetadata(new LocationRect(0,0,0,0)));

        #endregion


    }


}
