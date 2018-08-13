/*----------------------------------------------------------------------
Name:			Loop

Description:	Implements a loop in SQL that sets the grouping level
				for each selected field.

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
	/// Implements a loop in SQL that sets the grouping level
	/// for each selected field.
	/// </summary>
	public class Loop
	{
        private List<DataItem> _groups;
        private List<DataItem> _fields;
		private List<SetGroup> _setGroups;

		private TempInsert _tempInsert;

		/// <summary>
		/// Constructs a new Loop object.
		/// </summary>
		/// <param name="fieldList">The selected data items.</param>
		/// <param name="groupList">The data items being grouped by.</param>
        public Loop(List<DataItem> fieldList, List<DataItem> groupList)
		{
			_fields    = fieldList;
			_groups    = groupList;
            _setGroups = new List<SetGroup>();

			int i = 1;//_groups.Count ;
			foreach (DataItem CurrentGroup in _groups)
			{
				_setGroups.Add(new SetGroup(CurrentGroup.Name + CurrentGroup.Code.ToString().Replace("-", ""), i));
				i++;//--;
			}

			_tempInsert = new TempInsert(fieldList);
		}


		/// <summary>
		/// Renders this object as a SQL command.
		/// </summary>
		/// <returns>The SQL query.</returns>
		public string Render()
		{
			StringBuilder sqlString = new StringBuilder();

			sqlString.Append("OPEN curAddGroupingLevel FETCH NEXT FROM curAddGroupingLevel INTO ");

            foreach (DataItem CurrentField in _fields)
            {
                sqlString.Append("@" + CurrentField.Name + CurrentField.Code.ToString().Replace("-", "") + "Holder, ");
                if (CurrentField.IsLink)
                {
                    sqlString.Append("@" + CurrentField.LinkAssociatedField + CurrentField.Code.ToString().Replace("-", "") + "Holder, ");
                }
            }
			// Remove last comma
			sqlString.Remove(sqlString.Length - 2, 2);

			sqlString.Append(" WHILE (@@fetch_status <> -1) BEGIN ");
			sqlString.Append(" IF(@@fetch_status <> -2) BEGIN ");
			sqlString.Append(" SET @GroupingLevelHolder = " + (_groups.Count + 1) + " ");

			// Need to render these SetGroups in reverse order (i.e. more detailed first)
            List<SetGroup> ReversedSetGroups = new List<SetGroup>(_setGroups);
			ReversedSetGroups.Reverse();

			foreach (SetGroup CurrentSetGroup in ReversedSetGroups)
				sqlString.Append(CurrentSetGroup.Render() );

			sqlString.Append(" END ");
			sqlString.Append(_tempInsert.Render() );

			foreach (DataItem CurrentGroup in _groups)
            {
                sqlString.Append(" SET @" + CurrentGroup.Name + CurrentGroup.Code.ToString().Replace("-", "") + "Group = @" + CurrentGroup.Name + CurrentGroup.Code.ToString().Replace("-", "") + "Holder ");
            } 
			sqlString.Append(" FETCH NEXT FROM curAddGroupingLevel INTO ");


            foreach (DataItem CurrentField in _fields)
            {
                sqlString.Append("@" + CurrentField.Name + CurrentField.Code.ToString().Replace("-", "") + "Holder, ");
                if (CurrentField.IsLink)
                {
                    sqlString.Append("@" + CurrentField.LinkAssociatedField + CurrentField.Code.ToString().Replace("-", "") + "Holder, ");
                }
            }
			// Remove last comma
			sqlString.Remove(sqlString.Length - 2, 2);

			sqlString.Append(" END CLOSE curAddGroupingLevel ");

			return sqlString.ToString();
		}
				
	}
}
