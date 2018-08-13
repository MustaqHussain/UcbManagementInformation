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
    public class SearchType : INotifyPropertyChanged
    {
        private string _name;

        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(string propertyName)
        {
            var handler = this.PropertyChanged;
            if (handler != null)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        public string Name
        {
            get
            {
                return _name;
            }
            set 
            {
                _name = value;
                OnPropertyChanged("Name");
            }
        }
        public string Object { get; set; }
        public string Field { get; set; }
    }
}
