using System;
using System.Collections.ObjectModel;
namespace UcbManagementInformation.ViewModels
{
    public interface IESFTickerViewModel
    {
        string CurrentItem { get; set; }
        int CurrentItemIndex { get; set; }
        ObservableCollection<string> NewsItems { get; set; }
    }
}
