/*----------------------------------------------------------------------
Name:			FieldItem

Description:	Represents a field within an SQL query.

History:
--------
09 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Reviewed.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05
using System;
using System.Text;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Represents a field within an SQL query.
	/// </summary>
    public class FieldItem 
	{
		#region Private data members

		// Name of this field
		private string _name;

		// Name of the table to which this field belongs
		private string _tableName;

        // The name of the schema of the table to which this field belongs
        private string _tableSchemaName;

		// Caption this field is selected as
		private string _caption;

		#endregion

		/// <summary>
		/// Constructs a new FieldItem.
		/// </summary>
		/// <param name="name">Name of this field.</param>
		/// <param name="tableName">Name of the table to which this field belongs.</param>
		/// <param name="caption">Caption this field is selected as.</param>
		public FieldItem(string name, string tableName, string caption)
		{
			_name      = name;
			_tableName = tableName;
			_caption   = caption;
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes this field item render itself as a field in a select clause 
		/// to an XmlWriter instance.
		/// </summary>
		/// <returns>String of RDL that represents this object.</returns>
		public string Render()
		{
            return String.Format("[{0}].[{1}] AS '{2}'", _tableName, _name, _caption);
		}


        
    }
}
