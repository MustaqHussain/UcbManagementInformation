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
using System.Windows.Threading;

namespace UcbManagementInformation.ViewModels
{
    public class FakeESFTickerViewModel : ViewModel, IESFTickerViewModel
    {
        private static ObservableCollection<string> newsItems;
        private static int currentItemIndex;
        private static string currentItem;
        private static DispatcherTimer timer = new DispatcherTimer();
        
        
        public FakeESFTickerViewModel()
        {
            newsItems = new ObservableCollection<string> 
            {
                "Total reports created yesterday: 4",
                "Reports created this week 54",
                "Reports viewed yesterday 106",
                "Reports viewed this week 1976",
                "Reports viewed this year 100543"
            };
            currentItemIndex = 0;
            CurrentItem = FakeESFTickerViewModel.newsItems[currentItemIndex];
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(5);
            timer.Start();
            
        }

        void timer_Tick(object sender, EventArgs e)
        {
            CurrentItemIndex++;
            if (currentItemIndex > 4)
            {
                currentItemIndex = 0;
            }
            CurrentItem = FakeESFTickerViewModel.newsItems[currentItemIndex];
        }
        
        public ObservableCollection<string> NewsItems
        {
            get { return newsItems; }
            set { newsItems = value; }
        }
        public string CurrentItem
        {
            get { return currentItem; }
            set { currentItem = value; OnPropertyChanged("CurrentItem"); }
        }
        public int CurrentItemIndex
        {
            get { return currentItemIndex; }
            set { currentItemIndex = value; }
        }
    }
}
