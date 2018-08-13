using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    public class ReportChartSeriesBusinessObject
    {
        public string SeriesReportItemCode { get; set; }
        public string SeriesDataItemCode { get; set; }
        public string Code { get; set; }
        public int SortOrder { get; set; }

    }
}
