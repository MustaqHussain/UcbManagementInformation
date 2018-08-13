/*----------------------------------------------------------------------
Name:			TableJoin

Description:	Represents a join within an SQL query.

History:
--------
09 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
30 Jan 2006     1.03 DS     Release 3
14 Feb 2006     3.01 KB     Added IsOneToOne
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Text;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// A join within an SQL query.
	/// </summary>
	public class TableJoin 
	{
		#region Private data members

        // Schema names of joined tables
        private string _fromSchema;
        private string _toSchema;

		// Names of joined tables
		private string _fromTable;
		private string _toTable;

		// The joined fields of the involved tables
		private string _fromField;
		private string _toField;

		// This is the index of the join: Indices grow up from 0.
		private int _index;
		private string _joinMethod;

        // Denotes if this represents a one-to-one join
        private bool _isOneToOne;
		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new TableJoin.
		/// </summary>
		/// <param name="fromTable">First table involved in the join.</param>
		/// <param name="toTable">Second table involved in the join.</param>
		/// <param name="fromField">Field from the first table in the join.</param>
		/// <param name="toField">Field from the second table in the join.</param>
		/// <param name="isFirstTableJoin">Indicates that this is the first tablejoin if
		/// there is more than one table join involved.</param>
		/// <param name="joinMethod">Inner or Outer Joins for tables</param>
		/// <param name="isOneToOne">Denotes if this is a one to one join.</param>
		public TableJoin(string fromSchema, string toSchema, string fromTable, string toTable, string fromField, 
			string toField, int index, string joinMethod, bool isOneToOne)
		{
            if (!fromTable.StartsWith(fromSchema) || !toTable.StartsWith(toSchema))
            {
                System.Diagnostics.Debug.Print("");
            }

            _fromSchema = fromSchema;
			_fromTable	= fromTable;
            _fromField = fromField;
            _toSchema = toSchema;
			_toTable = toTable;			
			_toField = toField;
			_index = index;
            _isOneToOne = isOneToOne;
            switch (joinMethod)
            {
                case "INNER":
                    _joinMethod = "INNER JOIN";
                    break;
                case "FULL":
                    _joinMethod = "FULL OUTER JOIN";
                    break;
                case "LEFT":
                    _joinMethod = "LEFT JOIN";
                    break;
                case "RIGHT":
                    _joinMethod = "RIGHT JOIN";
                    break;

            }
		}

		#endregion

		#region Render method

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this object as a set of table join clauses for a FROM clause in an SQL
		/// query. 
		/// </summary>
		/// <returns>This TableJoin represented in RDL.</returns>
		public string Render()
		{
			string SqlString = "";
            

			if (_toField == null)
			{
                SqlString += String.Format("[{0}].[{1}]", _fromSchema, _fromTable);
			}
			else
			{
				if (_index == 0)
                    SqlString += String.Format("[{1}].[{2}] {0} [{4}].[{5}] ON [{1}].[{2}].[{3}] = [{4}].[{5}].[{6}]", _joinMethod, _fromSchema, _fromTable, _fromField, _toSchema, _toTable, _toField);
                else
                {
                    //Flip the left and right if not the first join in the query.
                    switch (_joinMethod)
                    {
                        case "LEFT JOIN":
                            _joinMethod = "RIGHT JOIN";
                            break;
                        case "RIGHT JOIN":
                            _joinMethod = "LEFT JOIN";
                            break;
                    }
                    SqlString += String.Format("{0} [{4}].[{5}] ON [{1}].[{2}].[{3}] = [{4}].[{5}].[{6}]", _joinMethod, _fromSchema, _fromTable, _fromField, _toSchema, _toTable, _toField);
				}
			}
			return SqlString;
		}
		#endregion

		#region Override methods

		//TODO: Code Review Issue 11/05/05: Missing XML summary
		/// <summary>
		/// Checks for equality between two TableJoin objects by comparing the 
		/// tables and fields within each TableJoin for equality. They are considered
		/// equal if both TableJoins have the same FromFields and ToFields.
		/// </summary>
		/// <param name="obj">The TableJoin this object is being compared to.</param>
		/// <returns>Whether the two objects are equal.</returns>
		public override bool Equals(object obj)
		{
			/* If the obj parameter is a TableJoin we must be specific in comparing
			 * the two objects: They can be considered equal if both from{Table|Field}
			 * fields are the same as their equivalent to{Table|Field} fields.
			*/

			if (obj is TableJoin)
			{
				//TODO: Code Review Issue 11/05/05: use Pascal casing for method variable.
				// Done 24/05/2005
				TableJoin TableJoinComparator = (TableJoin) obj;

				//TODO: Code Review Issue 11/05/05: Do not use abreviation in method variable.
				// Done 24/05/2005
				string FromInfoTableJoin1 = this._fromTable + this._fromField;
				string ToInfoTableJoin1   = this._toTable + this._toField;

				string FromInfoTableJoin2 = TableJoinComparator._fromTable + 
					TableJoinComparator._fromField;
				string ToInfoTableJoin2   = TableJoinComparator._toTable +
					TableJoinComparator._toField;


				if ((FromInfoTableJoin1 == FromInfoTableJoin2 && ToInfoTableJoin1 == ToInfoTableJoin2) ||
					(FromInfoTableJoin1 == ToInfoTableJoin2   && ToInfoTableJoin1 == FromInfoTableJoin2) )
					return true;
				else
					return false;
			}
			else // default
			{
				return base.Equals (obj);
			}
		}

		#endregion

		#region Properties

		public int Index
		{
			get { return _index; }
			set { _index = value; }
		}

        public string FromSchema
        {
            get { return _fromSchema; }
            set { _fromSchema = value; }
        }

        public string ToSchema
        {
            get { return _toSchema; }
            set { _toSchema = value; }
        }

		public string FromTable
		{
			get { return _fromTable; }
			set { _fromTable = value; }
		}

		public string ToTable
		{
			get { return _toTable; }
			set { _toTable = value; }
		}

		public string FromField
		{
			get { return _fromField; }
			set { _fromField = value; }
		}

		public string ToField
		{
			get { return _toField; }
			set { _toField = value; }
		}

		public string JoinMethod
		{
			get { return _joinMethod; }
			set { _joinMethod = value; }
		}

        public bool IsOneToOne
        {
            get { return _isOneToOne; }
            set { _isOneToOne = value; }
        }

		#endregion

	}
}
