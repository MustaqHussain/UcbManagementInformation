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
    /// A control used to display the selected carousel item.
    /// </summary>
    [TemplateVisualState(Name = ItemViewerControl.TitleShowStateName, GroupName = ItemViewerControl.TitleStateGroupName)]
    [TemplateVisualState(Name = ItemViewerControl.TitleHideStateName, GroupName = ItemViewerControl.TitleStateGroupName)]

    [TemplatePart(Name = ItemViewerControl.ElementTitleBorderName, Type = typeof(Border))]
    public class ItemViewerControl : Control
    {
        /// <summary>
        /// Animation of content
        /// </summary>
        private Storyboard scaleOutStoryboard;

        #region Template Parts
        /// <summary>
        /// The title visual state group name
        /// </summary>
        private const string TitleStateGroupName = "TitleStateGroup";

        /// <summary>
        /// The title show visual state name
        /// </summary>
        private const string TitleShowStateName = "TitleShowState";

        /// <summary>
        /// The title hide visual state name 
        /// </summary>
        private const string TitleHideStateName = "TitleHideState";

        /// <summary>
        /// The name of the title border element
        /// </summary>
        private const string ElementTitleBorderName = "titleBorder";

        /// <summary>
        /// Title border
        /// </summary>
        private Border titleBorder;

        /// <summary>
        /// Gets or sets Title Border
        /// </summary>
        private Border TitleBorder
        {
            get
            {
                return titleBorder;
            }
            set
            {
                titleBorder = value;
            }
        }

        #endregion

        #region Source Property
        /// <summary>
        /// Gets or sets the source for the image.
        /// </summary>
        public ImageSource Source
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Content.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Content"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("Source", typeof(ImageSource), typeof(ItemViewerControl),
            new PropertyMetadata(null, OnSourcePropertyChanged));

        /// <summary>
        /// <see cref="Source"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="TitledImage"/> control whose <see cref="Source"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnSourcePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemViewerControl titledImage = d as ItemViewerControl;
            titledImage.OnSourceChanged((ImageSource)e.OldValue, (ImageSource)e.NewValue);
        }
        #endregion

        #region Title Property
        /// <summary>
        /// Gets or sets a <c>string</c> value indicating the image title.
        /// </summary>
        public string Title
        {
            get { return (string)GetValue(TitleProperty); }
            set { SetValue(TitleProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty TitleProperty =
            DependencyProperty.Register("Title", typeof(string), typeof(ItemViewerControl),
            new PropertyMetadata(string.Empty, OnTitlePropertyChanged));


        /// <summary>
        /// <see cref="Title"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="TitledImage"/> control whose <see cref="Title"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnTitlePropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemViewerControl titledImage = d as ItemViewerControl;
            titledImage.OnTitleChanged((string)e.OldValue, (string)e.NewValue);
        }
        #endregion

        #region NavigateUri Property
        /// <summary>
        /// Gets or sets a <c>string</c> value indicating the image navigateUri.
        /// </summary>
        public string NavigateUri
        {
            get { return (string)GetValue(NavigateUriProperty); }
            set { SetValue(NavigateUriProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Title.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Title"/> dependency property. 
        /// </summary>
        public static readonly DependencyProperty NavigateUriProperty =
            DependencyProperty.Register("NavigateUri", typeof(string), typeof(ItemViewerControl),
            new PropertyMetadata(string.Empty, OnNavigateUriPropertyChanged));


        /// <summary>
        /// <see cref="Title"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="TitledImage"/> control whose <see cref="Title"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnNavigateUriPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ItemViewerControl titledImage = d as ItemViewerControl;
            titledImage.OnNavigateUriChanged((string)e.OldValue, (string)e.NewValue);
        }
        #endregion
        
        /// <summary>
        /// Initializes a new instance of the <see cref="ViewItemControl"/> class. 
        /// </summary>
        public ItemViewerControl()
        {
            this.DefaultStyleKey = typeof(ItemViewerControl);

            // Add transform
            TransformGroup tg = new TransformGroup();
            ScaleTransform st = new ScaleTransform();
            tg.Children.Add(st);
            this.RenderTransform = tg;
            this.RenderTransformOrigin = new Point(0.5, 0.5);

            this.Loaded += new RoutedEventHandler(ItemViewerControl_Loaded);
        }

        // When this control loaded
        void ItemViewerControl_Loaded(object sender, RoutedEventArgs e)
        {
            // build the storyboard
            scaleOutStoryboard = BuildStoryboard();
        }

        /// <summary>
        /// Called when apply new template.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            TitleBorder = GetTemplateChild(ElementTitleBorderName) as Border;
        }

        /// <summary>
        /// Called when mouse enter.
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);
            VisualStateManager.GoToState(this, TitleShowStateName, false);
        }

        /// <summary>
        /// Called when mouse leave
        /// </summary>
        /// <param name="e"></param>
        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);
            VisualStateManager.GoToState(this, TitleHideStateName, true);
        }

        /// <summary>
        /// The <see cref="Source"/> property changed.
        /// </summary>
        /// <param name="oldValue">The old <see cref="Source"/> value.</param>
        /// <param name="newValue">The new <see cref="Source"/> value.</param>
        protected virtual void OnSourceChanged(ImageSource oldValue, ImageSource newValue)
        {
            if (newValue != null && scaleOutStoryboard != null)
            {
                scaleOutStoryboard.Begin();
            }

            if (newValue == null)
            {
                VisualStateManager.GoToState(this, TitleHideStateName, false);
            }
        }

        /// <summary>
        ///  The <see cref="Title"/> property changed.
        /// </summary>
        /// <param name="oldValue">The old <see cref="Title"/> value.</param>
        /// <param name="newValue">The new <see cref="Title"/> value.</param>
        protected virtual void OnTitleChanged(string oldTitle, string newTitle)
        {
        }
        /// <summary>
        ///  The <see cref="Uri"/> property changed.
        /// </summary>
        /// <param name="oldValue">The old <see cref="Uri"/> value.</param>
        /// <param name="newValue">The new <see cref="Uri"/> value.</param>
        protected virtual void OnNavigateUriChanged(string oldValue, string newValue)
        {
            
        }  
        /// <summary>
        /// Create the scale out storyboard
        /// </summary>
        /// <returns>The scale out storyboard</returns>
        private Storyboard BuildStoryboard()
        {
            Storyboard sb = null;
            if (!this.Resources.Contains("scaleOutStoryboard"))
            {
                sb = new Storyboard();
                this.Resources.Add("scaleOutStoryboard", sb);

                DoubleAnimation daX = new DoubleAnimation();
                daX.From = 0.1;
                daX.To = 1;
                daX.Duration = TimeSpan.FromSeconds(1);
                Storyboard.SetTarget(daX, this);
                Storyboard.SetTargetProperty(daX, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleX)"));
                sb.Children.Add(daX);

                DoubleAnimation daY = new DoubleAnimation();
                daY.From = 0.1;
                daY.To = 1;
                daY.Duration = TimeSpan.FromSeconds(1);
                Storyboard.SetTarget(daY, this);
                Storyboard.SetTargetProperty(daY, new PropertyPath("(UIElement.RenderTransform).(TransformGroup.Children)[0].(ScaleTransform.ScaleY)"));
                sb.Children.Add(daY);

                if (TitleBorder != null)
                {
                    DoubleAnimation daOpacity = new DoubleAnimation();
                    daOpacity.From = 0;
                    daOpacity.To = 0.6;
                    daOpacity.Duration = TimeSpan.FromSeconds(3);
                    daOpacity.AutoReverse = true;
                    Storyboard.SetTarget(daOpacity, TitleBorder);
                    Storyboard.SetTargetProperty(daOpacity, new PropertyPath("Opacity"));
                    sb.Children.Add(daOpacity);
                }
            }
            else
            {
                sb = this.Resources["scaleOutStoryboard"] as Storyboard;
            }
            return sb;
        }
    }
}
