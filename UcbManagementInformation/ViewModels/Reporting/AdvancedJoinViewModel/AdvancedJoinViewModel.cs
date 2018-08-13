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
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.Generic;

namespace UcbManagementInformation.ViewModels
{
    public class AdvancedJoinViewModel : ViewModel, IAdvancedJoinViewModel
    {

        private ObservableCollection<DataTableJoin> joinList;
        private DataTableJoin selectedJoin;
        private ObservableCollection<string> joinTypes;

        public AdvancedJoinViewModel(IEnumerable<DataTableJoin> joinlistpassed)
        {
            JoinList = new ObservableCollection<DataTableJoin>(joinlistpassed);
            JoinTypes = new ObservableCollection<string> {"INNER","FULL","LEFT","RIGHT" };
        }

        public ObservableCollection<DataTableJoin> JoinList
        {
            get
            {
                return joinList;
            }
            set
            {
                joinList = value; OnPropertyChanged("JoinList");
            }
        }

        public DataTableJoin SelectedJoin
        {
            get
            {
                return selectedJoin;
            }
            set
            {
                selectedJoin = value; OnPropertyChanged("SelectedJoin");
            }
        }

        public ObservableCollection<string> JoinTypes
        {
            get
            {
                return joinTypes;
            }
            set
            {
                joinTypes = value; OnPropertyChanged("JoinTypes");
            }
        }
    }
}
