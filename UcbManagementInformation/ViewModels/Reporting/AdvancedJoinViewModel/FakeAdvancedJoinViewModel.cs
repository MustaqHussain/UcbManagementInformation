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
    public class FakeAdvancedJoinViewModel : ViewModel,IAdvancedJoinViewModel
    {

        public System.Collections.ObjectModel.ObservableCollection<Server.DataAccess.DataTableJoin> JoinList
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public Server.DataAccess.DataTableJoin SelectedJoin
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

        public System.Collections.ObjectModel.ObservableCollection<string> JoinTypes
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }
    }
}
