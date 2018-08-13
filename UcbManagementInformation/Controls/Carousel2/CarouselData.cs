using System.ComponentModel;

namespace UcbManagementInformation.Controls.Carousel2
{

   public class CarouselData : INotifyPropertyChanged
   {
      private bool? useMousePosition;
      private double speed;
      private double height;
      private double width;
      private double scalePerspective;
      
      public event PropertyChangedEventHandler PropertyChanged;

      public bool? UseMousePosition
      {
         get { return useMousePosition; }
         set
         {
            useMousePosition = value;
            OnPropertyChanged( "UseMousePosition" );
         }
      }
      public double Speed
      {
         get { return speed; }
         set
         {
            speed = value;
            OnPropertyChanged( "Speed" );
         }
      }


      public double Height
      {
         get { return height; }
         set
         {
            height = value;
            OnPropertyChanged( "Height" );
         }
      }


      public double Width
      {
         get { return width; }
         set
         {
            width = value;
            OnPropertyChanged( "Width" );
         }
      }


      public double ScalePerspective
      {
         get { return scalePerspective; }
         set
         {
            scalePerspective = value;
            OnPropertyChanged( "ScalePerspective" );
         }
      }



      protected void OnPropertyChanged( string name )
      {
         if ( PropertyChanged != null )
            PropertyChanged( this, new PropertyChangedEventArgs( name ) );

      }

      public static CarouselData GetCarouselData()
      {
         CarouselData d = new CarouselData();
         d.Height = 240;
         d.Width = 800;
         d.UseMousePosition = false;
         d.Speed = 0.25;
         d.ScalePerspective = 0.8;
         return d;
      }
   } 
}
