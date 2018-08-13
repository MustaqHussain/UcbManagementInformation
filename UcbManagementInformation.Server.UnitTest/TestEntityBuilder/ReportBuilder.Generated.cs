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
    public static partial class ReportBuilder
    {
        #region Create Method
        public static Report Create()
        {
            return new Report
            {
    				Code = Guid.NewGuid(),
    				Name = "test Name",
    				Description = "test Description",
    				GroupCode = Guid.NewGuid(),
    				DataModelCode = Guid.NewGuid(),
    				CreationDate = DateTime.Now,
    				CreationUserID = "test CreationUserID",
    				ModifiedDate = DateTime.Now,
    				ModifiedUserID = "test ModifiedUserID",
    				IsPageOnFirstItem = false,
    				IsAllowDrilldown = false,
    				IsInitialExpanded = false,
    				IsMatrix = false,
    				IsStandard = false,
    				IsDataMapDisplayed = false,
    				IsSummaryReport = false,
    				IsOuterJoin = false,
    				RowIdentifier = null
            };
        }

        #endregion
    
        #region With Methods
       	public static Report WithCode(this Report report, Guid code)
        {
            report.Code = code;
            return report;
        }
       	public static Report WithName(this Report report, String name)
        {
            report.Name = name;
            return report;
        }
       	public static Report WithDescription(this Report report, String description)
        {
            report.Description = description;
            return report;
        }
       	public static Report WithGroupCode(this Report report, Guid groupCode)
        {
            report.GroupCode = groupCode;
            return report;
        }
       	public static Report WithDataModelCode(this Report report, Guid dataModelCode)
        {
            report.DataModelCode = dataModelCode;
            return report;
        }
       	public static Report WithCreationDate(this Report report, DateTime creationDate)
        {
            report.CreationDate = creationDate;
            return report;
        }
       	public static Report WithCreationUserID(this Report report, String creationUserID)
        {
            report.CreationUserID = creationUserID;
            return report;
        }
       	public static Report WithModifiedDate(this Report report, DateTime modifiedDate)
        {
            report.ModifiedDate = modifiedDate;
            return report;
        }
       	public static Report WithModifiedUserID(this Report report, String modifiedUserID)
        {
            report.ModifiedUserID = modifiedUserID;
            return report;
        }
       	public static Report WithIsPageOnFirstItem(this Report report, Boolean isPageOnFirstItem)
        {
            report.IsPageOnFirstItem = isPageOnFirstItem;
            return report;
        }
       	public static Report WithIsAllowDrilldown(this Report report, Boolean isAllowDrilldown)
        {
            report.IsAllowDrilldown = isAllowDrilldown;
            return report;
        }
       	public static Report WithIsInitialExpanded(this Report report, Boolean isInitialExpanded)
        {
            report.IsInitialExpanded = isInitialExpanded;
            return report;
        }
       	public static Report WithIsMatrix(this Report report, Boolean isMatrix)
        {
            report.IsMatrix = isMatrix;
            return report;
        }
       	public static Report WithIsStandard(this Report report, Boolean isStandard)
        {
            report.IsStandard = isStandard;
            return report;
        }
       	public static Report WithIsDataMapDisplayed(this Report report, Boolean isDataMapDisplayed)
        {
            report.IsDataMapDisplayed = isDataMapDisplayed;
            return report;
        }
       	public static Report WithIsSummaryReport(this Report report, Boolean isSummaryReport)
        {
            report.IsSummaryReport = isSummaryReport;
            return report;
        }
       	public static Report WithIsOuterJoin(this Report report, Boolean isOuterJoin)
        {
            report.IsOuterJoin = isOuterJoin;
            return report;
        }
       	public static Report WithReportGroup(this Report report, ReportGroup reportGroup)
        {
            report.ReportGroup = reportGroup;
            return report;
        }
    
       	public static Report WithReportItems(this Report report, ICollection< ReportItem> reportItems)
        {
            report.ReportItems = reportItems;
            return report;
        }
    
       	public static Report WithReportDataTableJoins(this Report report, ICollection< ReportDataTableJoin> reportDataTableJoins)
        {
            report.ReportDataTableJoins = reportDataTableJoins;
            return report;
        }
    
       	public static Report WithReportCharts(this Report report, ICollection< ReportChart> reportCharts)
        {
            report.ReportCharts = reportCharts;
            return report;
        }
    
       	public static Report WithDataModel_1(this Report report, DataModel dataModel_1)
        {
            report.DataModel_1 = dataModel_1;
            return report;
        }
    

        #endregion
    }
}
