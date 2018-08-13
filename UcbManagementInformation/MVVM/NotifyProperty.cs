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

namespace UcbManagementInformation.Models
{
    public class NotifyProperty<T> : INotifyPropertyChanged
    {
        private T _notifyItem;
        public event PropertyChangedEventHandler PropertyChanged;

        public T NotifyItem
        {
            get
            {
                return _notifyItem;
            }
            set
            {
                _notifyItem = value;
                OnPropertyChanged("NotifyItem");
            }
        }
        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public NotifyProperty()
        {
        }
        public NotifyProperty(T notifyItem)
        {
            NotifyItem = notifyItem;
        }
    }
}
