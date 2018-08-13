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
using System.Collections.Generic;
using System.Collections;
using System.Collections.Specialized;
using System.Windows.Media.Imaging;
using System.Windows.Browser;

namespace UcbManagementInformation.Controls.Carousel
{
    /// <summary>
    /// A control display it's items like a carsouel.
    /// </summary>
    [TemplatePart(Name = Carousel.ElementSelectedItemViewerName, Type = typeof(ItemViewerControl))]
    [TemplatePart(Name=Carousel.ElementCarouselCanvas,Type=typeof(Canvas))]
    [TemplatePart(Name=Carousel.ElementFullScreenButtonName,Type=typeof(Button))]
    public class Carousel : Control
    {
        /// <summary>
        /// The resources dictionary key of the carousel storyboard.
        /// </summary>
        private const string CarouselStoryboardKey = "carouselStoryboard";

        /// <summary>
        /// Represents the circumference of a circle, specified by the constant, π. 
        /// </summary>
        private const double _2PI = 2 * Math.PI;

        /// <summary>
        /// The root visual size
        /// </summary>
        private Size rootVisualSize;

       
        #region Template Parts

        /// <summary>
        /// The name of the selected item panel element
        /// </summary>
        private const string ElementSelectedItemViewerName = "selectedItemViewer";
        /// <summary>
        ///  The name of the carsouel canvas element
        /// </summary>
        private const string ElementCarouselCanvas = "carouselCanvas";

        /// <summary>
        /// The name of the fullscreen button element
        /// </summary>
        private const string ElementFullScreenButtonName = "fullScreenButton";

        /// <summary>
        /// The panel used to display the selected item's larger image (or visual)
        /// </summary>
        private ItemViewerControl _selectedItemViewer;

        /// <summary>
        /// The canvas used to display carsouel like control
        /// </summary>
        private Canvas _carouselCanvas;

        /// <summary>
        /// The full screen button
        /// </summary>
        private Button _fullScreentButton;

        /// <summary>
        /// Gets or sets the SelectedItemPanel template part.
        /// </summary>
        private ItemViewerControl SelectedItemViewer
        {
            get
            {
                return _selectedItemViewer;
            }
            set
            {
                if (_selectedItemViewer != null)
                {
                    // Detach the MouseLeftButtonUp event handler
                    _selectedItemViewer.MouseLeftButtonUp -= _selectedItemViewer_MouseLeftButtonUp;
                }
                _selectedItemViewer = value;

                if (_selectedItemViewer != null)
                {
                    // Attach the MouseLeftButtonUp event hander
                    _selectedItemViewer.MouseLeftButtonUp += new MouseButtonEventHandler(_selectedItemViewer_MouseLeftButtonUp);
                }
            }
        }

        /// <summary>
        /// Gets or sets the CarouselCanvas template part.
        /// </summary>
        private Canvas CarouselCanvas
        {
            get
            {
                return _carouselCanvas;
            }
            set
            {
                if (_carouselCanvas != null)
                {
                    // detach the event handler
                    _carouselCanvas.SizeChanged -= _carouselCanvas_SizeChanged;
                }
                _carouselCanvas = value;

                if (_carouselCanvas != null)
                {
                    // attach the event handler
                    _carouselCanvas.SizeChanged += new SizeChangedEventHandler(_carouselCanvas_SizeChanged);

                    // Set the background
                    _carouselCanvas.Background = CarouselBackground;
                }
            }
        }

        /// <summary>
        /// Gets or sets the full screen button template part.
        /// </summary>
        private Button FullScreenButton
        {
            get
            {
                return _fullScreentButton;
            }
            set
            {
                if (_fullScreentButton != null)
                {
                    // detach the event handle
                    _fullScreentButton.Click -= _fullScreentButton_Click;
                    Application.Current.Host.Content.FullScreenChanged -= Content_FullScreenChanged;

                }

                _fullScreentButton = value;

                if (_fullScreentButton != null)
                {
                    _fullScreentButton.Click += new RoutedEventHandler(_fullScreentButton_Click);
                    //Attach a event handle for full screen
                    Application.Current.Host.Content.FullScreenChanged += new EventHandler(Content_FullScreenChanged);
                }
            }
        }

        #endregion               

        
        #region AutoTurn property

        /// <summary>
        /// Gets or sets a value indicating the carousel will be auto turn or not.
        /// </summary>
        /// <value> A value indicating the carousel will be auto turn or not.
        /// <para> The default value is true, which indicating the carousel will be auto turn when 
        /// it crated.</para></value>
        public bool AutoTurn
        {
            get { return (bool)GetValue(AutoTurnProperty); }
            set { SetValue(AutoTurnProperty, value); }
        }

        // Using a DependencyProperty as the backing store for AutoTurn.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="AutoTurn"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty AutoTurnProperty =
            DependencyProperty.Register("AutoTurn", typeof(bool), typeof(Carousel), new PropertyMetadata(true, OnAutoTurnPropertyChanged));

        /// <summary>
        /// <see cref="AutoTurn"/> PropertyChangedCallback static function.
        /// </summary>
        /// <param name="d"><see cref="Carousel"/> control whose <see cref="AutoTurn"/> property is changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnAutoTurnPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carousel = d as Carousel;
            carousel.AutoTurnChanged((bool)e.NewValue);
        }
        
        #endregion


        #region TurnDirection Property
        /// <summary>
        /// Gets or sets a value indicating the carouse turnning direction.
        /// </summary>
        /// <value>A value indicating the carousel turnning in clockwise 
        /// or in counterclockwise.
        /// <para>The default value is Clockwise.</para></value>
        public TurnDirection TurnDirection
        {
            get { return (TurnDirection)GetValue(TurnDirectionProperty); }
            set { SetValue(TurnDirectionProperty, value); }
        }

        // Using a DependencyProperty as the backing store for TurnDirection.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="TurnDirection"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TurnDirectionProperty =
            DependencyProperty.Register("TurnDirection", typeof(TurnDirection),
            typeof(Carousel),
            new PropertyMetadata(TurnDirection.Clockwise, OnTurnDirectionPropertyChanged));

        /// <summary>
        /// The <see cref="TurnDirection"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="Carousel"/> control whose <see cref="TurnDirection"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnTurnDirectionPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carousel = d as Carousel;
            carousel.TurnDirectionChanged((TurnDirection)e.NewValue);
        } 
        #endregion


        #region Duration Property
        /// <summary>
        /// Gets or set a timespan value specify the carousel turns one lap spended.
        /// </summary>
        /// <value>A timespan value spending which the carousel turns one lap.
        /// <para>The default value is 30 seconds.</para></value>
        public Duration Duration
        {
            get { return (Duration)GetValue(DurationProperty); }
            set { SetValue(DurationProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Duration.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Duration"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty DurationProperty =
            DependencyProperty.Register("Duration", typeof(Duration), typeof(Carousel),
            new PropertyMetadata(new Duration(TimeSpan.FromSeconds(30)), OnDurationPropertyChanged));

        /// <summary>
        /// <see cref="Duration"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="Carousel"/> control whose <see cref="Duration"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnDurationPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carousel = d as Carousel;
            carousel.DurationChanged();
        } 
        #endregion


        #region ItemSources Property
        /// <summary>
        /// Gets or sets the collection of image used to generate the content of the control. 
        /// </summary>
        /// <value>When the carousel only display the image, you can set the Items property  a collection of image.</value>
        /// <remarks>Use this property to set the content of the <see cref="Carousel"/> when only display image.</remarks>
        public ItemSourceCollection ItemSources
        {
            get { return (ItemSourceCollection)GetValue(ItemSourcesProperty); }
            set { SetValue(ItemSourcesProperty, value); }
        }
        

        // Using a DependencyProperty as the backing store for Items.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="Items"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemSourcesProperty =
            DependencyProperty.Register("ItemSources", typeof(ItemSourceCollection), typeof(Carousel),
            new PropertyMetadata(new ItemSourceCollection())); 
        #endregion   


        #region ItemWidth Property
        /// <summary>
        /// Gets or sets the carousel item width.
        /// </summary>
        /// <value>This value indicating the carousel item width.
        /// <para>The default value is 40.</para></value>
        public double ItemWidth
        {
            get { return (double)GetValue(ItemWidthProperty); }
            set { SetValue(ItemWidthProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemWidth.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="ItemWidth"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemWidthProperty =
            DependencyProperty.Register("ItemWidth", typeof(double),
            typeof(Carousel),
            new PropertyMetadata(40.0, OnItemWidthPropertyChanged));

        /// <summary>
        /// The <see cref="ItemWidth"/> PropertyChangedCallback static functin.
        /// </summary>
        /// <param name="d">The <see cref="Carousel"/> control whose <see cref="ItemWidth"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnItemWidthPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carousel = d as Carousel;
            carousel.ItemWidthChanged((double)e.NewValue);
        } 
        #endregion


        #region ItemHeight Property
        /// <summary>
        /// Gets or sets a value to specify the height of the carousel item.
        /// </summary>
        /// <value>A value specify the carousel item's height.
        /// <para>The default value is 40.</para></value>
        public double ItemHeight
        {
            get { return (double)GetValue(ItemHeightProperty); }
            set { SetValue(ItemHeightProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ItemHeight.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="ItemHeight"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty ItemHeightProperty =
            DependencyProperty.Register("ItemHeight", typeof(double), typeof(Carousel), new PropertyMetadata(40.0, OnItemHeightPropertyChanged));


        /// <summary>
        /// The <see cref="ItemHeight"/> PropertyChangedCallback static functin.
        /// </summary>
        /// <param name="d">The <see cref="Carousel"/> control whose <see cref="ItemHeight"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnItemHeightPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carousel = d as Carousel;
            carousel.ItemHeightChanged((double)e.NewValue);
        } 
        #endregion


        #region CarouselBackground Property
        /// <summary>
        /// Gets or sets a brush to paint the carousel canvas.
        /// </summary>
        public Brush CarouselBackground
        {
            get { return (Brush)GetValue(CarouselBackgroundProperty); }
            set { SetValue(CarouselBackgroundProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CarouselBackground.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="CarouselBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty CarouselBackgroundProperty =
            DependencyProperty.Register("CarouselBackground", typeof(Brush),
            typeof(Carousel), new PropertyMetadata(null, OnCarouselBackgroundChanged));

        /// <summary>
        /// <see cref="CarouselBackground"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="Carousel"/> control whose <see cref="CarouselBackground"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnCarouselBackgroundChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carousel = d as Carousel;
            carousel.CarouselBackgroundChanged((Brush)e.NewValue);
        } 
        #endregion

        #region TargetFrame Property
        /// <summary>
        /// Gets or sets a brush to paint the carousel canvas.
        /// </summary>
        public Frame TargetFrame
        {
            get { return (Frame)GetValue(TargetFrameProperty); }
            set { SetValue(TargetFrameProperty, value); }
        }

        // Using a DependencyProperty as the backing store for CarouselBackground.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="CarouselBackground"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty TargetFrameProperty =
            DependencyProperty.Register("TargetFrame", typeof(Frame),
            typeof(Carousel), new PropertyMetadata(null, OnTargetFrameChanged));

        /// <summary>
        /// <see cref="CarouselBackground"/> PropertyChangedCallback function.
        /// </summary>
        /// <param name="d">The <see cref="Carousel"/> control whose <see cref="CarouselBackground"/> property changed.</param>
        /// <param name="e">DependencyPropertyChangedEventArgs which contains the old and new values.</param>
        private static void OnTargetFrameChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            Carousel carousel = d as Carousel;
            carousel.TargetFrameChanged((Frame)e.NewValue);
        }
        #endregion

        /// <summary>
        /// Occurs when the user select a new item.
        /// </summary>
        public event EventHandler<SelectedItemChangedEventArgs> SelectedItemChanged;

        /// <summary>
        /// Occurs when the user select a new item.
        /// </summary>
        public event EventHandler<SelectedItemClickedEventArgs> SelectedItemClicked;


        /// <summary>
        /// Initializes a new instance of the <see cref="Carousel"/> class.
        /// </summary>
        public Carousel()
        {
            this.DefaultStyleKey = typeof(Carousel);
            // User add,remove item from the collection
            this.ItemSources.CollectionChanged += new NotifyCollectionChangedEventHandler(Items_CollectionChanged);
        }
        

        /// <summary>
        /// Turnning one step in specifies direction.
        /// </summary>
        /// <param name="direction">The turnning direction.</param>
        /// <remarks>You can call this function only 
        /// the <see cref="AutoTurn"/> is set to <c>false</c>.</remarks>
        public void TrunOnStep(TurnDirection direction)
        {
            if (AutoTurn)
                return;
            if (CarouselCanvas != null)
            {
                double perAngle = _2PI / CarouselCanvas.Children.Count;
                foreach (var item in CarouselCanvas.Children)
                {
                    CarouselItem cItem = item as CarouselItem;
                    if (cItem != null)
                    {
                        if (direction == TurnDirection.Clockwise)
                        {
                            cItem.Angle += perAngle;
                        }
                        else
                        {
                            cItem.Angle -= perAngle;
                        }

                        // if overceed 2 PI, then adjust it
                        if (cItem.Angle > _2PI)
                        {
                            cItem.Angle -= _2PI;
                        }
                        else if (cItem.Angle < -_2PI)
                        {
                            cItem.Angle += _2PI;
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Builds the visual tree for the <see cref="Carousel"/> control when a new 
        /// template is applied.
        /// </summary>
        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();
            SelectedItemViewer = GetTemplateChild(ElementSelectedItemViewerName) as ItemViewerControl;
            CarouselCanvas = GetTemplateChild(ElementCarouselCanvas) as Canvas;
            FullScreenButton = GetTemplateChild(ElementFullScreenButtonName) as Button;

            if (CarouselCanvas != null)
            {
                // Place items to carousel canvas
                PlaceItem(ItemSources);

                // Build storyboard
                Storyboard sb = BuildStoryboard(CarouselCanvas.Children, this.TurnDirection);

                if (this.AutoTurn)
                    sb.Begin();
            }
        }

        #region Private Property Changed Callback

        // The item collection changed
        private void Items_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (CarouselCanvas != null)
            {                

                switch(e.Action)
                {
                    case NotifyCollectionChangedAction.Add:
                        // Add item to collection
                        {
                            // X and Y axis of the ellipse
                            double radiusX = (this.CarouselCanvas.ActualWidth - 2 * ItemWidth
                                - (Padding.Left + Padding.Right)) / 2;
                            double radiusY = (this.CarouselCanvas.ActualHeight - 2 * ItemHeight
                                - (Padding.Top + Padding.Bottom)) / 2;

                            // center point of the ellipse
                            double centerX = radiusX + ItemWidth / 2 + Padding.Left;
                            double centerY = radiusY + Padding.Top;


                            Point center = new Point(centerX, centerY);
                            Size axis = new Size(radiusX, radiusY);

                            int count = e.NewItems.Count;
                            for (int i = 0; i < count; i++)
                            {                             

                                CarouselItem cItem = PlaceItem(e.NewItems[i] as ItemSource);
                                cItem.Center = center;
                                cItem.Axis = axis;
                            }

                            Storyboard sb = BuildStoryboard(CarouselCanvas.Children, this.TurnDirection);
                            if (this.AutoTurn)
                                sb.Begin();
                        }
                        break;

                    case NotifyCollectionChangedAction.Remove:
                        // Remove item from collection
                        {
                            int count = e.OldItems.Count;
                            for (int i = 0; i < count; i++)
                            {
                                CarouselCanvas.Children.RemoveAt(e.OldStartingIndex + i);
                            }

                            Storyboard sb = BuildStoryboard(CarouselCanvas.Children, this.TurnDirection);
                            if (this.AutoTurn)
                                sb.Begin();
                        }
                        break;

                    case NotifyCollectionChangedAction.Replace:
                        // Replace item
                        {
                            int count = e.OldItems.Count;
                            for (int i = 0; i < count; i++)
                            {
                                CarouselItem item = CarouselCanvas.Children[e.OldStartingIndex + i] as CarouselItem;
                                item.Source = (e.NewItems[i] as ItemSource).ImageSource;
                            }

                        }
                        break;

                    case NotifyCollectionChangedAction.Reset:
                        // Reset the collection
                        {
                            // do nothing
                        }
                        break;
                }
            }
        }

        //AutoTurn property chanaged
        private void AutoTurnChanged(bool turnning)
        {
            Storyboard sb = this.Resources[CarouselStoryboardKey] as Storyboard;
            if (sb != null)
            {
                if (turnning)
                {
                    sb.Begin();
                }
                else
                {
                    sb.Stop();
                }
            }
        }

        // The turnning direction changed
        private void TurnDirectionChanged(TurnDirection direction)
        {
            if (CarouselCanvas != null)
            {
                Storyboard sb = BuildStoryboard(CarouselCanvas.Children, direction);
                if (AutoTurn)
                    sb.Begin();
            }
        }

        // The turnning duration changed
        private void DurationChanged()
        {
            if (CarouselCanvas != null)
            {
                Storyboard sb = BuildStoryboard(CarouselCanvas.Children, TurnDirection);
                if (AutoTurn)
                    sb.Begin();
            }
        }

        // The carousel item width changed
        private void ItemWidthChanged(double itemWidth)
        {
            if (CarouselCanvas != null)
            {
                foreach (var item in CarouselCanvas.Children)
                {
                    CarouselItem ci = item as CarouselItem;
                    if (ci != null)
                    {
                        ci.Width = itemWidth;
                    }
                }
            }
        }

        // The carousel item height changed
        private void ItemHeightChanged(double itemHeight)
        {
            if (CarouselCanvas != null)
            {
                foreach (var item in CarouselCanvas.Children)
                {
                    CarouselItem ci = item as CarouselItem;
                    if (ci != null)
                    {
                        ci.Width = itemHeight;
                    }
                }
            }
        }

        /// <summary>
        /// The carousel canvas background changed.
        /// </summary>
        private void CarouselBackgroundChanged(Brush brush)
        {
            if (CarouselCanvas != null)
            {
                CarouselCanvas.Background = brush;
            }
        }

        /// <summary>
        /// The carousel canvas background changed.
        /// </summary>
        private void TargetFrameChanged(Frame brush)
        {
            
        }
        #endregion

        /// <summary>
        /// Place the items to the proper position.
        /// </summary>
        /// <param name="images">The item collection will be placed.</param>
        /// <param name="canvasSize">The size of carousel canvas.</param>
        private void PlaceItem(ItemSourceCollection items)
        {
            if (CarouselCanvas != null)
            {
                // Clear all content first
                CarouselCanvas.Children.Clear();

                int imagesCount = items.Count;
                for (int i = 0; i < imagesCount; i++)
                {
                    PlaceItem(ItemSources[i]);
                }

                // if you do not want init select a picture, please comment out 
                // the follow lines
                /*if (imagesCount > 0)
                {
                    //we only select when we set images
                    SelectedItemViewer.Source = ItemSources[0].ImageSource;
                    SelectedItemViewer.Title = ItemSources[0].Title;
                    SelectedItemViewer.NavigateUri = ItemSources[0].NavigateUri;
                }*/
            }
        }

        /// <summary>
        /// Place a item to carousel canvas.
        /// </summary>
        /// <param name="item">The item will be placed.</param>
        /// <returns>The <see cref="CarouselItem"/> just placed.</returns>
        private CarouselItem PlaceItem(ItemSource item)
        {
            CarouselItem cItem = new CarouselItem();
            cItem.Width = ItemWidth;
            cItem.Height = ItemHeight;
            cItem.Source = item.ImageSource;
            cItem.Tag = item;

            // Attach the handle
            cItem.MouseLeftButtonDown += new MouseButtonEventHandler(item_MouseLeftButtonDown);

            // Add item to carousel canvas
            CarouselCanvas.Children.Add(cItem);

            return cItem;
        }

        /// <summary>
        /// ReAssign angle of all items
        /// </summary>
        //private void ReAssignAngle()
        //{
        //    if (CarouselCanvas != null)
        //    {
        //        int count = CarouselCanvas.Children.Count;
        //        // Re calc the angle
        //        double perAngle = _2PI / count;
        //        for(int i=0;i<count;i++)
        //        {
        //            (CarouselCanvas.Children[i] as CarouselItem).Angle = i * perAngle;
        //        }
        //    }
        //}

        // Fire the SelectedItemChanged event
        void OnSelectedItemChanged(ItemSource item)
        {
            if (SelectedItemChanged != null)
            {
                SelectedItemChangedEventArgs e = new SelectedItemChangedEventArgs(item);
                SelectedItemChanged(this, e);
            }
        }
        // Fire the SelectedItemChanged event
        void OnSelectedItemClicked()
        {
            if (SelectedItemClicked != null)
            {
                SelectedItemClickedEventArgs e = new SelectedItemClickedEventArgs();
                SelectedItemClicked(this, e);
            }
        }

        // User click the item with left mouse button
        void item_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (SelectedItemViewer != null)
            {
                CarouselItem item = sender as CarouselItem;
                ItemSource source = item.Tag as ItemSource;
                SelectedItemViewer.Source = source.ImageSource;
                SelectedItemViewer.Title = source.Title;
                SelectedItemViewer.NavigateUri = source.NavigateUri;
                OnSelectedItemChanged(source);
            }
        }

        /// <summary>
        /// The carousel canvas's size changed.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _carouselCanvas_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            // The panel width not overceed half of the canvas
            SelectedItemViewer.MaxWidth = this.CarouselCanvas.ActualWidth * 0.6;

            // X and Y axis of the ellipse
            double radiusX = (e.NewSize.Width - 2 * ItemWidth
                - (Padding.Left + Padding.Right)) / 2;
            double radiusY = (e.NewSize.Height - 2 * ItemHeight
                - (Padding.Top + Padding.Bottom)) / 2;

            //// center point of the ellipse
            double centerX = radiusX + ItemWidth / 2 + Padding.Left;
            double centerY = radiusY + Padding.Top;


            Point center = new Point(centerX, centerY);
            Size axis = new Size(radiusX, radiusY);

            foreach (var item in CarouselCanvas.Children)
            {
                (item as CarouselItem).Center = center;
                (item as CarouselItem).Axis = axis;
            }
            
        }

        /// <summary>
        /// User clicked the panel of display the selected item.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _selectedItemViewer_MouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            SelectedItemViewer.Source = null;
            if (this.TargetFrame!=null)
            if (!string.IsNullOrEmpty(_selectedItemViewer.NavigateUri))
            {
                if (this.TargetFrame != null)
                {
                    this.TargetFrame.Navigate(new Uri(_selectedItemViewer.NavigateUri, UriKind.Relative)); 
                }
                else
                {
                    HtmlPage.Window.Navigate(new Uri(_selectedItemViewer.NavigateUri, UriKind.Relative)); 
                }
            }
            OnSelectedItemClicked();
        }

        /// <summary>
        /// Full screen button clicked
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void _fullScreentButton_Click(object sender, RoutedEventArgs e)
        {
            if (Application.Current.RootVisual is FrameworkElement)
            {
                FrameworkElement fe = Application.Current.RootVisual as FrameworkElement;
                rootVisualSize = new Size(fe.Width, fe.Height);
                fe.Width = double.NaN;
                fe.Height = double.NaN;
            }
            Application.Current.Host.Content.IsFullScreen = true;
        }

        /// <summary>
        /// Called when fullscreen changed
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        void Content_FullScreenChanged(object sender, EventArgs e)
        {
            if (Application.Current.Host.Content.IsFullScreen)
            {
                FullScreenButton.Visibility = Visibility.Collapsed;
            }
            else
            {
                if (Application.Current.RootVisual is FrameworkElement)
                {
                    FrameworkElement fe = Application.Current.RootVisual as FrameworkElement;
                    fe.Width = rootVisualSize.Width;
                    fe.Height = rootVisualSize.Height;
                }

                FullScreenButton.Visibility = Visibility.Visible;
            }
        }

        /// <summary>
        /// Build storyboard to animate the item.
        /// </summary>
        /// <param name="name">The name of the storyboard.</param>
        /// <param name="from">The from angle.</param>
        /// <param name="to">The end angle.</param>
        /// <param name="duration">The duration of the animation.</param>
        /// <returns>The builded storyboard.</returns>
        //private Storyboard BuildStoryboard(CarouselItem item, double from, double to, Duration duration)
        //{
        //    Storyboard storyboard = new Storyboard();
        //    //this.Resources.Add(name, storyboard);

        //    // Angle animation
        //    DoubleAnimation doubleAnimation = new DoubleAnimation();
        //    doubleAnimation.From = from;
        //    doubleAnimation.To = to;
        //    doubleAnimation.Duration = duration;
        //    doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

        //    // Set storyboard target property
        //    storyboard.Children.Add(doubleAnimation);
        //    Storyboard.SetTarget(doubleAnimation, item);
        //    Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Angle"));

        //    return storyboard;
        //}

        /// <summary>
        /// Build storyboard to animate the item.
        /// </summary>
        /// <param name="name">The name of the storyboard.</param>
        /// <param name="by">The total amount by which the animation changes its starting value.</param>
        /// <returns>The builded storyboard.</returns>
        private Storyboard BuildStoryboard(CarouselItem item, double by, Duration duration)
        {
            Storyboard storyboard = new Storyboard();
            //this.Resources.Add(name, storyboard);

            // Angle animation
            DoubleAnimation doubleAnimation = new DoubleAnimation();
            doubleAnimation.By = by;
            doubleAnimation.Duration = duration;
            doubleAnimation.RepeatBehavior = RepeatBehavior.Forever;

            // Set storyboard target property
            storyboard.Children.Add(doubleAnimation);
            Storyboard.SetTarget(doubleAnimation, item);
            Storyboard.SetTargetProperty(doubleAnimation, new PropertyPath("Angle"));

            return storyboard;
        }

        /// <summary>
        /// Create the total storyboard contains all item's animations. 
        /// </summary>
        /// <param name="items">The carousel items collection.</param>
        /// <param name="direction">The carousel turnning direction.</param>
        /// <returns>The storyboard just created.</returns>
        private Storyboard BuildStoryboard(UIElementCollection items, TurnDirection direction)
        {
            // The carousel storyboard
            Storyboard sb = null;

            if (this.Resources.Contains(CarouselStoryboardKey))
            {
                // if already in resources, get from resources
                sb = this.Resources[CarouselStoryboardKey] as Storyboard;
                // Must stop it before we change it
                sb.Stop();
                // and clear the children
                sb.Children.Clear(); // because we rebuild all item
            }
            else
            {
                sb = new Storyboard();
                // Add to resources dictionary
                this.Resources.Add(CarouselStoryboardKey, sb);
            }

            // Now, we start build each item's storyboard
            int itemsCount = items.Count;
            double perAngle=_2PI / itemsCount;
            double by = _2PI;
            for (int i = 0; i < itemsCount; i++)
            {
                double angle = i * perAngle + Math.PI / 2;

                // We only care the carouselitem
                CarouselItem item = items[i] as CarouselItem;
                if (item != null)
                {
                    item.Angle = angle;

                    // Add item's storyboard to main storyboard
                    if (direction == TurnDirection.Clockwise)
                    {
                        sb.Children.Add(BuildStoryboard(item, by, Duration));
                    }
                    else
                    {
                        sb.Children.Add(BuildStoryboard(item, -by, Duration));
                    }                   
                }
            }

            return sb;
        }
        
    }
}
