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
using UcbManagementInformation.Web.Models;

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class ReportGroup 
    {
        public ReportGroupAccessLevelType AccessLevel { get; set; }
        public bool JustAdded { get; set; }
    }
}
