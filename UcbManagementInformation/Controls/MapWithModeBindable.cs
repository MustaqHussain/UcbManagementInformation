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
using Microsoft.Maps.MapControl.Core;

namespace UcbManagementInformation.Controls
{
    public class MapWithModeBindable : Map
    {
        public MapWithModeBindable()
            : base()
        {
            base.LoadingError += (s, e) =>
                {
                    base.RootLayer.Children.RemoveAt(5);
                };
        }
        public MapMode BindableMode
        {
            get { return (MapMode)GetValue(BindableModeProperty); }
            set
            {
                SetValue(BindableModeProperty, value);
                //Fill = value;
            }
        }

        // Using a DependencyProperty as the backing store for BindableMode.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty BindableModeProperty =
            DependencyProperty.Register("BindableMode", typeof(MapMode), typeof(MapWithModeBindable), new PropertyMetadata(null, new PropertyChangedCallback(OnBindableModePropertyChanged)));

        static void OnBindableModePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapWithModeBindable instance = d as MapWithModeBindable;
            instance.Mode = (MapMode)e.NewValue;
        }
    }
}
