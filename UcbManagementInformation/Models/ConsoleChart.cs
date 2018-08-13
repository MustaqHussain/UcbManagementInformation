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
using System.Collections.Generic;
using System.ComponentModel;
using System.Collections.ObjectModel;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Models
{
    public partial class ConsoleChart : INotifyPropertyChanged
    {
        private ObservableCollection<DataItem> categoryDataItems = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> CategoryDataItems { get { return categoryDataItems; } set { categoryDataItems = value; OnPropertyChanged("CategoryDataItem"); } }

        private ObservableCollection<DataItem> seriesDataItems = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> SeriesDataItems { get { return seriesDataItems; } set { seriesDataItems = value; OnPropertyChanged("SeriesDataItems"); } }
        private string description;
        public string Description { get { return description; } set { description = value; OnPropertyChanged("Description"); } }

        private string title;
        public string Title { get { return title; } set { title = value; OnPropertyChanged("Title"); } }

        private string chartType;
        public string ChartType { get { return chartType; } set { chartType = value; OnPropertyChanged("ChartType"); } }
        private string chartStyle;
        public string ChartStyle { get { return chartStyle; } set { chartStyle = value; OnPropertyChanged("ChartStyle"); } }
        
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
