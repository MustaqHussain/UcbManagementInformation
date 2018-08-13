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

namespace UcbManagementInformation.ViewModels
{
    public interface IPublishToUcbViewModel
    {
        string Name { get; set; }
        string Description { get; set; }
        bool IsPrintAllowed { get; set; }
        bool IsExportAllowed { get; set; }
        bool? DialogResult { get; set; }
        bool IsSaveButtonVisible { get; set; }

        ReportCategory SelectedCategory
        {
            get;
            set;
        }

        System.Collections.ObjectModel.ObservableCollection<ReportCategory> ReportCategories
        {
            get;
            set;
        }
    }
}
