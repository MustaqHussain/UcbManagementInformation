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
using System.Collections.ObjectModel;
using UcbManagementInformation.MVVM;

namespace UcbManagementInformation.Models
{
    public class PanelSelection : NotifyPropertyChangedEnabledBase
    {
        public SearchType SelectedSearchType { get; set; }
        public string DisplayName { get; set; }
        public string Key { get; set; }
    }
}
