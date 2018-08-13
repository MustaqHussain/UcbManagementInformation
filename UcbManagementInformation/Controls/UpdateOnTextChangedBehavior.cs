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

namespace UcbManagementInformation.Controls
{
    public class UpdateOnTextChangedBehavior : Behavior<TextBox>
    {
        protected override void OnAttached()
        {
            base.OnAttached();
            this.AssociatedObject.TextChanged += new TextChangedEventHandler(AssociatedObject_TextChanged);
        }

        void AssociatedObject_TextChanged(object sender, TextChangedEventArgs e)
        {
            BindingExpression be = this.AssociatedObject.GetBindingExpression(TextBox.TextProperty);
            be.UpdateSource();
        }
        protected override void OnDetaching()
        {
            base.OnDetaching();
            this.AssociatedObject.TextChanged -= new TextChangedEventHandler(AssociatedObject_TextChanged);
        }
    }
}
