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
    [MetadataTypeAttribute(typeof(StandardReport.StandardReportMetadata))]
    public partial class StandardReport : IStandardReport
    {
    	public partial class StandardReportMetadata
    	{
    		[Key]
    		public System.Guid Code{get; set;}
    		[Include]
    		[Association("FK_StandardReport_ReportCategory", "ReportCategoryCode", "Code", IsForeignKey = true)]
    		public virtual ReportCategory ReportCategory
    		{get; set;}
    
    		}
    }
}
