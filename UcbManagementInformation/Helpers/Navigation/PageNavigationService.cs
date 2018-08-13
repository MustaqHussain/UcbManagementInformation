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
using UcbManagementInformation.ServiceLocation;

namespace UcbManagementInformation.Helpers
{
    public class PageNavigationService : INavigationService
    {
        private readonly NavigationService _navigationService;

        

        public PageNavigationService(NavigationService navigationService)
        {
            _navigationService = navigationService;
        }
        public void Navigate(string url)
	    {
	        _navigationService.Navigate(new Uri(url, UriKind.RelativeOrAbsolute));
	    }
    }
}
