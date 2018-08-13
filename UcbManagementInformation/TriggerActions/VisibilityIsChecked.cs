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
using System.Windows.Controls.Primitives;

namespace UcbManagementInformation.TriggerActions
{
    public class VisibilityIsChecked : TargetedTriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty VisibleWhenCheckedProperty =
            DependencyProperty.Register("VisibleWhenChecked", typeof(bool), 
            typeof(VisibilityIsChecked), new PropertyMetadata(true));
        public bool VisibleWhenChecked
        {
            get
            {
                return (bool)GetValue(VisibleWhenCheckedProperty);
            }
            set
            {
                SetValue(VisibleWhenCheckedProperty, value);
            }
        }
 
        protected override void OnAttached()
        {
            base.OnAttached();
            FrameworkElement element = this.AssociatedObject as FrameworkElement;
            if (element != null) element.Loaded += TargetLoaded;
        }
 
        void TargetLoaded(object sender, RoutedEventArgs e)
        {
            ToggleButton tb = this.AssociatedObject as ToggleButton;
            SetVisibility(tb);
        }
        protected override void Invoke(object parameter)
        {
            RoutedEventArgs args = parameter as RoutedEventArgs;
 
            if (args != null)
            {
                ToggleButton tb = args.OriginalSource as ToggleButton;
                SetVisibility(tb);
            }
        }
 
        private void SetVisibility(ToggleButton tb)
        {
            if (tb != null)
            {
                //Target.Visibility = ((bool)tb.IsChecked == VisibleWhenChecked) ? 
                //Visibility.Visible : Visibility.Collapsed;
                if (tb.IsChecked.Value)
                {
                    VisualStateManager.GoToState(Target as Control, "Showing", true);
                }
                else
                {
                    VisualStateManager.GoToState(Target as Control, "Hidden", true);
                }
            }
        }
    }

}
