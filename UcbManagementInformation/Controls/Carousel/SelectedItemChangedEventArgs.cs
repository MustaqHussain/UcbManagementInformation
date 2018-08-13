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
    /// Provide data for <see cref="SelectedItemChanged"/> event.
    /// </summary>
    public class SelectedItemChangedEventArgs : EventArgs
    {
        /// <summary>
        /// Gets or sets the <see cref="ItemSource"/> is selected.
        /// </summary>
        public ItemSource SelectedItem { get; set; }

        /// <summary>
        /// Initialize a new instance of <see cref="SelectedItemChangedEventArgs"/> class.
        /// </summary>
        public SelectedItemChangedEventArgs()
        {

        }

        /// <summary>
        /// Initialize a new instance of <see cref="SelectedItemChangedEventArgs"/>
        /// class supplies the selected item.
        /// </summary>
        /// <param name="selectedItem">The selected <see cref="ItemSource"/> item.</param>
        public SelectedItemChangedEventArgs(ItemSource selectedItem)
        {
            this.SelectedItem = selectedItem;
        }
    }

    public class SelectedItemClickedEventArgs : EventArgs
    {
 
    }
}
