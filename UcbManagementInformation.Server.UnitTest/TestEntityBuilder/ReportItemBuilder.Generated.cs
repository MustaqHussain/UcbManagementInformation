//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.UnitTest.TestEntityBuilder
{
    public static partial class ReportItemBuilder
    {
        #region Create Method
        public static ReportItem Create()
        {
            return new ReportItem
            {
    				Code = Guid.NewGuid(),
    				DataItemCode = Guid.NewGuid(),
    				ReportCode = Guid.NewGuid(),
    				IsField = false,
    				IsParameter = false,
    				IsRowTotal = false,
    				IsColumnTotal = false,
    				IsFilter = false,
    				SortField = 100,
    				RowIdentifier = null,
    				IsChartField = false
            };
        }

        #endregion
    
        #region With Methods
       	public static ReportItem WithCode(this ReportItem reportItem, Guid code)
        {
            reportItem.Code = code;
            return reportItem;
        }
       	public static ReportItem WithDataItemCode(this ReportItem reportItem, Guid dataItemCode)
        {
            reportItem.DataItemCode = dataItemCode;
            return reportItem;
        }
       	public static ReportItem WithReportCode(this ReportItem reportItem, Guid reportCode)
        {
            reportItem.ReportCode = reportCode;
            return reportItem;
        }
       	public static ReportItem WithIsField(this ReportItem reportItem, Boolean isField)
        {
            reportItem.IsField = isField;
            return reportItem;
        }
       	public static ReportItem WithIsParameter(this ReportItem reportItem, Boolean isParameter)
        {
            reportItem.IsParameter = isParameter;
            return reportItem;
        }
       	public static ReportItem WithIsRowTotal(this ReportItem reportItem, Boolean isRowTotal)
        {
            reportItem.IsRowTotal = isRowTotal;
            return reportItem;
        }
       	public static ReportItem WithIsColumnTotal(this ReportItem reportItem, Boolean isColumnTotal)
        {
            reportItem.IsColumnTotal = isColumnTotal;
            return reportItem;
        }
       	public static ReportItem WithIsFilter(this ReportItem reportItem, Boolean isFilter)
        {
            reportItem.IsFilter = isFilter;
            return reportItem;
        }
       	public static ReportItem WithSortField(this ReportItem reportItem, Int32 sortField)
        {
            reportItem.SortField = sortField;
            return reportItem;
        }
       	public static ReportItem WithIsChartField(this ReportItem reportItem, Boolean isChartField)
        {
            reportItem.IsChartField = isChartField;
            return reportItem;
        }
       	public static ReportItem WithDataItem(this ReportItem reportItem, DataItem dataItem)
        {
            reportItem.DataItem = dataItem;
            return reportItem;
        }
    
       	public static ReportItem WithFilters(this ReportItem reportItem, ICollection< Filter> filters)
        {
            reportItem.Filters = filters;
            return reportItem;
        }
    
       	public static ReportItem WithReport(this ReportItem reportItem, Report report)
        {
            reportItem.Report = report;
            return reportItem;
        }
    
       	public static ReportItem WithReportCharts(this ReportItem reportItem, ICollection< ReportChart> reportCharts)
        {
            reportItem.ReportCharts = reportCharts;
            return reportItem;
        }
    
       	public static ReportItem WithReportChartSeries(this ReportItem reportItem, ICollection< ReportChartSery> reportChartSeries)
        {
            reportItem.ReportChartSeries = reportChartSeries;
            return reportItem;
        }
    

        #endregion
    }
}
