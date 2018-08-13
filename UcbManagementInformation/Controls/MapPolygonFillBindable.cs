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
    public class MapPolygonFillBindable : MapPolygon
    {

        public Brush BindableFill
        {
            get { return (Brush)GetValue(BindableFillProperty); }
            set
            {
                SetValue(BindableFillProperty, value);
                //Fill = value;
            }
        }

        // Using a DependencyProperty as the backing store for BindableWidth.  This enables animation, styling, binding, etc... 
        public static readonly DependencyProperty BindableFillProperty =
            DependencyProperty.Register("BindableFill", typeof(Brush), typeof(MapPolygonFillBindable), new PropertyMetadata(null,new PropertyChangedCallback(OnBindableFillPropertyChanged)));

        static void OnBindableFillPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            MapPolygonFillBindable instance = d as MapPolygonFillBindable;
            instance.Fill = (Brush)e.NewValue;
        }
    }

}

