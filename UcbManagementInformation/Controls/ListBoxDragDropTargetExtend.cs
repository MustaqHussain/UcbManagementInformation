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
using UcbManagementInformation.Models;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Controls
{
    public class ListBoxDragDropTargetExtend : ListBoxDragDropTarget
    {
        protected override bool CanAddItem(ListBox itemsControl, object data)
        {
            bool baseCanAddItem = base.CanAddItem(itemsControl, data);
            if (baseCanAddItem== false && itemsControl.ItemsSource is ObservableCollection<Models.Filter> && data is DataItem)
            {
                return true;
            }
            else
            {
                return baseCanAddItem;
            }
        }
    }
}
