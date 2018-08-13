/*----------------------------------------------------------------------
Name:			SetGroup

Description:	<TO BE DONE>

History:
--------
09 Jun 2005		1.00 KB		Created.
---------------------------------------------------------------------- */
using System;
using System.Text;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// <TO BE DONE>
	/// </summary>
	public class SetGroup
	{
		private string _name;
		private int    _level;

		/// <summary>
		/// Constructs a new SetGroup with the given name and level.
		/// </summary>
		/// <param name="name">Name of this group.</param>
		/// <param name="level">The grouping level of this group.</param>
		public SetGroup(string name, int level)
		{
			_name  = name;
			_level = level;
		}

		/// <summary>
		/// Renders the SQL command to set the group levels.
		/// </summary>
		/// <returns>The SQL string.</returns>
		public string Render()
		{
			StringBuilder sqlString = new StringBuilder();

			sqlString.Append(" IF (@" + _name + "Group <> @" + _name + "Holder) BEGIN ");
			sqlString.Append(" SET @GroupingLevelHolder = " + _level + " END ");

			return sqlString.ToString();
		}

	}
}
