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
    [MetadataTypeAttribute(typeof(Staff.StaffMetadata))]
    public partial class Staff : IStaff
    {
    	public partial class StaffMetadata
    	{
    		[Key]
    		public System.Guid Code{get; set;}
    		[Include]
    		[Association("FK_StaffAttributes_Staff", "Code", "StaffCode", IsForeignKey = false)]
    		public virtual ICollection<StaffAttribute> StaffAttributes
    		{get; set;}
    
    		}
    }
}
