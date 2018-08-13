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
using System.Windows.Navigation;
using UcbManagementInformation.MVVM;

namespace UcbManagementInformation.Controls
{   public delegate void ConsolePageLoadedHandler(object sender,EventArgs e);
       
    public class UcbManagementInformationPage : Page
    {
        //private bool isNavigatedTo = false;
        //public event ConsolePageLoadedHandler ConsolePageLoaded;
        //public UcbManagementInformationPage()
        //{
        //    this.Loaded += new RoutedEventHandler(UcbManagementInformationPage_Loaded);
        //    this.ConsolePageLoaded += new ConsolePageLoadedHandler(UcbManagementInformationPage_ConsolePageLoaded);
        //}

        //void UcbManagementInformationPage_Loaded(object sender, EventArgs e)
        //{
        //    if (isNavigatedTo)
        //    {
        //        OnConsolePageLoaded(e);
        //    }
        //    isNavigatedTo = false;
        //}

        //void UcbManagementInformationPage_ConsolePageLoaded(object sender, EventArgs e)
        //{

        //}
        
        protected override void OnNavigatedTo(NavigationEventArgs e)
        {
            //isNavigatedTo = true;
            ViewModel vm = DataContext as ViewModel;
            if (vm != null)
            {
                vm.Initialize();
            }
        }

        //protected virtual void OnConsolePageLoaded(EventArgs e)
        //{
        //    if (ConsolePageLoaded != null)
        //    {
        //        ConsolePageLoaded(this, e);
        //    }
        //}
    }
}
