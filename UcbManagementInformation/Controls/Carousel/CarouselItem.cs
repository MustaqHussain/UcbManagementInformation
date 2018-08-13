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

namespace UcbManagementInformation.Controls.Carousel
{
    /// <summary>
    /// Represents the item used in <see cref="Carousel"/> control.
    /// </summary>
    [TemplateVisualState(Name = CarouselItem.NormalStateName, GroupName = CarouselItem.BorderStateGroupName)]
    [TemplateVisualState(Name = CarouselItem.MouseOverStateName, GroupName = CarouselItem.BorderStateGroupName)]

    [TemplatePart(Name = CarouselItem.ElementBorderName, Type = typeof(Border))]
    [TemplatePart(Name = CarouselItem.ElementMaskName, Type = typeof(Rectangle))]
    public class CarouselItem : Control
    {

        #region Template Parts
        /// <summary>
        /// Element border name.
        /// </summary>
        public const string ElementBorderName = "border";
        /// <summary>
        /// Element Mask Name
        /// </summary>
        public const string ElementMaskName = "mask";

        /// <summary>
        /// Border state group name
        /// </summary>
        public const string BorderStateGroupName = "BorderStateGroup";
        /// <summary>
        /// Normal state name
        /// </summary>
        public const string NormalStateName = "NormalState";
        /// <summary>
        /// Mouse over state name
        /// </summary>
        public const string MouseOverStateName = "MouseOverState";

        /// <summary>
        /// The border which contains the content.
        /// </summary>
        private Border _border;

        /// <summary>
        /// Gets or sets the border which contains the content.
        /// </summary>
        private Border Border
        {
            get
            {
                return _border;
            }
            set
            {
                //if (_border != null)
                {
                    
                }
                _border = value;

                //if (_border != null)
                {

                }
            }
        }

        /// <summary>
        /// The mask rectangle
        /// </summary>
        private Rectangle mask;

        /// <summary>
        /// Gets or sets the mask rectangle
        /// </summary>
        public Rectangle Mask
        {
            get
            {
                return mask;
            }
            set
            {
                mask = value;
            }
        }

        #endregion

        #region Angle Property
        /// <summary>
        /// Gets or sets a value indicating the position of 
        /// the item in angle.
        /// </summary>
        /// <value>The angle specified where the item in the carousel
        /// ellpise.</value>
        public double Angle
        {
            get { return (double)GetValue(AngleProperty); }
            set { SetValue(AngleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Angle.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Angle"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AngleProperty =
            DependencyProperty.Register("Angle", typeof(double), typeof(CarouselItem),
            new PropertyMetadata(0.0, OnAnglePropertyChanged));

        /// <summary>
        /// <see cref="Angle"/> PropertyChangedCallback static function.
        /// </summary>
        /// <param name="d">The <see cref="CarouselItem"/> object whose <see cref="Angle"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnAnglePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CarouselItem item = d as CarouselItem;
            if (d != null)
            {
                item.AngleChanged((double)e.OldValue, (double)e.NewValue);
            }
        } 
        #endregion


        #region Center Property
        /// <summary>
        /// Gets or sets a value specifies the center of the carousel.
        /// </summary>
        public Point Center
        {
            get { return (Point)GetValue(CenterProperty); }
            set { SetValue(CenterProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Center.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Center"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CenterProperty =
            DependencyProperty.Register("Center", typeof(Point),
            typeof(CarouselItem),
            new PropertyMetadata(new Point(), OnCenterPropertyChanged));

        /// <summary>
        /// The <see cref="Center"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="CarouselItem"/> object whose <see cref="Center"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnCenterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CarouselItem item = d as CarouselItem;
            if (d != null)
            {
                item.CenterChanged((Point)e.NewValue);
            }
        } 
        #endregion


        #region Axis Property
        /// <summary>
        /// Gets or sets the length of the axis.
        /// </summary>
        /// <value>A value specifies the X and Y axis by width and height.</value>
        public Size Axis
        {
            get { return (Size)GetValue(AxisProperty); }
            set { SetValue(AxisProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Axis.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Axis"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AxisProperty =
            DependencyProperty.Register("Axis", typeof(Size), typeof(CarouselItem), new PropertyMetadata(new Size(), OnAxisPropertyChanged));


        /// <summary>
        /// The <see cref="Axis"/> PropertyChangedCallback static function.
        /// </summary>
        /// <param name="d">The <see cref="CarouselItem"/> object whose <see cref="Axis"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnAxisPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            CarouselItem item = d as CarouselItem;
            if (d != null)
            {
                item.AxisChanged((Size)e.NewValue);
            }
        } 
        #endregion


        #region Source Property
        /// <summary>
        /// Gets or sets the source for the image. 
        /// </summary>
        /// <value>A source object for the drawn image. </value>
        /// <remarks>You can set the Source by specifying 
        /// an absolute URL (e.g. http://contoso.com/myPicture.jpg) 
        /// or specify a URL relative to the XAP file of your application. </remarks>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Source"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(CarouselItem),
            new PropertyMetadata(null));

        #endregion


        /// <summary>
        /// Initializes a new instance of the <see cref="CarouselItem"/> class
        /// with specified parent Canvas center and axis.
        /// </summary>
        public CarouselItem()
        {
            // The default style key
            this.DefaultStyleKey = typeof(CarouselItem);

            // Add Render Transform group and set the transform 
            // Origin to center of this control 
            this.RenderTransform = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            st.ScaleX = 1;
            st.ScaleY = 1;
            (this.RenderTransform as TransformGroup).Children.Add(st);
            this.RenderTransformOrigin = new Point(0.5, 0.5);

            // Set position on Canvas
            Canvas.SetLeft(this, 0);
            Canvas.SetTop(this, 0);
        }

        /// <summary>
        /// Call this function when  new template applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            Border = GetTemplateChild(ElementBorderName) as Border;
            Mask = GetTemplateChild(ElementMaskName) as Rectangle;
        }

        // Mouse enter
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, MouseOverStateName, true);
        }

        // Mouse leave
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, NormalStateName, true);
        }

        /// <summary>
        /// Call this fucntion when the <see cref="Angle"/> property changed.
        /// </summary>
        /// <param name="oldAngle">The old <see cref="Angle"/> value.</param>
        /// <param name="newAngle">The new <see cref="Angle"/> value.</param>
        /// <remarks>The <see cref="Angle"/> changed indicating the position
        /// of the item changed. So we change the positon of the item in Canvas.
        /// And position changing also make the scale and opacity changing.</remarks>
        private void AngleChanged(double oldAngle, double newAngle)
        {
            //if (oldAngle != newAngle && (oldAngle - newAngle) % (2 * Math.PI) != 0)
            {
                // Position
                double dx = Axis.Width * Math.Cos(newAngle) + Center.X;
                double dy = Axis.Height * Math.Sin(newAngle) + Center.Y;
                Canvas.SetLeft(this, dx);
                Canvas.SetTop(this, dy);

                // Scale 
                double sc = 2 + Math.Cos(newAngle - Math.PI / 2);
                ScaleTransform st = (this.RenderTransform as TransformGroup).Children[0] as ScaleTransform;
                st.ScaleX = sc;
                st.ScaleY = sc;

                // Set the ZIndex based the distance from us, the far item
                // is under the near item
                Canvas.SetZIndex(this, (int)dy);

                // mask, the far one is more blur than near one
                if (mask != null)
                {
                    mask.Opacity = (1 - Math.Cos(newAngle - Math.PI / 2)) * 0.3;
                }
            }
        }

        // The center changed, we need to recalc 
        // the position
        private void CenterChanged(Point center)
        {
            double dx = Axis.Width * Math.Cos(Angle) + center.X;
            double dy = Axis.Height * Math.Sin(Angle) + center.Y;
            Canvas.SetLeft(this, dx);
            Canvas.SetTop(this, dy);

            // Set the ZIndex based the distance from us, the far item
            // is under the near item
            Canvas.SetZIndex(this, (int)dy);
        }

        // The axis changed, we need to recalc 
        // the position
        private void AxisChanged(Size axis)
        {
            double dx = axis.Width * Math.Cos(Angle) + Center.X;
            double dy = axis.Height * Math.Sin(Angle) + Center.Y;
            Canvas.SetLeft(this, dx);
            Canvas.SetTop(this, dy);

            // Set the ZIndex based the distance from us, the far item
            // is under the near item
            Canvas.SetZIndex(this, (int)dy);
        }
    }
}
