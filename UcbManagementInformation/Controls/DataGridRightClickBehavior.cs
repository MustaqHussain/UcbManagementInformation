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
using System.Windows.Interactivity;
using System.Linq;

namespace UcbManagementInformation.Controls
{
    public class DataGridRightClickBehavior : Behavior<DataGrid>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.MouseRightButtonDown += new MouseButtonEventHandler(AssociatedObject_MouseRightButtonDown);
        }

        void AssociatedObject_MouseRightButtonDown(object sender, MouseButtonEventArgs e)
        {
            var elements = VisualTreeHelper.FindElementsInHostCoordinates(e.GetPosition(null), this.AssociatedObject);
            var rowQuery = from gridRow in elements where gridRow is DataGridRow select gridRow as DataGridRow;
            if
                (rowQuery != null && rowQuery.Count() != 0)
            {
                int rowIndex = rowQuery.First().GetIndex();
                this.AssociatedObject.SelectedIndex = rowIndex;
            }
            else
            {
                this.AssociatedObject.SelectedIndex = -1;
            }
        }

        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.MouseRightButtonDown -= new MouseButtonEventHandler(AssociatedObject_MouseRightButtonDown);
      
        }
    }
}
