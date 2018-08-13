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

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class DataItem : INotifyPropertyChanged
    {
        
        private bool isRowGrouping;
        private bool isSelectable;
        private bool isColumnGrouping;
        private bool isParameter;
        private bool isAvailableForRow;
        private bool isAvailableForColumn;

        

        public bool IsSelectable 
        { 
            get{return isSelectable;} 
            set
            {isSelectable=value;
            OnPropertyChanged("IsSelectable");
            }
        }

        public bool IsAvailableForRow
        {
            get { return !IsColumnGrouping && IsGroup; }
            
        }

        public bool IsAvailableForColumn
        {
            get { return !IsRowGrouping && IsGroup; }
            
        }

        public bool IsRowGrouping
        {
            get { return isRowGrouping; }
            set
            {
                isRowGrouping = value;
                OnPropertyChanged("IsRowGrouping");
                OnPropertyChanged("IsAvailableForColumn");
            }
        }
        public bool IsColumnGrouping
        {
            get { return isColumnGrouping; }
            set
            {
                isColumnGrouping = value;
                OnPropertyChanged("IsColumnGrouping");
                OnPropertyChanged("IsAvailableForRow");
            }
        }

        public bool IsParameter
        {
            get { return isParameter; }
            set
            {
                isParameter = value;
                OnPropertyChanged("IsParameter");
            }
        }

        partial void OnCaptionChanged()
        {
            OnPropertyChanged("Caption");
        }

        partial void OnDescriptionChanged()
        {
            OnPropertyChanged("Description");
        }

        partial void OnLocationOnSystemChanged()
        {
            OnPropertyChanged("LocationOnSystem");
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
}
