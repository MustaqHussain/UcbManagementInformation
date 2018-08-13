/*----------------------------------------------------------------------
Name: DateUtility

Description: Contains methods for getting current date and 
			 handling date formatting and validation 
            
History:
--------
29 Jun 2005   1.00    LL  Genesis.
25 Aug 2005	  1.01	  LL  Added overloaded FormatDate method
						  Added GetCurrentYearMonth method
 * IB did something diffferent
 * changed JA test
 * Change DH test 1
 * Ian did another test
----------------------------------------------------------------------
*/

using System;
using System.Runtime.Serialization;

namespace UcbManagementInformation.Server.Utilities
{
	/// <summary>
	/// Class for handling date.
	/// </summary>
	public class DateUtility
	{
		private DateUtility()
		{
			//
			// TODO: Add constructor logic here
			//
		}

		#region Constants

		private const string DATETIME_FORMAT = @"dd MMM yyyy HH:mm:ss";
		private const string DATE_FORMAT = @"dd MMM yyyy";
		private const string TIME_FORMAT = @"HH:mm:ss";
		private const string YEAR_FORMAT = @"yyyy";
		private const string YYYYMM_FORMAT = @"yyyyMM";

		#endregion

		#region static methods

		/// <summary>
		/// Format date to an English datetime in the "dd MMM yyyy HH:ss" format
		/// </summary>
		/// <param name="dateToFormat">The DateTime to be formated</param>
		/// <returns>string containing the formated datetime</returns>
		public static string FormatDate(DateTime dateToFormat)
		{
			return FormatDate(dateToFormat, DATETIME_FORMAT);
		}

		/// <summary>
		/// Format date to an English datetime in the given format
		/// </summary>
		/// <param name="dateToFormat">The DateTime to be formated</param>
		/// <param name="formatType">The format into which the datetime to be converted</param>
		/// <returns>string containing formated datetime</returns>
		public static string FormatDate(DateTime dateToFormat, string formatType)
		{
			string FormatedDate = "";

			// Set the culture to GB English
			IFormatProvider FormatProvider =	new System.Globalization.CultureInfo("en-GB", true);

			//Convert date to string using specified English datetime format
			try
			{	
				FormatedDate = dateToFormat.ToString(formatType, FormatProvider);
			}
			catch(FormatException)
			{
				// Invalid date entered by user
			}

			return FormatedDate;
		}

		/// <summary>
		/// Return the current year and month in the "yyyyMM" format
		/// </summary>
		public static string GetCurrentYearMonth()
		{
			return FormatDate(DateTime.Now, YYYYMM_FORMAT);
		}

		/// <summary>
		/// Return the current datetime in the "dd MMM yyyy HH:ss" format
		/// </summary>
		/// <returns>string containing the formatted current datetime</returns>
		public static string GetCurrentDateTime()
		{
			return FormatDate(DateTime.Now);
		}

		/// <summary>
		/// Validate the date string supplied can be converted to a date.
		/// </summary>
		/// <param name="dateToValidate">Date to be validated</param>
		/// <returns>true: valid date, false: invalid date</returns>
		public static bool IsValidDate(string dateToValidate)
		{
			bool IsValid = false;

			if(dateToValidate.Trim().Length == 0) return IsValid;

			// Set the culture to GB English
			IFormatProvider FormatProvider = new System.Globalization.CultureInfo("en-GB", true);

			//Convert date to string using specified English datetime format
			try
			{	
				dateToValidate = DateTime.Parse(dateToValidate, FormatProvider).ToString(DATE_FORMAT,FormatProvider);
				IsValid = true;
			}
			catch(FormatException)
			{
				// Invalid date entered by user
			}

			return IsValid;
		}

		/// <summary>
		/// Check if the year part is between 1900 and 2100
		/// </summary>
		/// <param name="yearToValidate"></param>
		/// <returns></returns>
		public static bool IsValidYear(string yearToValidate)
		{
			bool IsValid = false;

			if(yearToValidate == null || yearToValidate.Trim().Length == 0)
				return IsValid;

			try
			{	
				int ValidatedYear = int.Parse(yearToValidate);
				if (ValidatedYear >= 1900 && ValidatedYear <= 2100)
				{
					IsValid = true;
				}
			}
			catch(FormatException)
			{
				// Invalid date entered by user
			}

			return IsValid;
		}

		#endregion
	}
}

