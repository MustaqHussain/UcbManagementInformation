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
using UcbManagementInformation.Commanding;
namespace UcbManagementInformation.TriggerActions
{
    public abstract class MapEventToCommandBase<TEventArgsType> : TriggerAction<FrameworkElement> where TEventArgsType : EventArgs //// TriggerAction<FrameworkElement>
    {
        public static readonly DependencyProperty CommandProperty = DependencyProperty.Register("Command", typeof(RelayCommand), typeof(MapEventToCommandBase<TEventArgsType>), new PropertyMetadata(null, OnCommandPropertyChanged));
        public static readonly DependencyProperty CommandParameterProperty = DependencyProperty.Register("CommandParameter", typeof(object), typeof(MapEventToCommandBase<TEventArgsType>), new PropertyMetadata(null, OnCommandParameterPropertyChanged));

        private static void OnCommandParameterPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as MapEventToCommandBase<TEventArgsType>;
            if (invokeCommand != null)
            {
                invokeCommand.SetTheValue(CommandParameterProperty, e.NewValue);
            }
        }
        
        private static void OnCommandPropertyChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            var invokeCommand = d as MapEventToCommandBase<TEventArgsType>;
            if (invokeCommand != null)
            {
                invokeCommand.SetTheValue(CommandProperty, e.NewValue);
            }
        }

        protected override void Invoke(object parameter)
        {
            if (this.Command == null)
            {
                return;
            }

            if (this.Command.CanExecute(parameter))
            {
                var eventInfo = new EventInformation<TEventArgsType>
                                    {
                                        EventArgs = parameter as TEventArgsType,
                                        Sender = this.AssociatedObject,
                                        CommandArgument = GetValue(CommandParameterProperty)
                                    };
                this.Command.Execute(eventInfo);
            }
        }
        //DUE TO A PROBLEM WITH BINDING I WILL ONLY TRY TO SET THE VALUE WHEN THE ASSOCIATEDOBJECT IS NOT NULL
        public void SetTheValue(DependencyProperty dp, object newValue)
        {
            if (this.AssociatedObject != null)
            {
                base.SetValue(dp, newValue);
            }
        }
        public RelayCommand Command
        {
            get
            {
                return (RelayCommand)base.GetValue(CommandProperty);
            }
            set
            {
                base.SetValue(CommandProperty, value);
            }
        }

        public object CommandParameter
        {
            get
            {
                return base.GetValue(CommandParameterProperty);
            }
            set
            {
                base.SetValue(CommandParameterProperty, value);
            }
        }
    }
}
