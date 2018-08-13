using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    public class DoubleSeriesItem
    {
        public string IndependantAxisValue { get; set; }
        public List<double> DependentAxisValues { get; set; }

    }
}
