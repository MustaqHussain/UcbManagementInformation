using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class DataTable
    {
        #region Overriden methods

        /// <summary>
        /// Determines if this DataTable is equal to the supplied DataTable by comparing
        /// the DataTable codes.
        /// </summary>
        /// <param name="obj">The DataTable to compare this to.</param>
        /// <returns>Whether this object is equal to the supplied DataTable.</returns>
        public override bool Equals(object obj)
        {
            if (obj is DataTable)
            {
                DataTable ThisDataTable = (DataTable)obj;

                return this.Code == ThisDataTable.Code;
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
