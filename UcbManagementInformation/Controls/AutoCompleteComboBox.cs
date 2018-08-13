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
using System.Windows.Controls.Primitives;

namespace UcbManagementInformation.Controls
{
    public class AutoCompleteComboBox : AutoCompleteBox
    {
        private bool isUpdatingDPs { get; set; }
        #region SelectedItemBinding
        public static readonly DependencyProperty SelectedItemBindingProperty =
            DependencyProperty.Register("SelectedItemBinding",
                    typeof(object),
                    typeof(AutoCompleteComboBox),
                    new PropertyMetadata
                (new PropertyChangedCallback(OnSelectedItemBindingChanged))
                    );

        public object SelectedItemBinding
        {
            get { return GetValue(SelectedItemBindingProperty); }
            set { SetValue(SelectedItemBindingProperty, value); }
        }

        private static void OnSelectedItemBindingChanged
            (DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            ((AutoCompleteComboBox)d).OnSelectedItemBindingChanged(e);
        }

        protected virtual void OnSelectedItemBindingChanged(
                               DependencyPropertyChangedEventArgs e)
        {
            SetSelectemItemUsingSelectedItemBindingDP();
        }

        public void SetSelectemItemUsingSelectedItemBindingDP()
        {
            if (!this.isUpdatingDPs)
                SetValue(SelectedItemProperty, GetValue(SelectedItemBindingProperty));
        }

        #endregion

        protected override void OnDropDownClosed(RoutedPropertyChangedEventArgs<bool> e)
        {
            base.OnDropDownClosed(e);
            UpdateCustomDPs();
        }

        private void UpdateCustomDPs()
        {
            //flag to ensure that we don't reselect the selected item
            this.isUpdatingDPs = true;

            //if a new item is selected or the user blanked out the selection, update the DP
            if (this.SelectedItem != null || this.Text == string.Empty)
            {
                //update the SelectedItemBinding DP
                SetValue(SelectedItemBindingProperty, GetValue(SelectedItemProperty));
            }
            else
            {
                //revert to the originally selected one

                if (this.GetBindingExpression(SelectedItemBindingProperty) != null)
                {
                    SetSelectemItemUsingSelectedItemBindingDP();
                }
            }

            this.isUpdatingDPs = false;
        }

        public AutoCompleteComboBox()
            : base()
        {
            SetCustomFilter();
            this.DefaultStyleKey = typeof(AutoCompleteComboBox);
        }

        public override void OnApplyTemplate()
        {
            base.OnApplyTemplate();

            ToggleButton toggle = (ToggleButton)GetTemplateChild("DropDownToggle");
            if (toggle != null)
            {
                toggle.Click += DropDownToggle_Click;
            }
        }

        private void DropDownToggle_Click(object sender, RoutedEventArgs e)
        {
            FrameworkElement fe = sender as FrameworkElement;
            AutoCompleteBox acb = null;
            while (fe != null && acb == null)
            {
                fe = VisualTreeHelper.GetParent(fe) as FrameworkElement;
                acb = fe as AutoCompleteBox;
            }
            if (acb != null)
            {
                acb.IsDropDownOpen = !acb.IsDropDownOpen;
            }
        }
        protected virtual void SetCustomFilter()
        {
            //custom logic: how to autocomplete
            this.ItemFilter = (prefix, item) =>
            {
                //return all items for empty prefix
                if (string.IsNullOrEmpty(prefix))
                    return true;

                //return all items if a record is already selected
                if (this.SelectedItem != null)
                    if (this.SelectedItem.ToString() == prefix)
                        return true;

                //else return items that contains prefix
                return item.ToString().ToLower().Contains(prefix.ToLower());
            };
        }

        //highlighting logic
        protected override void OnPopulated(PopulatedEventArgs e)
        {
            base.OnPopulated(e);
            ListBox listBox = GetTemplateChild("Selector") as ListBox;
            if (listBox != null)
            {
                //highlight the selected item, if any
                if (this.ItemsSource != null && this.SelectedItem != null)
                {
                    listBox.SelectedItem = this.SelectedItem;

                    //now scroll the selected item into view
                    listBox.Dispatcher.BeginInvoke(delegate
                    {
                        listBox.UpdateLayout();
                        listBox.ScrollIntoView(listBox.SelectedItem);
                    });
                }
            }
        }
    }
}
