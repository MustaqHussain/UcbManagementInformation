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

namespace UcbManagementInformation.Models
{
    public class TitleValue
    {
        public string Title { get; set; }
        public object Value { get; set; }

        public TitleValue()
        {
        }

        public TitleValue(string title, object value)
        {
            Title = title;
            Value = value;
        }
    }
}
