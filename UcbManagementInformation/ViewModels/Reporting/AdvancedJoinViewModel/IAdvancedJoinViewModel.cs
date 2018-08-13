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
    public interface IAdvancedJoinViewModel
    {
        ObservableCollection<DataTableJoin> JoinList { get; set; }
        DataTableJoin SelectedJoin { get; set; }
        ObservableCollection<string> JoinTypes { get; set; }
    }
}
