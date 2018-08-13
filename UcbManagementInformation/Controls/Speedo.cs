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
using UcbManagementInformation.Helpers;

namespace UcbManagementInformation.Controls
{
    public class Speedo : Control
    {
        public static readonly DependencyProperty Text0LabelProperty = DependencyProperty.Register(
                "Text0Label",
                typeof(string),
                typeof(Speedo),
               null);
        public static readonly DependencyProperty Text20LabelProperty = DependencyProperty.Register(
                "Text20Label",
                typeof(string),
                typeof(Speedo),
               null);
        public static readonly DependencyProperty Text40LabelProperty = DependencyProperty.Register(
                "Text40Label",
                typeof(string),
                typeof(Speedo),
               null);
        public static readonly DependencyProperty Text50LabelProperty = DependencyProperty.Register(
                "Text50Label",
                typeof(string),
                typeof(Speedo),
               null);
        public static readonly DependencyProperty Text60LabelProperty = DependencyProperty.Register(
                "Text60Label",
                typeof(string),
                typeof(Speedo),
               null);
        public static readonly DependencyProperty Text80LabelProperty = DependencyProperty.Register(
                "Text80Label",
                typeof(string),
                typeof(Speedo),
               null);
        public static readonly DependencyProperty Text100LabelProperty = DependencyProperty.Register(
                "Text100Label",
                typeof(string),
                typeof(Speedo),
               null);
        public static readonly DependencyProperty NeedleToProperty = DependencyProperty.Register(
                "NeedleTo",
                typeof(double),
                typeof(Speedo),
               new PropertyMetadata(NeedleToChanged));

        public string Text0Label
        {
            get
            {
                return (string)this.GetValue(Text0LabelProperty);
            }
            set
            {
                this.SetValue(Text0LabelProperty, value);
            }
        }
        public string Text20Label
        {
            get
            {
                return (string)this.GetValue(Text20LabelProperty);
            }
            set
            {
                this.SetValue(Text20LabelProperty, value);
            }
        }
        public string Text40Label
        {
            get
            {
                return (string)this.GetValue(Text40LabelProperty);
            }
            set
            {
                this.SetValue(Text40LabelProperty, value);
            }
        }
        public string Text50Label
        {
            get
            {
                return (string)this.GetValue(Text50LabelProperty);
            }
            set
            {
                this.SetValue(Text50LabelProperty, value);
            }
        }
        public string Text60Label
        {
            get
            {
                return (string)this.GetValue(Text60LabelProperty);
            }
            set
            {
                this.SetValue(Text60LabelProperty, value);
            }
        }
        public string Text80Label
        {
            get
            {
                return (string)this.GetValue(Text80LabelProperty);
            }
            set
            {
                this.SetValue(Text80LabelProperty, value);
            }
        }
        public string Text100Label
        {
            get
            {
                return (string)this.GetValue(Text100LabelProperty);
            }
            set
            {
                this.SetValue(Text100LabelProperty, value);
            }
        }
        public double NeedleTo
        {
            get
            {
                return (double)this.GetValue(NeedleToProperty);
            }
            set
            {
                this.SetValue(NeedleToProperty, value);
            }
        }
 
        public Speedo()
        {
            this.DefaultStyleKey = typeof(Speedo);
        }
        private static void NeedleToChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            
            Speedo ThisSpeedo = d as Speedo;
            //Storyboard currentStoryboard = VisualTreeUtilities.FindVisualParent<Storyboard>(d); 
            //DoubleAnimationUsingKeyFrames animation = currentStoryboard.Children[0] as DoubleAnimationUsingKeyFrames; 
            //SplineDoubleKeyFrame keyframe = animation.KeyFrames[0] as SplineDoubleKeyFrame; 
            //keyframe.Value = (double)e.NewValue;
        }
    }
}
