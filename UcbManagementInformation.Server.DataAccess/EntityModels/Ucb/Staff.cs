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
    public partial class Staff
    {
        #region Primitive Properties
    
        public virtual System.Guid Code
        {
            get;
            set;
        }
    
        public virtual System.Guid SecurityLabel
        {
            get;
            set;
        }
    
        public virtual string StaffNumber
        {
            get;
            set;
        }
    
        public virtual string LastName
        {
            get;
            set;
        }
    
        public virtual string FirstName
        {
            get;
            set;
        }
    
        public virtual System.Guid GradeCode
        {
            get;
            set;
        }
    
        public virtual bool IsActive
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
    
        public virtual ICollection<StaffAttribute> StaffAttributes
        {
            get
            {
                if (_staffAttributes == null)
                {
                    var newCollection = new FixupCollection<StaffAttribute>();
                    newCollection.CollectionChanged += FixupStaffAttributes;
                    _staffAttributes = newCollection;
                }
                return _staffAttributes;
            }
            set
            {
                if (!ReferenceEquals(_staffAttributes, value))
                {
                    var previousValue = _staffAttributes as FixupCollection<StaffAttribute>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupStaffAttributes;
                    }
                    _staffAttributes = value;
                    var newValue = value as FixupCollection<StaffAttribute>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupStaffAttributes;
                    }
                }
            }
        }
        private ICollection<StaffAttribute> _staffAttributes;

        #endregion
        #region Association Fixup
    
        private void FixupStaffAttributes(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (StaffAttribute item in e.NewItems)
                {
                    item.Staff = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (StaffAttribute item in e.OldItems)
                {
                    if (ReferenceEquals(item.Staff, this))
                    {
                        item.Staff = null;
                    }
                }
            }
        }

        #endregion
    }
}
