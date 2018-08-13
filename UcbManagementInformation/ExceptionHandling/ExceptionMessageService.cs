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
    //using ModelSupport.Threading;

    public class ExceptionMessageService : IExceptionMessageService
    {
        public void ShowExceptionMessage(string message, Exception exception)
        {
            //var support = new ThreadingSupport();
            //support.ExecuteOnUIThread(() =>
            //                          ErrorWindow.CreateNew(message, exception, Guid.Empty, false));
        }

        public void ShowExceptionMessage(string message, Exception exception, bool closeApplicationAfterMessage)
        {
            //var support = new ThreadingSupport();
            //support.ExecuteOnUIThread(() =>
            //                          ErrorWindow.CreateNew(message, exception, Guid.Empty, closeApplicationAfterMessage));
        }
    }
}
