/*----------------------------------------------------------------------
Name: DataUtility

Description: Contains methods for data and file validation 
            
History:
--------
12 May 2005   1.00  LL	Genesis.
						Added IsValidFileName and IsValidDataType
24 May 2005	  1.01	KB	Code reviewed.
22 Jul 2005	  1.02	LL	TIR0224 - Allow various date format entered.
----------------------------------------------------------------------
Date last code reviewed: 24/05/2005 - KB
*/

using System;
using System.Collections;
using System.Reflection;
using System.Runtime.Serialization;

namespace UcbManagementInformation.Server.Utilities
{
	/// <summary>
	/// Class for handling data validations.
	/// </summary>
	public class DataUtility
	{
		private DataUtility()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region static methods

		/// <summary>
		/// Check if a particular file name is alphanumeric or has underscore
		/// </summary>
		/// <param name="checkedValue">true: valid file name; false: invalid</param>
		/// <returns></returns>
		public static bool IsValidFileName(string fileName)
		{
			// TODO: Code review issue 24/05/2005 - KB
			// All this can be done using 1-line regular expressions (^\w(\w|\s)*$)
			bool IsValid = false;
			string validString = "abcdefghijklmnopqrstuvwxyz0123456789_ ";
			foreach (char CurrentChar in fileName.ToLower().ToCharArray())
			{
				if (validString.IndexOf(CurrentChar.ToString(),0) == -1)
				{
					IsValid = false;
					break;
				}
                
				//if we come here, the string is valid
				IsValid = true;
			}
			return IsValid;
		}

		/// <summary>
		/// Check if a value matches with the given data type.
		/// </summary>
		/// <param name="value">The value of which type to be checked upon.</param>
		/// <param name="dataType">The data type the value should match to.</param>
		/// <returns>true: valid; false: invalid</returns>
		public static bool IsValidDataType(object dataValue, string dataType)
		{
			bool ReturnValue = true;
			FormatterConverter DataFormatter = new FormatterConverter();
			//Check if the value supplied can be converted to the given data type.
			//if not or if the data type is not defined in the following checks,
			//then invalidate the data value.
			try
			{
				switch (dataType.ToLower() )
				{
					case "char":
						DataFormatter.ToChar(dataValue);
						break;
					case "varchar":
						break;
					case "int":
						DataFormatter.ToInt32(dataValue);
						break;
					case "decimal":
					// TODO: Code review issue 24/05/2005 - KB
					// floats not allowed as datatypes
					case "float":
					case "money":
						DataFormatter.ToDecimal(dataValue);
						break;
					case "datetime":
						//22/07/05 LL - TIR0224 Allow various valid date formats
						ReturnValue = DateUtility.IsValidDate(dataValue.ToString());
						break;
					default:
						ReturnValue = false;
						break;
				}
			}
			catch (Exception e)
			{
				string Error = e.ToString();
				ReturnValue = false;
			}

			return ReturnValue;
		}

		#endregion
	}
}

