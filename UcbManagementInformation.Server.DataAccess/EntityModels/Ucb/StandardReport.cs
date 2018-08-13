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
    public partial class StandardReport
    {
        #region Primitive Properties
    
        public virtual System.Guid Code
        {
            get;
            set;
        }
    
        public virtual string ReportName
        {
            get;
            set;
        }
    
        public virtual string ReportDescription
        {
            get;
            set;
        }
    
        public virtual System.Guid ReportCategoryCode
        {
            get { return _reportCategoryCode; }
            set
            {
                if (_reportCategoryCode != value)
                {
                    if (ReportCategory != null && ReportCategory.Code != value)
                    {
                        ReportCategory = null;
                    }
                    _reportCategoryCode = value;
                }
            }
        }
        private System.Guid _reportCategoryCode;
    
        public virtual int SortOrder
        {
            get;
            set;
        }
    
        public virtual string ReportUrl
        {
            get;
            set;
        }
    
        public virtual bool IsPrintable
        {
            get;
            set;
        }
    
        public virtual bool IsExportable
        {
            get;
            set;
        }
    
        public virtual byte[] RowIdentifier
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ReportCategory ReportCategory
        {
            get { return _reportCategory; }
            set
            {
                if (!ReferenceEquals(_reportCategory, value))
                {
                    var previousValue = _reportCategory;
                    _reportCategory = value;
                    FixupReportCategory(previousValue);
                }
            }
        }
        private ReportCategory _reportCategory;

        #endregion
        #region Association Fixup
    
        private void FixupReportCategory(ReportCategory previousValue)
        {
            if (previousValue != null && previousValue.StandardReports.Contains(this))
            {
                previousValue.StandardReports.Remove(this);
            }
    
            if (ReportCategory != null)
            {
                if (!ReportCategory.StandardReports.Contains(this))
                {
                    ReportCategory.StandardReports.Add(this);
                }
                if (ReportCategoryCode != ReportCategory.Code)
                {
                    ReportCategoryCode = ReportCategory.Code;
                }
            }
        }

        #endregion
    }
}
