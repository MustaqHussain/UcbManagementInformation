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
using System.ComponentModel;
using System.Collections.ObjectModel;

namespace UcbManagementInformation.Server.DataAccess
{
    
    public partial class DataTableJoin : INotifyPropertyChanged
    {
        

        private string joinType;
        public string JoinType
        {
            get { return joinType; }
            set { joinType = value; OnPropertyChanged("JoinType"); }
        }

        private ObservableCollection<JoinType> availableJoinTypes;
        public ObservableCollection<JoinType> AvailableJoinTypes
        {
            get { return availableJoinTypes; }
            set { availableJoinTypes = value; OnPropertyChanged("AvailableJoinTypes"); }
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
    public class JoinType
    {
        public string ShortText { get; set; }
        public string LongText { get; set; }
    }
}
