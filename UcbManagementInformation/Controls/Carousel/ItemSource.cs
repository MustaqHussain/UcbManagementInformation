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
using System.Windows.Media.Imaging;

namespace UcbManagementInformation.Controls.Carousel
{
    /// <summary>
    /// Provides an object source type for <see cref="Carousel"/> <see cref="ItemSources"/> property. 
    /// </summary>
    public class ItemSource : DependencyObject
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSource"/> class
        /// </summary>
        public ItemSource()
        {

        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ItemSource"/> class using
        /// <c>BitmapImage</c> object as the <see cref="ImageSource"/> specified
        /// by the supplied URI.
        /// </summary>
        /// <param name="uri">The URI that references the source graphics file for the image. </param>
        /// <param name="title">The picture title.</param>
        public ItemSource(Uri uri, string title, string navigateUri)
        {
            BitmapImage image = new BitmapImage(uri);
            this.ImageSource = image;
            this.Title = title;
            this.NavigateUri = navigateUri;
        }

        /// <summary>
        /// Gets or sets the image source for the <c>Image</c> control. 
        /// </summary>
        /// <value>A source object for the drawn image. </value>
        /// <remarks>You can set the Source by specifying an 
        /// absolute URL (e.g. http://contoso.com/myPicture.jpg) or specify a URL 
        /// relative to the XAP file of your application. </remarks>
        public ImageSource ImageSource
        {
            get { return (ImageSource)GetValue(SourceProperty); }
            set { SetValue(SourceProperty, value); }
        }

        // Using a DependencyProperty as the backing store for Source.  This enables animation, styling, binding, etc...
        /// <summary>
        /// Identifies the <see cref="ImageSource"/> dependency property.
        /// </summary>
        public static readonly DependencyProperty SourceProperty =
            DependencyProperty.Register("ImageSource", typeof(ImageSource), typeof(ItemSource),
            new PropertyMetadata(null));

        /// <summary>
        /// Gets or set a string value indicating the image title.
        /// </summary>
        /// <value> A string value indicating the image title.</value>
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
            DependencyProperty.Register("Title", typeof(string), typeof(ItemSource), 
            new PropertyMetadata(string.Empty));

        /// <summary>
        /// Gets or set a string value indicating the image title.
        /// </summary>
        /// <value> A string value indicating the image title.</value>
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
            DependencyProperty.Register("NavigateUri", typeof(string), typeof(ItemSource),
            new PropertyMetadata(string.Empty));
    }
}
