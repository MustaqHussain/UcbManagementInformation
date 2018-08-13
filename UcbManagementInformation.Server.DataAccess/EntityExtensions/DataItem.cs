using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class DataItem
    {
        public int GroupingLevel { get; set; }
        public bool IsGroupIDPresent { get; set; }
        public bool IsField { get; set; }
        #region Overriden methods

        /// <summary>
        /// Determines if this DataTable is equal to the supplied DataTable by comparing
        /// the DataTable codes.
        /// </summary>
        /// <param name="obj">The DataTable to compare this to.</param>
        /// <returns>Whether this object is equal to the supplied DataTable.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DataItem)
            {
                DataItem ThisDataItem = (DataItem)obj;

                return this.Code == ThisDataItem.Code;
            }
            else
            {
                return base.Equals(obj);
            }
        }

        /// <summary>
        /// Overrides the default hashing method.
        /// </summary>
        /// <returns>The hash code of the DataTable code.</returns>
        public override int GetHashCode()
        {
            return Code.GetHashCode();
        }
        #endregion
    }
}
