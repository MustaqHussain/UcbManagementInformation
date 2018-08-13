/*----------------------------------------------------------------------
Name:			ParameterItem

Description:	Represents a parameter within an SQL query.

History:
--------
09 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
27 May 2005		1.03 KB		Render now enclosed in parentheses.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Text;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Represents a parameter within an SQL query.
	/// </summary>
	public class ParameterItem
	{
		#region Private data members

		// Name of the parameter's field
		private string _fieldName;

		// Name of the parameter's table
		private string _tableName;
        // Name of the parameter's table
        private Boolean _isString;

		#endregion

		/// <summary>
		/// Constructs a new ParameterItem.
		/// </summary>
		/// <param name="fieldName">Name of this parameter's field.</param>
		/// <param name="tableName">Name of this parameter's field.</param>
		public ParameterItem(string fieldName, string tableName, bool isString = true)
		{
			_fieldName = fieldName;
			_tableName = tableName;            
            _isString = isString;
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this object as a parameter clause in a select query.
		/// </summary>
		/// <returns>String of RDL that represents this object.</returns>
        public string Render()
        {
            // Data items require name of table they belong to in front of their names
            // to ensure uniqueness in case of two items with same name            
            if (_isString)
            {
                return "(" + _tableName + "." + _fieldName + " = " + "@" + _tableName + _fieldName +
                    " OR @" + _tableName + _fieldName + " = '<All>')";
            }
            else
            {
                return "(" + _tableName + "." + _fieldName + " = " + "@" + _tableName + _fieldName +
                    " OR (@" + _tableName + _fieldName + " IS NULL AND " + _tableName + "." + _fieldName + " IS NULL))";
            }
        }
	}
}
