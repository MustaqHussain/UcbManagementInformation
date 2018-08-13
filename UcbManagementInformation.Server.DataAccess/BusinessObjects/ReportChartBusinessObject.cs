using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    public class ReportChartBusinessObject
    {
        public string Style { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Description { get; set; }
        public string CategoryReportItemCode { get; set; }
        public string CategoryDataItemCode { get; set; }
        public int SortOrder { get; set; }

        public List<ReportChartSeriesBusinessObject> ChartSeriesList { get; set; }
    }
}
