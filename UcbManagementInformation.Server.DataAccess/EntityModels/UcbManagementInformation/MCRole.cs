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
    public partial class MCRole
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
    
        public virtual byte[] RowIdentifier
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ICollection<MCUserRole> UserRoles
        {
            get
            {
                if (_userRoles == null)
                {
                    var newCollection = new FixupCollection<MCUserRole>();
                    newCollection.CollectionChanged += FixupUserRoles;
                    _userRoles = newCollection;
                }
                return _userRoles;
            }
            set
            {
                if (!ReferenceEquals(_userRoles, value))
                {
                    var previousValue = _userRoles as FixupCollection<MCUserRole>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupUserRoles;
                    }
                    _userRoles = value;
                    var newValue = value as FixupCollection<MCUserRole>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupUserRoles;
                    }
                }
            }
        }
        private ICollection<MCUserRole> _userRoles;

        #endregion
        #region Association Fixup
    
        private void FixupUserRoles(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (MCUserRole item in e.NewItems)
                {
                    item.Role = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (MCUserRole item in e.OldItems)
                {
                    if (ReferenceEquals(item.Role, this))
                    {
                        item.Role = null;
                    }
                }
            }
        }

        #endregion
    }
}
