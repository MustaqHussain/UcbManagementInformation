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
using System.Windows.Data;

namespace UcbManagementInformation.TriggerActions
{
    public class TextBoxEnterKeyTrigger : TriggerBase<UIElement> 
    { 
        protected override void OnAttached() 
        {
            base.OnAttached(); 
            TextBox textBox = this.AssociatedObject as TextBox; 
            if (textBox != null)
            {
                textBox.TextChanged += new TextChangedEventHandler(textBox_TextChanged);
                textBox.LostFocus += new RoutedEventHandler(textBox_LostFocus);
                this.AssociatedObject.KeyUp += new System.Windows.Input.KeyEventHandler(AssociatedObject_KeyUp);
            } 
            else
            {
                throw new InvalidOperationException("This behavior only works with TextBoxes");
            } 
        }

        void textBox_LostFocus(object sender, RoutedEventArgs e)
        {
            TextBox textBox = AssociatedObject as TextBox;
            //hopefully the report group.
            InvokeActions(null); 
        }

        void textBox_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression be = ((TextBox)this.AssociatedObject).GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }
        protected override void OnDetaching() 
        {
            base.OnDetaching(); 
            AssociatedObject.KeyUp -= new KeyEventHandler(AssociatedObject_KeyUp);
            ((TextBox)this.AssociatedObject).TextChanged -= new TextChangedEventHandler(textBox_TextChanged);
            AssociatedObject.LostFocus -= new RoutedEventHandler(textBox_LostFocus);
        }
        private void AssociatedObject_KeyUp(object sender, KeyEventArgs e) 
        { 
            if (e.Key == Key.Enter) 
            {
                TextBox textBox = AssociatedObject as TextBox;
                //hopefully the report group.
                InvokeActions(null);
                
            }
        }
    }
}
