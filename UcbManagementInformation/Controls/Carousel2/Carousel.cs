using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;


namespace UcbManagementInformation.Controls.Carousel2
{
   // based on code by Jaimie Rodriguez
   // blogs.msdn.com/jaimer
   // See additional notes silverlight.net/blogs/jesseliberty

   public class CarouselPanel : Panel
   {
      #region private member variables

      internal double InternalSpeed { get; set; }
      protected DispatcherTimer timer;
      public double ItemSize { get; set; }

       //Flick scroller convert to flick carousel
        private readonly DispatcherTimer m_animationTimer = 
            new DispatcherTimer();
        private double m_friction;  
        private Point m_currentMousePos; 
        private Point m_previousPoint;
        private Point m_scrollStartOffset = new Point();  
        private Point m_scrollStartPoint;
        private double[] m_startAngle;
        private Point m_scrollTarget = new Point();
        private double m_velocity;
        private ScrollViewer scrollViewer;
      

      #endregion 

      public CarouselPanel()
         : base()
      {

          //Flick scroller convert to flick carousel
       
            //DefaultStyleKey = typeof(FlickScrollView);  
            
            m_friction = 0.9; 
            
            m_animationTimer.Interval = new  
                TimeSpan(0, 0, 0, 0, 20); 
            m_animationTimer.Tick += HandleWorldTimerTick; 
            m_animationTimer.Start();
            
          
          MouseMove += MouseMoveHandler;
          MouseLeftButtonUp += MouseLeftButtonUpHandler;
          MouseLeftButtonDown += MouseLeftButtonDownHandler;
            this.Loaded += new RoutedEventHandler(CarouselPanel_Loaded);
         

         //MouseMove += new MouseEventHandler( CarouselPanel_MouseMove ); 
         //MouseEnter += new MouseEventHandler( CarouselPanel_MouseEnter );
         //MouseLeave += new MouseEventHandler( CarouselPanel_MouseLeave );
         RefreshRate = 15;
         ItemSize = 0.0;
      }

      void CarouselPanel_Loaded(object sender, RoutedEventArgs e)
      {
          m_startAngle = new double[Children.Count];
        /*  foreach (UIElement item in Children)
          {
              item.MouseMove += MouseMoveHandler;
              item.MouseLeftButtonUp += MouseLeftButtonUpHandler;
              item.MouseLeftButtonDown += MouseLeftButtonDownHandler;
          }*/
      }
       private bool IsMouseCaptured { get; set; } 
        
        public double Friction
        { 
            get { return 1.0 - m_friction; } 
            set { m_friction =   
                Math.Min(Math.Max(1.0 - value, 0), 1.0); }  
        } 
        
        
      #region Properties (based on DPs)


      public double Speed
      {
         get { return (double) GetValue( SpeedProperty ); }
         set
         {
            SetValue( SpeedProperty, value );
            InternalSpeed = value;
         }
      }

      // Using a DependencyProperty as the backing store to enable animation, styling, binding, etc.
      public static readonly DependencyProperty SpeedProperty =
        DependencyProperty.Register(
         "Speed",
         typeof( double ),
         typeof( CarouselPanel ),
         new PropertyMetadata( new PropertyChangedCallback( OnSpeedChanged ) ) );


   

      
      // Multiplier by which to scale items as they are further 'back' in the carousel... 
      public double ScalePerspective
      {
         get { return (double) GetValue( ScalePerspectiveProperty ); }
         set { SetValue( ScalePerspectiveProperty, value ); }
      }

      public static readonly DependencyProperty ScalePerspectiveProperty =
                DependencyProperty.Register(
                "ScalePerspective",
                typeof( double ),
                typeof( CarouselPanel ),
                new PropertyMetadata( new PropertyChangedCallback( OnScalePerspectiveChanged ) ) );


      public bool UseMousePosition
      {
         get { return (bool) GetValue( UseMousePositionProperty ); }
         set { SetValue( UseMousePositionProperty, value ); }
      }

      public static readonly DependencyProperty UseMousePositionProperty =
                DependencyProperty.Register(
                "UseMousePosition",
                typeof( bool ),
                typeof( CarouselPanel ),
                null );

      //Rate in MILLISECONDS for getting callbacks; the call 
      public double RefreshRate
      {
         get { return (double) GetValue( RefreshRateProperty ); }
         set { SetValue( RefreshRateProperty, value ); }
      }

      public static readonly DependencyProperty RefreshRateProperty =
                DependencyProperty.Register(
                "RefreshRate",
                typeof( double ),
                typeof( CarouselPanel ),
                null );



      //  Represents location of child setting the property within the carousel
      //  Range is 0-360
      //  NB: Angle is an attached dependency property.  
      //  The children inside the Carousel will set this in their own instance.
      public static readonly DependencyProperty AngleProperty =
            DependencyProperty.RegisterAttached( 
            "Angle", 
            typeof( double ), 
            typeof( CarouselPanel ), 
            null ); 

      
      public static double GetAngle( DependencyObject obj )
      {
         return (double) obj.GetValue( AngleProperty );
      }

      public static void SetAngle( DependencyObject obj, double value )
      {
         obj.SetValue( AngleProperty, value );
      }

      #endregion

      #region Event Handlers 

      private void CarouselPanel_MouseLeave( object sender, MouseEventArgs e )
      {
         // when you enter a child in the panel you get a MouseLeave from both
         // this is not bubbling but two events
         // we only want to stop the timer when the mouse leaves the panel
         if ( sender != this )
            return;

         if ( timer != null )
            timer.Stop();
      }

      // on entering the panel, begin the timer for callbacks
      private void CarouselPanel_MouseEnter( object sender, MouseEventArgs e )
      {
         if ( timer == null )
         {
            timer = new DispatcherTimer();
            timer.Tick += new EventHandler( timer_Tick );
         }

         timer.Interval = TimeSpan.FromMilliseconds( RefreshRate );
         timer.Start();
      }


      private void CarouselPanel_MouseMove( object sender, MouseEventArgs e )
      {
         if ( UseMousePosition )
         {
            // the the mouse position with respect to the panel
            Point p = e.GetPosition( this );
            // divide the X coordinate of the point by the width of the panel to get the 
            // percentage of the panel on the x axis
            double pctX = p.X / this.ActualWidth;
            // set InternalSpeed to twice the difference of 1/2 less pctX and multiply by Speed
            InternalSpeed = Speed * ( 2 * ( 0.5 - pctX ) );
         }
      }

      private void timer_Tick( object sender, EventArgs e )
      {
         for ( int i = 0; i < Children.Count; i++ )
         {
            UIElement item = Children[i];
            double current = (double) item.GetValue( CarouselPanel.AngleProperty );
            item.SetValue( CarouselPanel.AngleProperty,
                     current + ( InternalSpeed * ( 3 / 360.0 ) * ( 2 * Math.PI ) ) );
         }
         DoArrange();
      }



       private void MouseLeftButtonDownHandler(object sender,
            MouseButtonEventArgs e) 
        {
            if (sender == null) 
                return;  
            
            m_scrollStartPoint = e.GetPosition(this);
            for (int i = 0; i < Children.Count; i++)
            {
                m_startAngle[i] = (double)Children[i].GetValue(CarouselPanel.AngleProperty);
            }
           //m_scrollStartOffset.X = scrollViewer.HorizontalOffset; 
            //m_scrollStartOffset.Y = scrollViewer.VerticalOffset; 
            
            CaptureMouse(); 
            IsMouseCaptured = true;
            e.Handled = false;
        }
        
        private void MouseLeftButtonUpHandler(object sender, 
            MouseButtonEventArgs e) 
        { 
            if (!IsMouseCaptured) 
                return;  
            ReleaseMouseCapture();  
            IsMouseCaptured = false;
            e.Handled = false;
        } 
        
        private void MouseMoveHandler(object sender, MouseEventArgs e) 
        {
            if (sender == null) 
                return;  
            
            m_currentMousePos = e.GetPosition(this); 
            
            if (IsMouseCaptured) 
            { 
                Point currentPoint = e.GetPosition(this);
                Point center = new Point((this.Width - ItemSize) / 2, (this.Height - ItemSize) / 2);


                double Alpha = System.Math.Atan(((center.X - m_scrollStartPoint.X) / (center.Y - m_scrollStartPoint.Y)));
                double Beta = System.Math.Atan(((center.X - currentPoint.X) / (center.Y - currentPoint.Y)));

                if ((center.Y - m_scrollStartPoint.Y) < 0)
                {
                    Alpha += Math.PI;
                }

                if ((center.Y - currentPoint.Y) < 0)
                {
                    Beta += Math.PI;
                }
                double theta =  Alpha - Beta;

                for (int i = 0; i < Children.Count; i++)
                {
                    UIElement item = Children[i];
                    double current = (double)item.GetValue(CarouselPanel.AngleProperty);
                    item.SetValue(CarouselPanel.AngleProperty,
                             m_startAngle[i] + theta);
                }
                DoArrange();

                /*
                // Determine the new amount to scroll.
                var delta = new Point(m_scrollStartPoint.X - 
                    currentPoint.X, m_scrollStartPoint.Y - currentPoint.Y); 
                
                m_scrollTarget.X = m_scrollStartOffset.X + delta.X;
                m_scrollTarget.Y = m_scrollStartOffset.Y + delta.Y;  
                
                // Scroll to the new position.
                scrollViewer.ScrollToHorizontalOffset(m_scrollTarget.X);  
                scrollViewer.ScrollToVerticalOffset(m_scrollTarget.Y); */
            }
            
        } 
        
        private void HandleWorldTimerTick(object sender, EventArgs e) 
        {
            //if (scrollViewer == null) 
             //   return; 
            
            if (IsMouseCaptured) 
            {
                Point center = new Point((this.Width - ItemSize) / 2, (this.Height - ItemSize) / 2);

                

                Point currentPoint = m_currentMousePos;

                double Alpha = System.Math.Atan(((center.X - m_previousPoint.X) / (center.Y - m_previousPoint.Y)));
                double Beta = System.Math.Atan(((center.X - currentPoint.X) / (center.Y - currentPoint.Y)));

                double theta = Alpha - Beta;

                Speed = theta;
                //m_velocity.X = m_previousPoint.X - currentPoint.X; 
                //m_velocity.Y = m_previousPoint.Y - currentPoint.Y; 
                m_previousPoint = currentPoint; 
            }
            else
            {
                //if (m_velocity.Length > 1) 
               // {
                //    scrollViewer.ScrollToHorizontalOffset(m_scrollTarget.X);
                 //   scrollViewer.ScrollToVerticalOffset(m_scrollTarget.Y); 
                  //  m_scrollTarget.X += m_velocity.X; 
                  //  m_scrollTarget.Y += m_velocity.Y;
                    //m_velocity *= m_friction;
                
            //    } 
                Speed *= m_friction;
                if (System.Math.Abs(Speed) < .001)
                { 
                    Speed = 0;
                    return;
                }
                for (int i = 0; i < Children.Count; i++)
                {
                    UIElement item = Children[i];
                    double current = (double)item.GetValue(CarouselPanel.AngleProperty);
                    item.SetValue(CarouselPanel.AngleProperty,
                             current + Speed);
                }
            DoArrange();
            }
        } 
      #endregion

      #region Override of drawing methods + helper method

      // called by the layout system whenever layout is invalidated. 
      protected override System.Windows.Size ArrangeOverride( System.Windows.Size finalSize )
      {
         if ( Children.Count == 0 )
            return new Size( Width, Height );

         for ( int i = 0; i < Children.Count; i++ )
         {
            Children[i].SetValue( CarouselPanel.AngleProperty, i * ( Math.PI * 2 ) / Children.Count );
         }
         DoArrange();
         return new Size( Width, Height ); 
      }
        
      private void DoArrange()
      {
         // Center is used to get the 'radiusX' and radiusY of the Ellipse  we are using;
         Point center = new Point( ( this.Width - ItemSize ) / 2, ( this.Height - ItemSize ) / 2 );

         double radiusX = center.X;
         double radiusY = center.Y;


         // cache the value so that we are re-entrant
         double scale = ScalePerspective;

         for ( int i = 0; i < Children.Count; i++ )
         {
            UIElement item = Children[i];
            double radians = (double) item.GetValue( CarouselPanel.AngleProperty );

            Point p = new Point(
                                ( Math.Cos( radians ) * radiusX ) + center.X,
                                ( Math.Sin( radians ) * radiusY ) + center.Y
                               );


            if ( item.RenderTransform == null )
            {
               item.RenderTransform = new MatrixTransform();
               // item.RenderTransformOrigin = new Point(0.5, 0.5); 
            }

            /* Matrix
             * M11 (1.0)   M12 (0.0)      0
             * M21 (0.0)   M22 (1.0)      0
             * OffX(0.0)   OffY(0.0)      1
             * 
             * M11 = X Scale
             * M12 = y Skew
             * M21 = x Skew
             * M22 = Y Scale
             * OffX = translation on x axis
             * OffY = translation on y axis
             */


            MatrixTransform mt = item.RenderTransform as MatrixTransform;
            double scaleMinusRounding = p.Y / ( center.Y + radiusY );
            double scaleX = Math.Min( scaleMinusRounding + scale, 1.0 );
            double scaleY = Math.Min( scaleMinusRounding + scale, 1.0 );
            Matrix mx = new Matrix( scaleX, 0.0, 0.0, scaleY, 0.0, 0.0 );

            mt.Matrix = mx;

            // up to next line !!

            item.RenderTransform = mt;


            // position items that are clsoer to the user in the front .. 
            int zIndex = (int) ( ( p.Y / base.Height ) * 50 );
            item.SetValue( Canvas.ZIndexProperty, zIndex );

            Rect r = new Rect( p.X, p.Y, ItemSize, ItemSize );
            item.Arrange( r );
         }
      }



      protected override Size MeasureOverride( Size availableSize )
      {
         double maxSize = ItemSize;
         int numChildren = 0;
         if ( ItemSize == 0.0 )
         {
            foreach ( UIElement element in Children )
            {
               element.Measure( availableSize );
               maxSize = Math.Max( element.DesiredSize.Width, maxSize );
               maxSize = Math.Max( element.DesiredSize.Height, maxSize );
               ++numChildren;
            }
            ItemSize = maxSize;
         }


         Size sz = new Size( numChildren * maxSize, numChildren * maxSize );
         return sz;
      }

      #endregion

      #region static event handlers for dp's

      protected static void OnSpeedChanged( DependencyObject obj, 
         DependencyPropertyChangedEventArgs args )
      {
         CarouselPanel panel = obj as CarouselPanel;
         panel.InternalSpeed = (double) args.NewValue;
      }


      protected static void OnScalePerspectiveChanged( DependencyObject obj, 
         DependencyPropertyChangedEventArgs args )
      {
         CarouselPanel panel = obj as CarouselPanel;
         if ( panel != null )
            panel.InvalidateArrange();
      }

      #endregion 

   }
}


