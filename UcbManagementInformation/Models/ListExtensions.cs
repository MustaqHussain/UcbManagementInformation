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

namespace UcbManagementInformation.Models
{
    public static class ListExtensions
    {
        /// <summary>     
        /// Removes all entries from a target list where the predicate is true.   
        /// </summary>   
        /// <typeparam name="T">The type of item that must exist in the list.</typeparam>     
        /// <param name="list">The list to remove entries from</param>     
        /// <param name="predicate">The predicate that contains a testing criteria to determine if an entry should be removed from the list.</param>     
        /// <returns>The number of records removed.</returns>    
        public static int RemoveAll<T>(this IList<T> list, Predicate<T> predicate)
        {
            int returnCount = 0;
            for (int i = list.Count - 1; i >= 0; --i)
            {
                if (predicate(list[i]))
                {
                    list.RemoveAt(i);
                    returnCount++;
                }
            }
            return returnCount;
        } 
    }
}
