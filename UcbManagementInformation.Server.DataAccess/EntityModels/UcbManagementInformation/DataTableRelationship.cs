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
    public partial class DataTableRelationship
    {
        #region Primitive Properties
    
        public virtual System.Guid Code
        {
            get;
            set;
        }
    
        public virtual string JoinInfo
        {
            get;
            set;
        }
    
        public virtual System.Guid DataTableFromCode
        {
            get { return _dataTableFromCode; }
            set
            {
                if (_dataTableFromCode != value)
                {
                    if (DataTable != null && DataTable.Code != value)
                    {
                        DataTable = null;
                    }
                    _dataTableFromCode = value;
                }
            }
        }
        private System.Guid _dataTableFromCode;
    
        public virtual System.Guid DataTableToCode
        {
            get { return _dataTableToCode; }
            set
            {
                if (_dataTableToCode != value)
                {
                    if (DataTable1 != null && DataTable1.Code != value)
                    {
                        DataTable1 = null;
                    }
                    _dataTableToCode = value;
                }
            }
        }
        private System.Guid _dataTableToCode;
    
        public virtual byte[] RowIdentifier
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
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
    
        public virtual DataTable DataTable1
        {
            get { return _dataTable1; }
            set
            {
                if (!ReferenceEquals(_dataTable1, value))
                {
                    var previousValue = _dataTable1;
                    _dataTable1 = value;
                    FixupDataTable1(previousValue);
                }
            }
        }
        private DataTable _dataTable1;
    
        public virtual ICollection<DataTableRelationshipJoin> DataTableRelationshipJoins
        {
            get
            {
                if (_dataTableRelationshipJoins == null)
                {
                    var newCollection = new FixupCollection<DataTableRelationshipJoin>();
                    newCollection.CollectionChanged += FixupDataTableRelationshipJoins;
                    _dataTableRelationshipJoins = newCollection;
                }
                return _dataTableRelationshipJoins;
            }
            set
            {
                if (!ReferenceEquals(_dataTableRelationshipJoins, value))
                {
                    var previousValue = _dataTableRelationshipJoins as FixupCollection<DataTableRelationshipJoin>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupDataTableRelationshipJoins;
                    }
                    _dataTableRelationshipJoins = value;
                    var newValue = value as FixupCollection<DataTableRelationshipJoin>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupDataTableRelationshipJoins;
                    }
                }
            }
        }
        private ICollection<DataTableRelationshipJoin> _dataTableRelationshipJoins;

        #endregion
        #region Association Fixup
    
        private void FixupDataTable(DataTable previousValue)
        {
            if (previousValue != null && previousValue.DataTableRelationships.Contains(this))
            {
                previousValue.DataTableRelationships.Remove(this);
            }
    
            if (DataTable != null)
            {
                if (!DataTable.DataTableRelationships.Contains(this))
                {
                    DataTable.DataTableRelationships.Add(this);
                }
                if (DataTableFromCode != DataTable.Code)
                {
                    DataTableFromCode = DataTable.Code;
                }
            }
        }
    
        private void FixupDataTable1(DataTable previousValue)
        {
            if (previousValue != null && previousValue.DataTableRelationships1.Contains(this))
            {
                previousValue.DataTableRelationships1.Remove(this);
            }
    
            if (DataTable1 != null)
            {
                if (!DataTable1.DataTableRelationships1.Contains(this))
                {
                    DataTable1.DataTableRelationships1.Add(this);
                }
                if (DataTableToCode != DataTable1.Code)
                {
                    DataTableToCode = DataTable1.Code;
                }
            }
        }
    
        private void FixupDataTableRelationshipJoins(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (DataTableRelationshipJoin item in e.NewItems)
                {
                    item.DataTableRelationship = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (DataTableRelationshipJoin item in e.OldItems)
                {
                    if (ReferenceEquals(item.DataTableRelationship, this))
                    {
                        item.DataTableRelationship = null;
                    }
                }
            }
        }

        #endregion
    }
}
