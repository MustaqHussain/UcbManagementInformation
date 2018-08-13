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
using System.Collections.ObjectModel;

namespace UcbManagementInformation.Controls.Carousel
{
    /// <summary>
    /// Represents a dynamic <seealso cref="Image"/> collection that provides
    /// notifications when items get added, removed, or when the entire list is refreshed. 
    /// </summary>
    public class ImageCollection : ObservableCollection<Image>
    {
    }
}
