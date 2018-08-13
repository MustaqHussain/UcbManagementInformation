using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Windows.Navigation;
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;

namespace UcbManagementInformation.Views.Reporting.ReportWizard
{
    public partial class ReportWizard : Page
    {
        public ReportWizard()
        {
            InitializeComponent();
        }

        // Executes when the user navigates to this page.
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
        }

        

        private void Expander_GotFocus(object sender, RoutedEventArgs e)
        {
            SelectedDataItemListBox.SelectedItem = ((sender as Expander).Parent as StackPanel).DataContext;
            //(sender as Button).Focus();
            
        }

        private void ListBoxDragDropTarget_ItemDroppedOnSource(object sender, Microsoft.Windows.DragEventArgs e)
        {

        }

        private void ListBoxDragDropTarget_ItemDroppedOnTarget(object sender, ItemDragEventArgs e)
        {
            e.Handled = true;
        }

        private void ListBoxDragDropTarget_Drop(object sender, Microsoft.Windows.DragEventArgs e)
        {

        }

        private void TreeViewDragDropTarget_ItemDroppedOnSource(object sender, Microsoft.Windows.DragEventArgs e)
        {

        }

        private void TreeViewDragDropTarget_ItemDroppedOnTarget(object sender, ItemDragEventArgs e)
        {

        }

        private void ListBoxDragDropTargetExtend_Drop(object sender, Microsoft.Windows.DragEventArgs e)
        {
            
        }

        private void ListBoxDragDropTargetExtend_ItemDroppedOnSource(object sender, Microsoft.Windows.DragEventArgs e)
        {

        }

        private void ListBoxDragDropTargetExtend_ItemDroppedOnTarget(object sender, ItemDragEventArgs e)
        {

        }

        

        
        private void SeriesListTarget_ItemDroppedOnTarget(object sender, ItemDragEventArgs e)
        {

        }
        private void SeriesListTarget_Drop(object sender, Microsoft.Windows.DragEventArgs e)
        {

        }

        private void SeriesListTarget_ItemDroppedOnSource(object sender, Microsoft.Windows.DragEventArgs e)
        {

        }

        private void CategoryBorder_Drop(object sender, Microsoft.Windows.DragEventArgs e)
        {

        }

      

        

        

        

       

        

        
    }
}
