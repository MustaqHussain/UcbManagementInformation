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
using UcbManagementInformation.Web.Models;
using System.Collections.ObjectModel;
using UcbManagementInformation.MVVM;

namespace UcbManagementInformation.Models
{
    public class ReportGroupFolder : NotifyPropertyChangedEnabledBase
    {
        public ReportGroupFolder()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportGroupFolder_PropertyChanged);
        }

        void ReportGroupFolder_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "Name":
                    if (!String.IsNullOrEmpty(FullPath))
                    {
                        string ParentPath = FullPath.Substring(0,FullPath.Substring(0,FullPath.Length-2).LastIndexOf('/')+1);
                        FullPath = ParentPath + Name + "/";
                    }
                    break;
            }

        }

        
        private ObservableCollection<ReportGroupFolder> children = new ObservableCollection<ReportGroupFolder>();
        public ObservableCollection<ReportGroupFolder> Children { get { return children; } set { children = value; } }
        private string name;
        public string Name { get { return name; } set { name = value;OnPropertyChanged("Name");} }
        public Guid Code { get; set; }
        public ReportGroupAccessLevelType AccessLevel { get; set; }
        public string Color { get; set; }
        public string FullPath { get; set; }
        public bool IsTemp { get; set; }
        public byte[] RowIdentifier { get; set; }
        public ReportGroupFolder Parent;
        private bool isExpanded;
        private bool isJustAdded;
        private bool isRenaming;

        public bool IsJustAdded
        {
            get { return isJustAdded; }
            set { isJustAdded = value; OnPropertyChanged("IsJustAdded"); }
        }

        public bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; OnPropertyChanged("IsExpanded"); }
        }

        public bool IsRenaming
        {
            get { return isRenaming; }
            set { isRenaming = value; OnPropertyChanged("IsRenaming"); }
        }

    }
}
