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

namespace UcbManagementInformation.Models
{
    public class Stage : NotifyPropertyChangedEnabledBase
    {
        private Guid code;

        public Guid Code
        {
            get { return code; }
            set { code = value; OnPropertyChanged("Code"); }
        }
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; OnPropertyChanged("Name"); }
        }
        private StageStatus status;

        public StageStatus Status
        {
            get { return status; }
            set { status = value; OnPropertyChanged("Status"); }
        }
    }
    public enum StageStatus
    {
        NotStarted,
        Running,
        Completed
    }
}
