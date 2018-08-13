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

namespace UcbManagementInformation.Models
{
    public class MainMenuItem
    {
        int menuLevel;

        public int MenuLevel
        {
            get { return menuLevel; }
            set { menuLevel = value; }
        }
        string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        private ObservableCollection<MainMenuItem> children;

        public ObservableCollection<MainMenuItem> Children
        {
            get { return children; }
            set { children = value; }
        }

        private MainMenuItem parentMenuItem;

        public MainMenuItem ParentMenuItem
        {
            get { return parentMenuItem; }
            set { parentMenuItem = value; }
        }

        string url;

        public string Url
        {
            get { return url; }
            set { url = value; }
        }
        private string imageUrl;

        public string ImageUrl
        {
            get { return imageUrl; }
            set { imageUrl = value; }
        }

    }
}
