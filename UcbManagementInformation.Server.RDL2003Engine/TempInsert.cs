/*----------------------------------------------------------------------
Name:			TempInsert

Description:	Implements a SQL command that inserts values into a
				temporary table used in the grouping level solution.

History:
--------
09 Jun 2005		1.00 KB		Created.
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
	/// Implements a SQL command that inserts values into a
	/// temporary table used in the grouping level solution.
	/// </summary>
	public class TempInsert
	{
        private List<DataItem> _fields;

		/// <summary>
		/// Constructs a new TempInsert object.
		/// </summary>
		/// <param name="fieldList">List of selected data items to include within the query.</param>
		public TempInsert(List<DataItem> fieldList)
		{
			_fields = fieldList;
		}

		/// <summary>
		/// Renders this object as a SQL insert query.
		/// </summary>
		/// <returns>The SQL query as a string.</returns>
		public string Render()
		{
			StringBuilder sqlString = new StringBuilder();
			sqlString.Append("INSERT ##tblGlobalTempExport VALUES (");

            foreach (DataItem CurrentField in _fields)
            {
                sqlString.Append("@" + CurrentField.Name + CurrentField.Code.ToString().Replace("-", "") + "Holder, ");
                if (CurrentField.IsLink)
                {
                    sqlString.Append("@" + CurrentField.LinkAssociatedField + CurrentField.Code.ToString().Replace("-", "") + "Holder, ");
                }
            }
			sqlString.Append("@GroupingLevelHolder)");

			return sqlString.ToString();
		}
	}
}
