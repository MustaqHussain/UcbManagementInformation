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

namespace UcbManagementInformation.ServiceLocation
{
    public class SimpleServiceLocator
    {
        public static void SetServiceLocatorProvider(ISimpleServiceLocator simpleServiceLocator)
        {
            Instance = simpleServiceLocator;
        }

        public static ISimpleServiceLocator Instance { get; private set; }
    }
}
