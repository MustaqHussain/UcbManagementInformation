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
using System.Windows.Threading;
using UcbManagementInformation.Server.DataAccess;
using System.ServiceModel.DomainServices.Client;
using UcbManagementInformation.Web.Models;

namespace UcbManagementInformation.ViewModels
{
    public class ESFTickerViewModel : ViewModel,IESFTickerViewModel
    {

        private  ObservableCollection<string> newsItems;
        private  int currentItemIndex;
        private  string currentItem;
        private  DispatcherTimer timer = new DispatcherTimer();


        public ESFTickerViewModel()
        {
            //myContext.Load<DataItem>(myContext.GetDataItemByCodeQuery(dataItemCode), LoadBehavior.MergeIntoCurrent, LoadDataItemCallback, null);
            
            //ESFTickerContext myContext = new ESFTickerContext();
            //myContext.Load<EsfTickerItemDto>(myContext.GetEsfTickerItemsQuery(),LoadBehavior.MergeIntoCurrent, LoadTickerCallback, null);
        }

        //void LoadTickerCallback(LoadOperation<EsfTickerItemDto> lo)
        //{
        //    newsItems = new ObservableCollection<string>();
            
        //    foreach (EsfTickerItemDto currentItem in lo.Entities)
        //    {
        //        string item = currentItem.GenericText + " : " + currentItem.RetrievedValue.ToString();
        //        newsItems.Add(item);
        //    }

        //    currentItemIndex = 0;
        //    if (newsItems.Count > 0)
        //    {
        //        CurrentItem = newsItems[currentItemIndex];
        //        timer.Tick += new EventHandler(timer_Tick);
        //        timer.Interval = TimeSpan.FromSeconds(5);
        //        timer.Start();
        //    }

        //}
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

        public ObservableCollection<string> NewsItems
        {
            get { return newsItems; }
            set { newsItems = value; }
        }


        void timer_Tick(object sender, EventArgs e)
        {
            CurrentItemIndex++;
            if (currentItemIndex > newsItems.Count-1)
            {
                currentItemIndex = 0;
            }
            CurrentItem = newsItems[currentItemIndex];
        }
    }
}
