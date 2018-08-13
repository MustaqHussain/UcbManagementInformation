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
    public static partial class DataTableJoinBuilder
    {
        #region Create Method
        public static DataTableJoin Create()
        {
            return new DataTableJoin
            {
    				Code = Guid.NewGuid(),
    				FromTable = "test FromTable",
    				FromField = "test FromField",
    				ToTable = "test ToTable",
    				ToField = "test ToField",
    				IsOneToOne = false,
    				RowIdentifier = null,
    				DefaultJoinType = "test DefaultJoinType",
    				DataModelCode = Guid.NewGuid()
            };
        }

        #endregion
    
        #region With Methods
       	public static DataTableJoin WithCode(this DataTableJoin dataTableJoin, Guid code)
        {
            dataTableJoin.Code = code;
            return dataTableJoin;
        }
       	public static DataTableJoin WithFromTable(this DataTableJoin dataTableJoin, String fromTable)
        {
            dataTableJoin.FromTable = fromTable;
            return dataTableJoin;
        }
       	public static DataTableJoin WithFromField(this DataTableJoin dataTableJoin, String fromField)
        {
            dataTableJoin.FromField = fromField;
            return dataTableJoin;
        }
       	public static DataTableJoin WithToTable(this DataTableJoin dataTableJoin, String toTable)
        {
            dataTableJoin.ToTable = toTable;
            return dataTableJoin;
        }
       	public static DataTableJoin WithToField(this DataTableJoin dataTableJoin, String toField)
        {
            dataTableJoin.ToField = toField;
            return dataTableJoin;
        }
       	public static DataTableJoin WithIsOneToOne(this DataTableJoin dataTableJoin, Boolean isOneToOne)
        {
            dataTableJoin.IsOneToOne = isOneToOne;
            return dataTableJoin;
        }
       	public static DataTableJoin WithDefaultJoinType(this DataTableJoin dataTableJoin, String defaultJoinType)
        {
            dataTableJoin.DefaultJoinType = defaultJoinType;
            return dataTableJoin;
        }
       	public static DataTableJoin WithDataModelCode(this DataTableJoin dataTableJoin, Guid dataModelCode)
        {
            dataTableJoin.DataModelCode = dataModelCode;
            return dataTableJoin;
        }
       	public static DataTableJoin WithDataTableRelationshipJoins(this DataTableJoin dataTableJoin, ICollection< DataTableRelationshipJoin> dataTableRelationshipJoins)
        {
            dataTableJoin.DataTableRelationshipJoins = dataTableRelationshipJoins;
            return dataTableJoin;
        }
    
       	public static DataTableJoin WithReportDataTableJoins(this DataTableJoin dataTableJoin, ICollection< ReportDataTableJoin> reportDataTableJoins)
        {
            dataTableJoin.ReportDataTableJoins = reportDataTableJoins;
            return dataTableJoin;
        }
    
       	public static DataTableJoin WithDataModel(this DataTableJoin dataTableJoin, DataModel dataModel)
        {
            dataTableJoin.DataModel = dataModel;
            return dataTableJoin;
        }
    

        #endregion
    }
}
