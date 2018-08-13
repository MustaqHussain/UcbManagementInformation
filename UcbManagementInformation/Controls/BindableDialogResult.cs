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

namespace UcbManagementInformation.Controls
{

    public static class BindableDialogResult
    {
        public static readonly DependencyProperty DialogResultProperty
            = DependencyProperty.RegisterAttached(
            "DialogResult",
            typeof(bool?),
            typeof(BindableDialogResult),
            new PropertyMetadata(OnSetDialogResultCallback));


        public static void SetDialogResult(ChildWindow childWindow, bool? dialogResult)
        {
            childWindow.SetValue(DialogResultProperty, dialogResult);
        }

        public static bool? GetDialogResult(ChildWindow childWindow)
        {
            return childWindow.GetValue(DialogResultProperty) as bool?;
        }

        private static void OnSetDialogResultCallback
            (DependencyObject dependencyObject, DependencyPropertyChangedEventArgs e)
        {
            ChildWindow childWindow = dependencyObject as ChildWindow;
            if (childWindow != null)
            {
                childWindow.DialogResult = e.NewValue as bool?;
            }
        }
    }
    
}
