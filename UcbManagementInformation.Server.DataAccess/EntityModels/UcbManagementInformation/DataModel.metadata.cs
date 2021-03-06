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
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace UcbManagementInformation.Server.DataAccess
{
    [MetadataTypeAttribute(typeof(DataModel.DataModelMetadata))]
    public partial class DataModel : IDataModel
    {
    	public partial class DataModelMetadata
    	{
    		[Key]
    		public System.Guid Code{get; set;}
    		[Include]
    		[Association("FK_DataCategory_DataModel", "Code", "DataModelCode", IsForeignKey = false)]
    		public virtual ICollection<DataCategory> DataCategories
    		{get; set;}
    
    		[Include]
    		[Association("FK_DataTable_DataModel", "Code", "DataModelCode", IsForeignKey = false)]
    		public virtual ICollection<DataTable> DataTables
    		{get; set;}
    
    		[Include]
    		[Association("FK_DataTableJoin_DataModel", "Code", "DataModelCode", IsForeignKey = false)]
    		public virtual ICollection<DataTableJoin> DataTableJoins
    		{get; set;}
    
    		[Include]
    		[Association("FK_Report_DataModel1", "Code", "DataModelCode", IsForeignKey = false)]
    		public virtual ICollection<Report> Reports_1
    		{get; set;}
    
    		}
    }
}
