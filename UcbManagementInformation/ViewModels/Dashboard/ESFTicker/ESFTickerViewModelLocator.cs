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
    public class ESFTickerViewModelLocator : ViewModelLocatorBase<IESFTickerViewModel>
    {
        public ESFTickerViewModelLocator()
        {
            if (ESFTickerViewModelLocator.IsInDesignMode)
            {
                this.DesigntimeViewModel = new FakeESFTickerViewModel();
            }
        }
    }
}
