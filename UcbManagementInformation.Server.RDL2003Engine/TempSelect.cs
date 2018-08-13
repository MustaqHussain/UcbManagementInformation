/*----------------------------------------------------------------------
Name:			TempSelect

Description:	Implements a SQL command that returns the values from
				the temporary table created within the solution to the
				grouping solution.

History:
--------
09 Jun 2005		1.00 KB		Created.
06 Jul 2005		1.01 LL		TIR0150 Set condition for appending 'ORDER BY' in sql
							when group list is not empty
---------------------------------------------------------------------- */
using System;
using System.Text;
using System.Collections;
//using Dwp.Esf.Mis.BusinessObjects;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Implements a SQL command that returns the values from the temporary table 
	/// created within the solution to the grouping solution.
	/// </summary>
	public class TempSelect
	{
        private List<DataItem> _fieldList;
        private List<DataItem> _groupList;

        public TempSelect(List<DataItem> fieldList, List<DataItem> groupList)
		{
			_fieldList = fieldList;
			_groupList = groupList;
		}

		public string Render()
		{
			StringBuilder sqlString = new StringBuilder();
			sqlString.Append("SELECT ");

            foreach (DataItem CurrentField in _fieldList)
            {
                sqlString.Append("[" + CurrentField.Caption + "], ");
                if (CurrentField.IsLink)
                {
                    sqlString.Append("[" + CurrentField.LinkAssociatedField + "], ");
                }
            }
			sqlString.Append(" GroupingLevel FROM ##tblGlobalTempExport ");

			//06/07/05 LL - TIR0150 Only append 'ORDER BY' if group list is not empty
			if(_groupList.Count > 0)
			{
				sqlString.Append(" ORDER BY ");

				foreach (DataItem CurrentGroup in _groupList)
					sqlString.Append("[" + CurrentGroup.Caption + "], ");

				// Remove last comma
				sqlString.Remove(sqlString.Length - 2, 2);
			}

			return sqlString.ToString();
		}
	}
}
