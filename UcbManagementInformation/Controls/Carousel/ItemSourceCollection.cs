using System;
using System.Linq;
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
using System.Collections.Generic;

namespace UcbManagementInformation.Controls.Carousel
{
    /// <summary>
    /// Represents a dynamic <seealso cref="ItemSource"/> collection that provides
    /// notifications when items get added, removed, or when the entire list is refreshed. 
    /// </summary>
    public class ItemSourceCollection : ObservableCollection<ItemSource>
    {
        /// <summary>
        /// Add a <see cref="ItemSource"/> to the collection which specified by 
        /// the picture URI and title.
        /// </summary>
        /// <param name="uri">The URI that references the source graphics file for the image.  </param>
        /// <param name="title">The string title for the picture.</param>
        public virtual void Add(Uri uri, string title,string navigateUri)
        {
            ItemSource itemSource = new ItemSource(uri, title,navigateUri);
            this.Add(itemSource);
        }

        /// <summary>
        /// Remove all the item whose title match the 
        /// supplied <paramref name="title"/>.
        /// </summary>
        /// <param name="title">The title of the item which will be removed.</param>
        /// <returns>The total item has been removed.</returns>
        public virtual int Remove(string title)
        {
            int count = 0;
            var matched = this.Items.Where(s => s.Title == title).ToList();
            foreach (var item in matched)
            {
                if (this.Remove(item))
                    count++;
            }
            return count;
        }
    }
}
