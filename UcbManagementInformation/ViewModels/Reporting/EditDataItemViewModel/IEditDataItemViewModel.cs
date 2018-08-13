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
using UcbManagementInformation.Server.DataAccess;
using System.Collections.ObjectModel;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.ViewModels
{
    public interface IEditDataItemViewModel
    {
        DataItem DataItemToEdit { get; set; }
        RelayCommand SaveCommand { get; set; }
        RelayCommand CancelCommand { get; set; }
        bool? DialogResult { get; set; }
        bool IsSaveButtonVisible { get; set; }
    }
}
