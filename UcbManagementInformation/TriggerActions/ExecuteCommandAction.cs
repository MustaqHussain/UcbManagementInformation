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

namespace UcbManagementInformation.TriggerActions
{
    public class ExecuteCommandAction : TriggerAction<DependencyObject> 
    {     
        #region Properties     
        public ICommand Command 
        {  
            get
            { 
                return (ICommand)base.GetValue(CommandProperty);
            }  
            set
            {
                base.SetValue(CommandProperty, value); 
            } 
        }  
        
        public object CommandParameter
        {
            get { return GetValue(CommandParameterProperty); }
            set { SetValue(CommandParameterProperty, value); }
        }
        public static object GetCommandParameter(DependencyObject obj)
        {
            return obj.GetValue(CommandParameterProperty);
        }
        public static void SetCommandParameter(DependencyObject obj, object value)
        {
            obj.SetValue(CommandParameterProperty, value);
        }
 
        
        public static ICommand GetCommand(DependencyObject obj) 
        {
            return (ICommand)obj.GetValue(CommandProperty);  
        } 
        public static void SetCommand(DependencyObject obj, ICommand value) 
        {
            obj.SetValue(CommandProperty, value);  
        }
 
        // We use a DependencyProperty so we can bind commands directly rather   
        // than have to use reflection info to find them   
        public static readonly DependencyProperty CommandProperty =  
            DependencyProperty.Register("Command", typeof(ICommand), typeof(ExecuteCommandAction), null);

        public static DependencyProperty CommandParameterProperty =
        DependencyProperty.Register("CommandParameter",
                                    typeof(object), typeof(ExecuteCommandAction),
                                    null);

        #endregion Properties  
        
        
        protected override void  Invoke(object parameter)
        {
            ICommand command = Command ?? GetCommand(AssociatedObject);
            object commandParameter = CommandParameter ?? GetCommandParameter(AssociatedObject);

            if (command != null && command.CanExecute(commandParameter))
            {
                command.Execute(commandParameter);   
            } 
        } 



    } 
}
