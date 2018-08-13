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

namespace UcbManagementInformation.MVVM
{
    public class StateManager : DependencyObject
    {
        public static string GetVisualStateProperty(DependencyObject obj)
        {
            return (string)obj.GetValue(VisualStatePropertyProperty);
        }
        public static void SetVisualStateProperty(DependencyObject obj, string value)
        {
            obj.SetValue(VisualStatePropertyProperty, value);
        }
        public static readonly DependencyProperty VisualStatePropertyProperty =
            DependencyProperty.RegisterAttached(
            "VisualStateProperty",
            typeof(string),
            typeof(StateManager),
            new PropertyMetadata((s, e) =>
            {
                var propertyName = (string)e.NewValue;
                var ctrl = s as Control;
                if (ctrl == null)
                    throw new InvalidOperationException("This attached property only supports types derived from Control.");
                System.Windows.VisualStateManager.GoToState(ctrl, (string)e.NewValue, true);
            }));
    }
}
