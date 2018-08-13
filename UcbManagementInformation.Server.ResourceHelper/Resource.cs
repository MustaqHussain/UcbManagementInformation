/*----------------------------------------------------------------------
Name: 		 Resource

Description: Implements the functions to retrieve resource text for the MIS 
			 system. Used in all layers of the system.
			

History:
--------
01 Mar 2005   1.00  DMH Genisis
03 Mar 2005   1.01  DMH Modified GetString method
27 Apr 2005	  1.02  DMH Amended to put New line functionality after check for null.
---------------------------------------------------------------------- */
using System;
using System.Resources;
using System.Reflection;

namespace UcbManagementInformation.Server.ResourceHelper
{
	/// <summary>
	/// Summary description for Resource.
	/// </summary>
	public sealed class Resource
	{
		#region Private Static Resource Manager Definition
		/// <summary>
		/// Resource Manager for localized text.
		/// </summary>
		private static ResourceManager resourceManager = new ResourceManager(typeof(Resource).Namespace + ".ResourceHelperText",Assembly.GetAssembly(typeof(Resource)));

		#endregion

		/// <summary>
		/// Private constructor to restrict an instance of this class from being created.
		/// </summary>
		private Resource()
		{
		}
		
		/// <summary>
		/// Returns a String from the Resource Helper
		/// </summary>
		/// <param name="name"></param>
		/// <returns></returns>
		public static string GetString(string name)
		{
			
			// Validate the passed paramters
			if (name == null)            
			{
				throw new ArgumentNullException("name");
			}

			string ResourceString = resourceManager.GetString(name);

			if (ResourceString == null)
			{
				throw new Exception("Resource string " + name + " does not exist.");
			}
			//Workaround to allow multi line tool tips.
			ResourceString = ResourceString.Replace(@"\r\n", Environment.NewLine);
			return ResourceString;
		}

	}
}
