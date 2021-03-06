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
    public partial class DataItem
    {
        #region Primitive Properties
    
        public virtual System.Guid Code
        {
            get;
            set;
        }
    
        public virtual string Name
        {
            get;
            set;
        }
    
        public virtual string DataType
        {
            get;
            set;
        }
    
        public virtual string Caption
        {
            get;
            set;
        }
    
        public virtual string Description
        {
            get;
            set;
        }
    
        public virtual bool IsGroup
        {
            get;
            set;
        }
    
        public virtual string LocationOnSystem
        {
            get;
            set;
        }
    
        public virtual bool IsValueType
        {
            get;
            set;
        }
    
        public virtual bool IsSummable
        {
            get;
            set;
        }
    
        public virtual bool IsCommonTableGrouping
        {
            get;
            set;
        }
    
        public virtual System.Guid DataTableCode
        {
            get { return _dataTableCode; }
            set
            {
                if (_dataTableCode != value)
                {
                    if (DataTable != null && DataTable.Code != value)
                    {
                        DataTable = null;
                    }
                    _dataTableCode = value;
                }
            }
        }
        private System.Guid _dataTableCode;
    
        public virtual System.Guid DataCategoryCode
        {
            get { return _dataCategoryCode; }
            set
            {
                if (_dataCategoryCode != value)
                {
                    if (DataCategory != null && DataCategory.Code != value)
                    {
                        DataCategory = null;
                    }
                    _dataCategoryCode = value;
                }
            }
        }
        private System.Guid _dataCategoryCode;
    
        public virtual byte[] RowIdentifier
        {
            get;
            set;
        }
    
        public virtual bool IsLink
        {
            get;
            set;
        }
    
        public virtual string LinkAssociatedField
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual DataCategory DataCategory
        {
            get { return _dataCategory; }
            set
            {
                if (!ReferenceEquals(_dataCategory, value))
                {
                    var previousValue = _dataCategory;
                    _dataCategory = value;
                    FixupDataCategory(previousValue);
                }
            }
        }
        private DataCategory _dataCategory;
    
        public virtual DataTable DataTable
        {
            get { return _dataTable; }
            set
            {
                if (!ReferenceEquals(_dataTable, value))
                {
                    var previousValue = _dataTable;
                    _dataTable = value;
                    FixupDataTable(previousValue);
                }
            }
        }
        private DataTable _dataTable;
    
        public virtual ICollection<ReportItem> ReportItems
        {
            get
            {
                if (_reportItems == null)
                {
                    var newCollection = new FixupCollection<ReportItem>();
                    newCollection.CollectionChanged += FixupReportItems;
                    _reportItems = newCollection;
                }
                return _reportItems;
            }
            set
            {
                if (!ReferenceEquals(_reportItems, value))
                {
                    var previousValue = _reportItems as FixupCollection<ReportItem>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupReportItems;
                    }
                    _reportItems = value;
                    var newValue = value as FixupCollection<ReportItem>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupReportItems;
                    }
                }
            }
        }
        private ICollection<ReportItem> _reportItems;

        #endregion
        #region Association Fixup
    
        private void FixupDataCategory(DataCategory previousValue)
        {
            if (previousValue != null && previousValue.DataItems.Contains(this))
            {
                previousValue.DataItems.Remove(this);
            }
    
            if (DataCategory != null)
            {
                if (!DataCategory.DataItems.Contains(this))
                {
                    DataCategory.DataItems.Add(this);
                }
                if (DataCategoryCode != DataCategory.Code)
                {
                    DataCategoryCode = DataCategory.Code;
                }
            }
        }
    
        private void FixupDataTable(DataTable previousValue)
        {
            if (previousValue != null && previousValue.DataItems.Contains(this))
            {
                previousValue.DataItems.Remove(this);
            }
    
            if (DataTable != null)
            {
                if (!DataTable.DataItems.Contains(this))
                {
                    DataTable.DataItems.Add(this);
                }
                if (DataTableCode != DataTable.Code)
                {
                    DataTableCode = DataTable.Code;
                }
            }
        }
    
        private void FixupReportItems(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (ReportItem item in e.NewItems)
                {
                    item.DataItem = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (ReportItem item in e.OldItems)
                {
                    if (ReferenceEquals(item.DataItem, this))
                    {
                        item.DataItem = null;
                    }
                }
            }
        }

        #endregion
    }
}
