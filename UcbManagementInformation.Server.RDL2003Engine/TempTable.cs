/*----------------------------------------------------------------------
Name:			TempTable

Description:	Implements a temporary table used in the QueryWrapper
				solution to the subtotaling at various levels problem.

History:
--------
09 Jun 2005		1.00 KB		Created.
---------------------------------------------------------------------- */
using System;
using System.Xml;
using System.Text;
using System.Collections;
//using Dwp.Esf.Mis.BusinessObjects;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Implements a temporary table used in the QueryWrapper
	/// solution to the subtotaling at various levels problem.
	/// </summary>
	public class TempTable
	{
		// Selected fields on the report.
		private List<DataItem> _fieldList;

		/// <summary>
		/// Constructs a new TempTable for the supplied fields.
		/// </summary>
        public TempTable(List<DataItem> fieldList)
		{
            _fieldList = fieldList;
		}


		/// <summary>
		/// Renders this object as a SQL query to create it as a new table including all fields
		/// in the field list.
		/// </summary>
		/// <returns>A string containing the create table SQL query.</returns>
		public string Render()
		{
			StringBuilder sqlString = new StringBuilder();

			sqlString.Append("IF NOT EXISTS (SELECT [name] FROM tempdb.dbo.sysobjects ");
			sqlString.Append("WHERE [name] = '##tblGlobalTempExport') ");

			sqlString.Append(" BEGIN ");
			sqlString.Append(" CREATE TABLE [dbo].[##tblGlobalTempExport] (");

			foreach (DataItem CurrentField in _fieldList)
			{
                sqlString.Append("[" + CurrentField.Caption + "] " + CurrentField.DataType);
				if (CurrentField.DataType == "char" || CurrentField.DataType == "varchar" || CurrentField.DataType == "nchar" || CurrentField.DataType == "nvarchar") 
					sqlString.Append("(250) ");

				sqlString.Append(" NULL, ");
                if (CurrentField.IsLink)
                {
                    sqlString.Append("[" + CurrentField.LinkAssociatedField + "] " + "nvarchar");
                    sqlString.Append("(250) ");

                    sqlString.Append(" NULL, ");
                }
			}

			sqlString.Append("[GroupingLevel] INT NOT NULL ) END");

			return sqlString.ToString();
		}
	}
}
