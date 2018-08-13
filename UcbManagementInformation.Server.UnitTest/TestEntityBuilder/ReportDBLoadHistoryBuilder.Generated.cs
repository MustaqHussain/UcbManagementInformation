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
    public static partial class ReportDBLoadHistoryBuilder
    {
        #region Create Method
        public static ReportDBLoadHistory Create()
        {
            return new ReportDBLoadHistory
            {
    				Code = Guid.NewGuid(),
    				ReportDBLoadNumber = "test ReportDBLoadNumber",
    				ReportDBLoadStartTime = DateTime.Now,
    				ReportDBLoadTriggeredBy = "test ReportDBLoadTriggeredBy",
    				ReportDBLoadStatus = "test ReportDBLoadStatus",
    				ReportDBLoadEndTime = DateTime.Now,
    				RowIdentifier = null
            };
        }

        #endregion
    
        #region With Methods
       	public static ReportDBLoadHistory WithCode(this ReportDBLoadHistory reportDBLoadHistory, Guid code)
        {
            reportDBLoadHistory.Code = code;
            return reportDBLoadHistory;
        }
       	public static ReportDBLoadHistory WithReportDBLoadNumber(this ReportDBLoadHistory reportDBLoadHistory, String reportDBLoadNumber)
        {
            reportDBLoadHistory.ReportDBLoadNumber = reportDBLoadNumber;
            return reportDBLoadHistory;
        }
       	public static ReportDBLoadHistory WithReportDBLoadStartTime(this ReportDBLoadHistory reportDBLoadHistory, DateTime reportDBLoadStartTime)
        {
            reportDBLoadHistory.ReportDBLoadStartTime = reportDBLoadStartTime;
            return reportDBLoadHistory;
        }
       	public static ReportDBLoadHistory WithReportDBLoadTriggeredBy(this ReportDBLoadHistory reportDBLoadHistory, String reportDBLoadTriggeredBy)
        {
            reportDBLoadHistory.ReportDBLoadTriggeredBy = reportDBLoadTriggeredBy;
            return reportDBLoadHistory;
        }
       	public static ReportDBLoadHistory WithReportDBLoadStatus(this ReportDBLoadHistory reportDBLoadHistory, String reportDBLoadStatus)
        {
            reportDBLoadHistory.ReportDBLoadStatus = reportDBLoadStatus;
            return reportDBLoadHistory;
        }
       	public static ReportDBLoadHistory WithReportDBLoadEndTime(this ReportDBLoadHistory reportDBLoadHistory, DateTime reportDBLoadEndTime)
        {
            reportDBLoadHistory.ReportDBLoadEndTime = reportDBLoadEndTime;
            return reportDBLoadHistory;
        }

        #endregion
    }
}
