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
using UcbManagementInformation.MVVM;

namespace UcbManagementInformation.ViewModels
{
    public class MainPageViewModelLocator : ViewModelLocatorBase<IMainPageViewModel>
    {
        public MainPageViewModelLocator()
        {
            if (MainPageViewModelLocator.IsInDesignMode)
            {
                this.DesigntimeViewModel = new FakeMainPageViewModel();
            }
         }
    }
}
