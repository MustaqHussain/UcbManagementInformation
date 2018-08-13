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
    [MetadataTypeAttribute(typeof(Application.ApplicationMetadata))]
    public partial class Application : IApplication
    {
    	public partial class ApplicationMetadata
    	{
    		[Key]
    		public System.Guid Code{get; set;}
    		[Include]
    		[Association("FK_ApplicationAttribute_Application", "Code", "ApplicationCode", IsForeignKey = false)]
    		public virtual ICollection<ApplicationAttribute> ApplicationAttributes
    		{get; set;}
    
    		[Include]
    		[Association("FK_StaffAttributes_Application", "Code", "ApplicationCode", IsForeignKey = false)]
    		public virtual ICollection<StaffAttribute> StaffAttributes
    		{get; set;}
    
    		}
    }
}
