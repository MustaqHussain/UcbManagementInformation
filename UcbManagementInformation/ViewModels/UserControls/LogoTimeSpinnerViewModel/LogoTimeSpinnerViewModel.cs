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
using System.Threading;
using System.Windows.Threading;

namespace UcbManagementInformation.ViewModels
{
    public class LogoTimeSpinnerViewModel : ViewModel, ILogoTimeSpinnerViewModel
    {
        public string datetimeString;

        public LogoTimeSpinnerViewModel()
        {
            this.PropertyChanged +=new System.ComponentModel.PropertyChangedEventHandler(LogoTimeSpinnerViewModel_PropertyChanged);
            DispatcherTimer TimerForClock = new DispatcherTimer();
            TimerForClock.Interval = new TimeSpan(0, 0, 1);
            TimerForClock.Tick += new EventHandler(TimerForClock_Tick);
            TimerForClock.Start();
        }

        void TimerForClock_Tick(object sender, EventArgs e)
        {
            DateTimeString = DateTime.Now.ToString("ddd dd MMM yyyy HH:mm:ss");
        }

        void  LogoTimeSpinnerViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
 	        
        }

      

        public string DateTimeString 
        {
            get { return datetimeString; }
            set { datetimeString = value; OnPropertyChanged("DateTimeString"); }
        }
    }
}
