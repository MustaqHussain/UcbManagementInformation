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

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    public partial class ReportDefinitionBusinessObject
    {
        public List<DataItem> SelectedDataItems{get;set;}
        public List<DataItem> RowTotalDataItems{get;set;}
        public List<DataItem> ColumnTotalDataItems{get;set;}
        public List<DataItem> ParameterDataItems{get;set;}
    }
}
