/*----------------------------------------------------------------------
Name:			QueryBulder

Description:	This object builds an SQL query given a set of 
				parameters.

History:
--------
09 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
08 Jun 2005		1.03 KB		Refactored code into a new method ConstructTableJoins.
24 Jun 2005		1.04 KB		Actual 'tableJoinList' in OrderTableJoins() had its 
								REFERNCE changed, formal _tableJoinList did not see 
								the changes. The actual now retains its reference value 
								and so changes are seen by the formal parameter.
30 Jun 2005		1.05 LL		Set Ordering for Parameter Query
25 Aug 2005		1.06 LL		TIR0289(Change request)-Construct SQL for filter value = 'Current Month'
30 jan 2006     1.07 DS     Release 3
14 Feb 2006     3.01 KB     Extra parameter added for DataTableJoin constructor.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;
using System.Text;
using System.Collections;
//using Dwp.Esf.Mis.BusinessObjects;
//using Dwp.Esf.Mis.ResourceHelper;
//using Dwp.Esf.DataAccess.DataTransferManager;
//using Dwp.Esf.DataAccess.DataTransferObject;
//using Dwp.Esf.Mis.CrossLayer;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using UcbManagementInformation.Server.ResourceHelper;
using UcbManagementInformation.Server.Utilities;
using System.Linq.Expressions;
using System.Linq;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// This object builds an SQL query given a set of parameters.
	/// </summary>
	
	public class QueryBuilder : IDataModelAware
	{
		#region Private data members

		// List of fields used in the query
        private List<FieldItem> _fieldItemList = new List<FieldItem>();

		// List of TableJoins used in the query
        private List<TableJoin> _tableJoinList = new List<TableJoin>();

        // List of ReportTableJoins used in the query
        private List<ReportDataTableJoin> _joinList = new List<ReportDataTableJoin>();

		// List of ReportParameters used in the query
        private List<ParameterItem> _reportParameterList = new List<ParameterItem>();

		// List of FilterItems used in the query
        private List<FilterItem> _filterItemList = new List<FilterItem>();

		// List of items for the query to group by
        private List<string> _groupList = new List<string>();

		// The string of SQL that this QueryBuilder constructs.
		private StringBuilder _sqlString = new StringBuilder();

		// Denotes whether this query is used as a dataset query for <ValidValues>
		// tag in RDL (i.e. a parameter query) or is a query for data selection.
		private bool _isParameterQuery;

		private string _joinMethod;

        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		#region Constants
		//The value for Current Month passed in through filter value
		private const string CURRENTMONTH = "Current Month";
		#endregion

		/// <summary>
		/// This object builds an SQL query given a set of parameters.
		/// </summary>
		/// <param name="fieldList">List of fields used in the query</param>
		/// <param name="groupList">List of items for the query to group by</param>
		/// <param name="filterList">List of Filters used in the query</param>
		/// <param name="parameterList">List of parameters used in the query</param>
		/// <param name="isParameterQuery">Denotes whether this query is used as a 
		/// dataset query for a ValidValues tag in RDL (i.e. a parameter query) or 
		/// is a query for data selection.</param>
		/// <param name="joinMethod">Inner or Outer joins for tables</param>
        public QueryBuilder(List<DataItem> fieldList, List<DataItem> groupList,
            List<ReportFilterBusinessObject> filterList, List<DataItem> parameterList, bool isParameterQuery, List<ReportDataTableJoin> joinList,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;

			// Temporary list to store DataTables used in query
			List<DataTable> DataTableList = new List<DataTable>();

			_isParameterQuery = isParameterQuery;
			//_joinMethod=joinMethod;
			// Check that each of parameters to this constructor are non-null;
			// Initialise them if not.
			if (fieldList == null)
				fieldList = new List<DataItem>();
            if (groupList == null)
                groupList = new List<DataItem>();
			if (filterList == null)
				filterList = new List<ReportFilterBusinessObject>();
			if (parameterList == null)
				parameterList = new List<DataItem>();
            if (joinList == null)
                joinList = new List<ReportDataTableJoin>();
			// This loop adds all required fields to the fieldItem list and 
			// adds all required tables to the dataTable
			foreach (DataItem Item in fieldList)
			{
				// Retrieve DataTable
				DataTable DataTable = CurrentDataModel.DataTables.Single(x=>x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);

				_fieldItemList.Add(new FieldItem(Item.Name, DataTable.Name, Item.Caption));
                //If the systems' link field is in the query then add the associated field to the query.
                if (Item.IsLink)
                {
                    _fieldItemList.Add(new FieldItem(Item.LinkAssociatedField, DataTable.Name, Item.LinkAssociatedField));
                }

				if (! DataTableList.Contains(DataTable) )
					DataTableList.Add(DataTable);
			}

      
			// If there is more than one table in the DataTableList, we must construct
			// a set of joins covering all related tables
			if (DataTableList.Count > 1)
			{
				_tableJoinList = ConstructTableJoins(DataTableList,joinList,CurrentDataModel);
				OrderTableJoins(_tableJoinList);
			}
			else
			{
				DataTable dt = (DataTable) DataTableList[0];
				_tableJoinList.Add(new TableJoin(dt.SchemaName, null, dt.Name, null, null, null, -1, null, false ));
			}

			
			// Populate filterList
			foreach (ReportFilterBusinessObject CurrentFilter in filterList)
			{
				//TODO: Code Review Issue 11/05/05: Do not use 'my' in variable names. 
				//Choose a meaningful name. Pascal casing for variables.
				// Done 24/05/2005
                Guid CodeForRepository = Guid.Parse(CurrentFilter.DataItemCode);
                DataItem CurrentItem = CurrentDataModel.DataTables.SelectMany(x=>x.DataItems).Single(y=>y.Code == CodeForRepository);//DataAccessUtilities.RepositoryLocator<IDataItemRepository>().GetByCode(CodeForRepository);

                DataTable CurrentTable = CurrentDataModel.DataTables.Single(x => x.Code == CurrentItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(CurrentItem.DataTableCode);

				//25/08/05 LL - TIR0289 convert 'Current Month' to appropriate data format 
				if(CurrentFilter.FilterValue == CURRENTMONTH)
					CurrentFilter.FilterValue = DateUtility.GetCurrentYearMonth();

				_filterItemList.Add(new FilterItem(CurrentItem.Name, CurrentTable.Name, CurrentFilter.FilterValue,
					CurrentFilter.Operand, CurrentItem.DataType));
			}


			// Populate parameterList
			foreach (DataItem Item in parameterList)
			{
                DataTable CurrentTable = CurrentDataModel.DataTables.Single(x=>x.Code==Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
                _reportParameterList.Add(new ParameterItem(Item.Name, CurrentTable.Name, Item.DataType.Contains("char")));
			}


			// Populate groupList
			foreach (DataItem Item in groupList)
				_groupList.Add(Item.Caption);
		}

		//TODO: Code Review Issue 11/05/05: Missing XML summary.
		// Done 24/05/2005
		/// <summary>
		/// Causes this object to construct an SQL SELECT statement for its member data items,
		/// table joins, filters and parameters.
		/// </summary>
		/// <returns>A string containing the SQL query.</returns>
		public string Render()
		{       
			_sqlString.Append("SELECT "); // DISTINCT "); Cannot have distinct as we are losing data!

			// selected fields in query
			foreach (FieldItem CurrentFilterItem in _fieldItemList)
				_sqlString.Append(CurrentFilterItem.Render() + ", ");

			// Remove last comma
			_sqlString.Remove(_sqlString.Length - 2, 2);

			
			// selected tables in query
			_sqlString.Append(" FROM ");

			foreach (TableJoin tj in _tableJoinList)
				_sqlString.Append(tj.Render() );
			

			// Unless both lists are empty, add WHERE clause
			if ( !(_filterItemList.Count == 0 && _reportParameterList.Count == 0) )
			{
				_sqlString.Append(" WHERE ");

				// Records if this is the first item from the filterItemList
				// and/or parameterList, both of which are used constructing the
				// WHERE clause
				bool isFirstItem = true;

				foreach (FilterItem CurrentFilterItem in _filterItemList)
				{
					if (! isFirstItem)
						_sqlString.Append(" AND ");
					else
						isFirstItem = false;

					_sqlString.Append(CurrentFilterItem.Render() );
				}
				
				foreach (ParameterItem CurrentParameterItem in _reportParameterList)
				{
					if (! isFirstItem)
						_sqlString.Append(" AND ");
					else
						isFirstItem = false;

					_sqlString.Append(CurrentParameterItem.Render() );
				}
			}

			// If this is a query to set up a parameter list, we must add the option
			// for the user to select all parameters
			if (_isParameterQuery)
				_sqlString.Append(" UNION SELECT '<All>' ORDER BY 1");


			// ORDER BY clause
			if (_groupList.Count > 0)
			{
				_sqlString.Append(" ORDER BY ");

				foreach (string Caption in _groupList)
				{
					_sqlString.Append("'" + Caption + "', ");
				}

				// Remove final comma
				_sqlString.Remove(_sqlString.Length - 2, 2);
			}


			// final SQL string
			return _sqlString.ToString();
		}

        /// <summary>
        /// Constructs a set of DataTableJoin objects representing the joins necessary
        /// for joining the tables in the List. 
        /// </summary>
        /// <param name="tableCodeList">A list of Data Table codes to be joined</param>
        /// <returns>A list of DataTableJoin objects representing the joins for the supplied tables.</returns>
        public static List<DataTableJoin> RetreiveTableJoins(List<Guid> tableCodeList)
        {
            List<DataTableJoin> tableJoinList = new List<DataTableJoin>();
            // Holds a DataTableRelationship
            DataTableRelationship Dtr;

            // A DataTableJoin
            DataTableJoin Dtj;

            // A list of DataTableRelationshipJoins
            List<DataTableRelationshipJoin> DtrjList = new List<DataTableRelationshipJoin>();

            // j is the outer loop control variable: It is the index of the
            // current table in the list we are considering when comparing against
            // the rest of the tables.
            foreach (Guid currentTableOuterLoop in tableCodeList)// (int j = 0; j < tableList.Count; j++)
            {
                // i is the inner loop control variable: It is the index of
                // the table being compared against
                foreach (Guid currentTableInnerLoop in tableCodeList) //(int i = 0; i < tableList.Count; i++)
                {
                    // Ensure we don't compare table to itself
                    if (currentTableInnerLoop != currentTableOuterLoop)
                    {

                        // Get DataTableRelationship between tables j and i
                        Dtr = DataAccessUtilities.RepositoryLocator<IDataTableRelationshipRepository>().GetByTableFromAndTableTo(currentTableOuterLoop, currentTableInnerLoop);

                        // Get list of RelationshipJoins if there are Relationships
                        if (Dtr != null)
                            DtrjList = DataAccessUtilities.RepositoryLocator<IDataTableRelationshipJoinRepository>().GetByDataTableRelationshipCode(Dtr.Code);

                        // Add all unique RelationshipJoins to list of joins							
                        foreach (DataTableRelationshipJoin join in DtrjList)
                        {
                            Dtj = DataAccessUtilities.RepositoryLocator<IDataTableJoinRepository>().GetByCode(join.DataTableJoinCode);

                            // Do not add the new TableJoin to the list if it
                            // already exists in there
                            if (!tableJoinList.Contains(Dtj))
                                tableJoinList.Add(Dtj);
                        }
                    } // if (i != j)
                } // i loop
            } // j loop



            return tableJoinList;

        }

		/// <summary>
		/// Constructs a set of TableJoin objects representing the joins necessary
		/// for joining the tables in tableList. The joins are returned in a valid
		/// order within the list.
		/// </summary>
		/// <param name="tableList">A list of DataTable objects to be joined.</param>
		/// <param name="joinMethod">Join Method (inner or outer)</param>
		/// <returns>A list of TableJoin objects representing the joins for the supplied tables.
		/// </returns>
		public static List<TableJoin> ConstructTableJoins(List<DataTable> tableList, List<ReportDataTableJoin> joinList,DataModel currentDataModel)
		{
			// Temporary storage variables:
			// List of table joins for the query.
            List<TableJoin> tableJoinList = new List<TableJoin>();

			// Holds a DataTableRelationship
			DataTableRelationship Dtr;

			// A DataTableJoin
			DataTableJoin Dtj;

			// A list of DataTableRelationshipJoins
            List<DataTableRelationshipJoin> DtrjList = new List<DataTableRelationshipJoin>();

			// A TableJoin object
			TableJoin CurrentTableJoin;

				
			// j is the outer loop control variable: It is the index of the
			// current table in the list we are considering when comparing against
			// the rest of the tables.
			foreach(DataTable currentTableOuterLoop in tableList)// (int j = 0; j < tableList.Count; j++)
			{
				// i is the inner loop control variable: It is the index of
				// the table being compared against
				foreach(DataTable currentTableInnerLoop in tableList) //(int i = 0; i < tableList.Count; i++)
				{
					// Ensure we don't compare table to itself
					if (currentTableInnerLoop != currentTableOuterLoop)
					{
                        DtrjList = currentDataModel.DataTableJoins
                            .SelectMany(x => x.DataTableRelationshipJoins)
                            .Where(y => y.DataTableRelationship.DataTable.Name == currentTableOuterLoop.Name
                                        && y.DataTableRelationship.DataTable.SchemaName == currentTableOuterLoop.SchemaName
                                        && y.DataTableRelationship.DataTable1.Name == currentTableInnerLoop.Name
                                        && y.DataTableRelationship.DataTable1.SchemaName == currentTableInnerLoop.SchemaName).ToList();

						// Add all unique RelationshipJoins to list of joins							
						foreach (DataTableRelationshipJoin join in DtrjList)
						{
                            Dtj = join.DataTableJoin;//DataAccessUtilities.RepositoryLocator<IDataTableJoinRepository>().GetByCode(join.DataTableJoinCode);
                            ReportDataTableJoin AssociatedReportJoin = (from x in joinList where x.DataTableJoinCode == Dtj.Code select x).FirstOrDefault();
							// Construct a TableJoin - if this is the first TableJoin
							// in the tableJoinList, then .Count == 0 is true

                            /*ExceptionLogging try
                            {*/
                                string fromSchema = join.DataTableRelationship.DataTable.SchemaName;
                                string toSchema = join.DataTableRelationship.DataTable1.SchemaName;
                                CurrentTableJoin = new TableJoin(fromSchema, toSchema, Dtj.FromTable, Dtj.ToTable,
                                    Dtj.FromField, Dtj.ToField, -1, AssociatedReportJoin == null ? Dtj.DefaultJoinType : AssociatedReportJoin.JoinType,
                                    Dtj.IsOneToOne);
                            /*ExceptionLogging}
                            catch (Exception)
                            {
                                System.IO.File.WriteAllText("C:\\Releases\\Ucb MI\\MI_Error_Log2.txt", string.Format("Failed getting join info. from: '{0}', to: '{1}'", Dtj.FromTable, Dtj.ToTable));
                                throw;
                            }*/

							// Do not add the new TableJoin to the list if it
							// already exists in there
							if (! tableJoinList.Contains(CurrentTableJoin) )
								tableJoinList.Add(CurrentTableJoin);
						}
					} // if (i != j)
				} // i loop
			} // j loop

			

			return tableJoinList;
		}



		/// <summary>
		/// This method has two objectives:
		/// 1)	Ordering the TableJoins so that they are constructed in a valid order in
		///		the query.
		///	2)	Ensuring that none of the tables being joined on clash with the table
		///		being joined from (i.e. ... FROM t INNER JOIN t ... cannot happen)
		/// </summary>
		private static void OrderTableJoins(List<TableJoin> tableJoinList)
		{
			// List of tables already present in a join
            List<string> UsedTablesList = new List<string>();

			// Ordered list of table joins
            List<TableJoin> OrderedTableJoinList = new List<TableJoin>();

			// Each join has an index that determines its order within the query.
			// This keep count of the current order we are up to.
			int OrderIndex = 1;

			// The number of times we have looped around the do loop
			int LoopCount = 0;

			//TODO: Code Review Issue 11/05/05: Prefix 'Is' to boolean variable names. 
			// Done 24/05/2005
			// Determines if a table join that can be ordered has been found.
			bool IsTableJoinFound = false;

			if (tableJoinList.Count > 0)
			{
				// The first table in the list features the table that is being 
				// joined from, so order this join first and put the tables 
				// within this join into the used list.
				UsedTablesList.Add( ((TableJoin) tableJoinList[0]).FromTable );
				UsedTablesList.Add( ((TableJoin) tableJoinList[0]).ToTable );
				( (TableJoin) tableJoinList[0] ).Index = 0;
				OrderedTableJoinList.Add(tableJoinList[0]);

				/* We must order the rest of the joins in the tableJoinList:
				* Search for a tablejoin with no order asssigned to it; 
				* When found check if it contains a table that features in the used list;
				* If so, verify that the TO table is NOT in the used list (if it
				* is then you can swap the 'from' information and the 'to' information around 
				* because the 'from' table is guaranteed not to be used yet);
				* Add the table from this join that is not yet in the used list into
				* the used list;
				* Assign an order to the TableJoin;
				*/
				do
				{
					// Reset found indicator
					IsTableJoinFound = false;
					
					for (int i = 1; i < tableJoinList.Count; i++)
					{
						TableJoin CurrentTableJoin = (TableJoin) tableJoinList[i];
						bool FromTableUsed = UsedTablesList.Contains(CurrentTableJoin.FromTable);
						bool ToTableUsed   = UsedTablesList.Contains(CurrentTableJoin.ToTable);

						// Verify if a join is found without an order that has at
						// least one table in used list
						if (CurrentTableJoin.Index == -1 && (FromTableUsed || ToTableUsed) )
						{
							IsTableJoinFound = true;

							// Verify that only one table from the join has already
							// been used.
							if ( !(FromTableUsed && ToTableUsed) )
							{							
								// Verify 'to' table is not in used list
								if (UsedTablesList.Contains(CurrentTableJoin.ToTable) )
								{
									// Swap from and to
                                    string TempToSchema = CurrentTableJoin.ToSchema;
									string TempToTable = CurrentTableJoin.ToTable;
									string TempToField = CurrentTableJoin.ToField;

                                    CurrentTableJoin.ToSchema = CurrentTableJoin.FromSchema;
                                    CurrentTableJoin.FromSchema = TempToSchema;
									CurrentTableJoin.ToTable = CurrentTableJoin.FromTable;
									CurrentTableJoin.FromTable = TempToTable;
									CurrentTableJoin.ToField = CurrentTableJoin.FromField;
									CurrentTableJoin.FromField = TempToField;
								}

								// The table should now be the table to be assigned an order and added
								CurrentTableJoin.Index = OrderIndex;
								UsedTablesList.Add(CurrentTableJoin.ToTable);

								// The join should be added to the ordered list of joins
								OrderedTableJoinList.Add(CurrentTableJoin);
								OrderIndex++;
							}
							else
							{
								// Mark this join as unneccessary
								CurrentTableJoin.Index = -2;
							}
						
						}
					}

					//TODO: Code Review Issue 11/05/05: Replace the hard-coded message text by a Resource Helper Text.
					// This do-loop should only loop around as many times as there are
					// joins in the tableJoinList AT MOST. If we have looped around more
					// times than that, we have entered an infinite loop.
					if (LoopCount > tableJoinList.Count)
						throw new Exception(Resource.GetString("RES_EXCEPTION_QUERYBUILDER_INFINITELOOP"));

					LoopCount++;
				}
				while (IsTableJoinFound);

				// Now remove all unnecessary joins
				for (int i = 0; i < OrderedTableJoinList.Count; i++)
				{
					if ( ((TableJoin) OrderedTableJoinList[i]).Index == -2 )
						OrderedTableJoinList.RemoveAt(i);
				}


				// The member list of table joins can now be assigned the ordered joins
				tableJoinList.Clear();
				tableJoinList.AddRange(OrderedTableJoinList);
			}
		}
	}
}
