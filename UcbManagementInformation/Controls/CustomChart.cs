using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Controls.DataVisualization.Charting;

namespace UcbManagementInformation.Controls
{
    public class CustomChart : Chart
    {
        public CustomChart() : base()
        {
            this.DefaultStyleKey = typeof(CustomChart);
        }
        public static readonly DependencyProperty LinearAxisMaximumProperty = DependencyProperty.Register(
                "LinearAxisMaximum",
                typeof(double),
                typeof(CustomChart),
               new PropertyMetadata(LinearAxisMaximumChanged));

        public double LinearAxisMaximum
        {
            get
            {
                return (double)this.GetValue(LinearAxisMaximumProperty);
            }
            set
            {
                this.SetValue(LinearAxisMaximumProperty, value);
            }
        }
        private static void LinearAxisMaximumChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CustomChart ChartItem = d as CustomChart;
        }
    }
}
