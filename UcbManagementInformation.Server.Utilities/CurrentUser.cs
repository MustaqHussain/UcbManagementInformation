/*----------------------------------------------------------------------
Name:			CurrentUser

Description:	Offers a collection of methods for manipulating the
				current user of the system.
             
History:
--------
19 Apr 2005		1.00 KB		Created with GetUserName().
-----------------------------------------------------------------------*/

using System;
using System.Web;
using System.Security.Principal;

namespace UcbManagementInformation.Server.Utilities
{
	/// <summary>
	/// Offers a collection of methods for manipulating the
	/// current user of the system.
	/// </summary>
	public class CurrentUser
	{
		private CurrentUser()
		{
			// Private to prevent instantiation.
		}

		public static string GetUserName()
		{
			string CurrentFullUserName = "";

			if (HttpContext.Current == null)
			{
				// HttpContext.Current is null if execution is occurring locally, so
				// we require the local username
				CurrentFullUserName = WindowsIdentity.GetCurrent().Name;
			}
			else
			{
				// Otherwise, the execution is within the webservice, so we must get
				// the username via the HttpContext.
				CurrentFullUserName = HttpContext.Current.User.Identity.Name;
			}

			// The user name is in the form DOMAIN\USERNAME, so we must remove the
			// DOMAIN part from it.
			string[] UserNameArray = CurrentFullUserName.Split('\\');

			// The username is the last element of the full user name, and therefore
			// the array also.
			string CurrentUserName = UserNameArray[UserNameArray.Length - 1];			

			return CurrentUserName;
		}			

	}
}
