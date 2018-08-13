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
using System.Windows.Data;

namespace UcbManagementInformation.Controls
{
   
    public static class TreeViewItemService
    {
        public static readonly DependencyProperty IsExpandedProperty
            = DependencyProperty.RegisterAttached(
            "IsExpanded",
            typeof(bool),
            typeof(TreeViewItemService),
            new PropertyMetadata(false,OnSetIsExpandedCallback));


        public static void SetIsExpanded(TreeViewItem treeViewItem, bool isExpanded)
        {
            treeViewItem.SetValue(IsExpandedProperty, isExpanded);
        }

        public static bool GetIsExpanded(TreeViewItem treeViewItem)
        {
            return (bool)treeViewItem.GetValue(IsExpandedProperty);
        }

        private static void OnSetIsExpandedCallback
            (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            TreeViewItem treeViewItem = dependencyObject as TreeViewItem;
            if (treeViewItem == null)
            {
                return;
            }
          
            treeViewItem.IsExpanded = (bool)e.NewValue;
        }


    }
    //public class TreeViewItemExpandedBindable : TreeViewItem
    //{
        
    //    protected override DependencyObject GetContainerForItemOverride()
    //    {
    //        var itm = new TreeViewItemExpandedBindable();
    //        itm.SetBinding(TreeViewItem.IsExpandedProperty, new Binding("IsExpanded"){Mode=BindingMode.TwoWay});
    //        return itm;

    //    }
       
    //}
    //public class TreeViewExpandedBindable : TreeView
    //{
        
    //    protected override DependencyObject GetContainerForItemOverride()
    //    {
    //        var itm = new TreeViewItemExpandedBindable();
    //        itm.SetBinding(TreeViewItem.IsExpandedProperty, new Binding("IsExpanded") { Mode = BindingMode.TwoWay });
    //        return itm;

    //    }

    //}
}
