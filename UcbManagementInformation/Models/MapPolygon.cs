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
using System.Collections.ObjectModel;
using UcbManagementInformation.MVVM;

namespace UcbManagementInformation.Models
{
    public class MapPolygon : NotifyPropertyChangedEnabledBase, IMapShape
    {
        private int percent;
        private int currentValue;
        private int maxValue;
        public ObservableCollection<LatLong> Locations { get; set; }
        public string Name { get; set; }
        public string ShortName { get; set; }

        public int Percent
        {
            get { return percent; }
            private set { percent = value; OnPropertyChanged("Percent"); }
        }

        public int Value
        { 
            get { return currentValue; }
            set
            {
                currentValue = value;
                OnPropertyChanged("Value");
                Percent = (MaxValue != 0) ? (int)Math.Round((float)Value / (float)MaxValue * 100,0) : 0;
            }
        }

        public int MaxValue
        {
            get { return maxValue; } 
            set
            {
                maxValue = value;
                OnPropertyChanged("MaxValue");
                Percent = (MaxValue != 0) ? (int)Math.Round((float)Value / (float)MaxValue * 100, 0) : 0;
            } 
        }
    }
}
