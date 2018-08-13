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
using System.ComponentModel;
using UcbManagementInformation.Helpers;

namespace UcbManagementInformation.Controls
{
    public class SortButton : Button
    {
        public static readonly DependencyProperty LabelNameProperty = DependencyProperty.Register(
                "LabelName",
                typeof(string),
                typeof(SortButton),
               null);
        public static readonly DependencyProperty NameWidthProperty = DependencyProperty.Register(
                "NameWidth",
                typeof(double),
                typeof(SortButton),
               null);
        public static readonly DependencyProperty IndicatorTextProperty = DependencyProperty.Register(
                "IndicatorText",
                typeof( string ),
                typeof( SortButton ),
               null);
        public static readonly DependencyProperty IndicatorWidthProperty = DependencyProperty.Register(
                "IndicatorWidth",
                typeof(double),
                typeof(SortButton),
               null);
        public static readonly DependencyProperty ControlWidthProperty = DependencyProperty.Register(
                "ControlWidth",
                typeof(double),
                typeof(SortButton),
               new PropertyMetadata(ControlWidthChanged));
        public double ControlWidth
        {
            get
            {
                return (double)this.GetValue(ControlWidthProperty);
            }
            set
            {
                this.SetValue(ControlWidthProperty, value);
            }
        }
        public string LabelName
        {
            get
            {
                return ( string )this.GetValue( LabelNameProperty );
            }            
            set
            {
                this.SetValue(LabelNameProperty, value);
            }
        }
        public double NameWidth
        {
            get
            {
                return (double)this.GetValue(NameWidthProperty);
            }
            set
            {
                this.SetValue(NameWidthProperty, value);
            }

        }
        public string IndicatorText
        {
            get
            {
                return (string)this.GetValue(IndicatorTextProperty);
            }
            set
            {
                this.SetValue(IndicatorTextProperty, value);
            }

        }
        public double IndicatorWidth
        {
            get
            {
                return (double)this.GetValue(IndicatorWidthProperty);
            }
            set
            {
                this.SetValue(IndicatorWidthProperty, value);
            }

        }
        public bool IsSorted { get; set; }
        public bool IsAscending { get; set; }
        public SortButton()
            : base()
        {
            Reset();

        }

        public void Reset()
        {
            IndicatorText = " ";
            IsSorted = false;
            IsAscending = false;

        }

        public void ToggleState()
        {
            switch (IndicatorText)
            {
                case "v":
                    IndicatorText = "^";
                    IsAscending = true;
                    IsSorted = true;
                    //base.Tooltip = "Sort ascending";
                    break;
                case "^":
                    IndicatorText = "v";
                    IsAscending = false;
                    IsSorted = true;
                    //Tooltip = "Sort descending";
                    break;
                case " ":
                    IndicatorText = "^";
                    IsAscending = true;
                    IsSorted = true;
                    //Tooltip = "Sort descending";
                    break;
            }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private static void ControlWidthChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            SortButton ThisButton = d as SortButton;
            ThisButton.Width = ThisButton.ControlWidth;
            ThisButton.NameWidth = (ThisButton.Width > 40 ? ThisButton.Width - 30 : ThisButton.Width);
            ThisButton.IndicatorWidth = (ThisButton.Width > 40 ? 20 : 0);
        }
     }
}
