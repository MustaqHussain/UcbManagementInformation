using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    public class ColumnSeriesObject
    {
        public string IndependantAxisName { get; set; }
        public List<DoubleSeriesItem> SeriesCollection { get; set; }

    }
}
