/*----------------------------------------------------------------------
Name:			RdlFormatter

Description:	Centralises various rules that determine the
				formatting of information in an RDL file.

History:
--------
05 Apr 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
29 Jun 2005		1.03 LL		TIR0165 - Replaced Currency Formatter with a Custom Formatter.
21 Jul 2005		1.04 LL		TIR0200 - Modified FormatAlignment
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Centralises various rules that determine the formatting of 
	/// information in an RDL file.
	/// </summary>
	public class RdlFormatter
	{
		private RdlFormatter()
		{

		}

		/// <summary>
		/// Determines the alignment for a data item in an RDL Textbox type based on
		/// the provided data type.
		/// </summary>
		/// <param name="dataType">The type of the data to be aligned.</param>
		/// <returns>A string for the Alignment tag in an RDL Textbox type.</returns>
		public static string FormatAlignment(string dataType)
		{
			string Alignment = "";
			
			switch (dataType.ToLower() )
			{
				//21/07/05 LL - TIR0200 added data type of decimal and datetime
				case "int":
				case "decimal":
				case "money":
				case "datetime":
					Alignment = "Right";
					break;
				default:
					Alignment = "Left";
					break;
			}

			return Alignment;
		}

		/// <summary>
		/// Creates a string for the Format tag of a Style type in an
		/// RDL file, depending on the data type. Integers and 
		/// string values are unaffected. Real values are displayed
		/// to 2 decimal places. Dates are in the form dd/MM/yyyy.
		/// </summary>
		/// <param name="dataType">Data type of value to be formatted.</param>
		/// <returns>A string for the Format tag.</returns>
		public static string FormatData(string dataType)
		{
			return FormatData(dataType, 2);
		}
		
		//TODO: Code Review Issue 11/05/05: no description for the second parameter and returns tags.
		// Done 24/05/2005
		/// <summary>
		/// Creates a string for the Format tag of a Style type in an
		/// RDL file, depending on the data type. Integers and 
		/// string values are unaffected. Real values are displayed
		/// to a specified number of decimal places. 
		/// Dates are in the form dd/MM/yyyy.
		/// </summary>
		/// <param name="dataType">Data type of value to be formatted.</param>
		/// <param name="decimalPlaces">Number of decimal places to format to, if the data
		/// is of a suitable numeric type.</param>
		/// <returns>The format string for this data type.</returns>
		public static string FormatData(string dataType, int decimalPlaces)
		{
			//TODO: Code Review Issue 11/05/05: Use a varible to capture the return value
			//and return it at the end of method.
			// Acknowledged 24/05/2005
            
			switch (dataType.ToLower() )
			{
				case "int":
				case "char":
				case "varchar":
                case "nchar":
                case "nvarchar":
					// do nothing
					return "";
				case "decimal":
					return "F" + decimalPlaces;
				case "money":
					//29/06/05 LL - TIR0165 Replaced Currency format to a custom format
					//as Euro fields cannot have � sign.
					return "#,##0.00";
				case "datetime":
                case "date":
                    return "dd/MM/yyyy";
			}

			return "";
		}

	}
}
