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
    public partial class UploadJobQueue
    {
        #region Primitive Properties
    
        public virtual System.Guid Code
        {
            get;
            set;
        }
    
        public virtual int Position
        {
            get;
            set;
        }
    
        public virtual string Status
        {
            get;
            set;
        }
    
        public virtual System.DateTime TimeAdded
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> TimeStarted
        {
            get;
            set;
        }
    
        public virtual string JobDescription
        {
            get;
            set;
        }
    
        public virtual string UserId
        {
            get;
            set;
        }
    
        public virtual byte[] RowIdentifier
        {
            get;
            set;
        }
    
        public virtual Nullable<System.DateTime> EndTime
        {
            get;
            set;
        }
    
        public virtual string JobData
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ICollection<JobStep> JobSteps
        {
            get
            {
                if (_jobSteps == null)
                {
                    var newCollection = new FixupCollection<JobStep>();
                    newCollection.CollectionChanged += FixupJobSteps;
                    _jobSteps = newCollection;
                }
                return _jobSteps;
            }
            set
            {
                if (!ReferenceEquals(_jobSteps, value))
                {
                    var previousValue = _jobSteps as FixupCollection<JobStep>;
                    if (previousValue != null)
                    {
                        previousValue.CollectionChanged -= FixupJobSteps;
                    }
                    _jobSteps = value;
                    var newValue = value as FixupCollection<JobStep>;
                    if (newValue != null)
                    {
                        newValue.CollectionChanged += FixupJobSteps;
                    }
                }
            }
        }
        private ICollection<JobStep> _jobSteps;

        #endregion
        #region Association Fixup
    
        private void FixupJobSteps(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.NewItems != null)
            {
                foreach (JobStep item in e.NewItems)
                {
                    item.UploadJobQueue = this;
                }
            }
    
            if (e.OldItems != null)
            {
                foreach (JobStep item in e.OldItems)
                {
                    if (ReferenceEquals(item.UploadJobQueue, this))
                    {
                        item.UploadJobQueue = null;
                    }
                }
            }
        }

        #endregion
    }
}
