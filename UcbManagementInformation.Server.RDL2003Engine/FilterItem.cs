/*----------------------------------------------------------------------
Name:			FilterItem

Description:	Represents a filter within an SQL query.

History:
--------
09 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Reviewed.
24 May 2005		1.02 KB		Code review amendments.
27 May 2005		1.03 KB		Render enclosed in parentheses.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05
using System;
using System.Text;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Represents a filter within an SQL query.
	/// </summary>
	public class FilterItem
	{
		#region Private data members

		// Field that is being filtered
		private string _fieldName;

		// Table to which the field belongs
		private string _tableName;

		// Operand value used in the filter
		private string _value;

		// Operator used in the filter
		private string _operator;

		// Data type of the field being filtered
		private string _dataType;

		#endregion

		/// <summary>
		/// Constructs a new FilterItem.
		/// </summary>
		/// <param name="fieldName">Field that is being filtered.</param>
		/// <param name="tableName">Table to which the field belongs.</param>
		/// <param name="filterValue">Operand value used in the filter.</param>
		/// <param name="filterOperator">Operator used in the filter.</param>
		/// <param name="dataType">Data type of the field being filtered.</param>
		public FilterItem(string fieldName, string tableName, string filterValue, 
			string filterOperator, string dataType)
		{
			_fieldName = fieldName;
			_tableName = tableName;
            switch (filterOperator)
            {
                case "CONTAINS":
                    _value     = "%" + filterValue + "%";
                    _operator  = filterOperator = "LIKE";
                    break;
                case "STARTS":
                    _value = filterValue + "%";
                    _operator = filterOperator = "LIKE";
                    break;
                default:
                    _value     = filterValue;
                    _operator  = filterOperator == "ISNULL" ? "IS" : filterOperator;
                    break;
            }
           
			_dataType  = dataType;
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this object as a filter clause in a select query.
		/// </summary>
		/// <returns>String of RDL that represents this object.</returns>
		public string Render()
		{
			//TODO: Code Review Issue: Use Pascal style of casing for method variable.
			// Done 24/05/2005
			string SqlString = "(" + _tableName + ".[" + _fieldName + "] " + _operator + " ";

			// Write the value according to which data type it is
			switch (_dataType)
			{
				// Data types that require enclosing apostrophes
				case "char":
				case "varchar":
                case "nchar":
                case "nvarchar":
                case "datetime":
                case "date":
                case "time":
                case "timestamp":
				{
					//DBS Release 3 test error
					if (_value != "NULL")
					{
						SqlString += "'" + _value + "'";
					}
					else
					{
						SqlString +=  _value ;
					}
					break;
					
				}

				// Data types that may be used as is
				case "int":
				case "money":
				case "decimal":
				case "numeric":
				default:
				{
					SqlString += _value;
					break;
				}
			}

			return SqlString + ")";
		}

	}
}
