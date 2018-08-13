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

namespace UcbManagementInformation.TriggerActions
{
    public class EventInformation<TEventArgsType> where TEventArgsType : EventArgs
    {
        public object Sender { get; set; }
        public TEventArgsType EventArgs { get; set; }
        public object CommandArgument { get; set; }
    }
}
