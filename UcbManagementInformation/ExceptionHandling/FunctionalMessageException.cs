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

namespace UcbManagementInformation.ExceptionHandling
{
    public class FunctionalException : Exception
    {
        public FunctionalException()
        {
        }

        public FunctionalException(string message) : base(message)
        {
        }

        public FunctionalException(string message, Exception inner) : base(message, inner)
        {
        }
    }
}
