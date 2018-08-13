/*----------------------------------------------------------------------
Name:			QueryWrapper

Description:	Wraps the existing QueryBuilder object, enabling it to
				provide Reporting Services with the capability to
				subtotal at various levels of a report involving grouped
				data items.

History:
--------
08 Jun 2005		1.00 KB		Created.
22 Jul 2005		1.01 LL		TIR0222 - Modified InsertInOrder to 
							check if ItemList is empty
30 Jan 2006     1.02 DS     Release 3
14 Feb 2006     3.01 KB     Now handles one to one joins.
20 Feb 2006     3.02 KB     Amendment to one to one handling.
---------------------------------------------------------------------- */
using System;
using System.Text;
using System.Collections;
using System.Linq;
using System.Xml;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using System.Collections.Generic;
using UcbManagementInformation.Server.ResourceHelper;
//using Dwp.Esf.Mis.BusinessObjects;
//using Dwp.Esf.Mis.CrossLayer;
//using Dwp.Esf.Mis.ResourceHelper;
//using Dwp.Esf.DataAccess.DataTransferManager;
//using Dwp.Esf.DataAccess.DataTransferObject;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Wraps the existing QueryBuilder object, enabling it to provide 
	/// Reporting Services with the capability to subtotal at various 
	/// levels of a report involving grouped data items.
	/// </summary>
	public class QueryWrapper : IRdlRenderable,IDataModelAware
	{
		#region Private data items

		private List<DataItem> _fieldList;
        private List<DataItem> _groupList;
		private List<ReportFilterBusinessObject> _filterList;
        private List<DataItem> _parameterList;
        private List<ReportDataTableJoin> _joinList;

		private List<string> _queryParameterList;

		// Denotes if this object needs to wrap the QueryBuilder
		private bool _wrapperRequired;

		// The query for this report
		private Query _query;

		private TempTable  _tempTableObject;
		private Loop       _loopObject;
		private TempSelect _tempSelectObject;

		// A list of table names sorted in order of the database hierarchy
		private List<string> DataTableHierarchyList;

		// The list of grouping data items including those added by the wrapper
        private List<DataItem> _newGroupList;
		private string _joinMethod;

        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new QueryWrapper for the given parameters.
		/// </summary>
		/// <param name="fieldList">List of data items representing fields in the query.</param>
		/// <param name="groupList">List of data items used in the ORDER BY clause of the query.</param>
		/// <param name="filterList">List of filters used in the query.</param>
		/// <param name="parameterList">List of data items used as parameters in the query.</param>
        public QueryWrapper(List<DataItem> fieldList, List<DataItem> groupList, List<ReportFilterBusinessObject> filterList,
            List<DataItem> parameterList, bool isParameterQuery, List<ReportDataTableJoin> joinList,DataModel currentDataModel)//string joinMethod)
		{
            CurrentDataModel = currentDataModel;
			if (isParameterQuery)
			{
				// Wrapper not required
				_wrapperRequired = false;
				_query = new Query(fieldList, groupList, filterList, parameterList, isParameterQuery, null,CurrentDataModel);
				return;
			}

			_fieldList     = fieldList;
			_groupList     = groupList;
			_filterList    = filterList;
			_parameterList = parameterList;
			_joinList = joinList;
			_queryParameterList = new List<string>();

			// Includes tables that the selected items directly belong to.
            List<DataTable> DataTableList = new List<DataTable>();

			// Includes names of ALL tables that are involved in the joining.
            List<string> CompleteDataTableNameList = new List<string>();			

			// The lowst level table involved in the query.
			DataTable DetailTable = new DataTable();

			// Determine which is the lowest level table involved (the details table)
			// First we need all of the tables that the selected items belong to:
			foreach (DataItem Item in _fieldList)
			{
				// Retrieve DataTable
				DataTable DataTable = CurrentDataModel.DataTables.Single(x=>x.Code==Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);

				if (! DataTableList.Contains(DataTable) )
					DataTableList.Add(DataTable);
			}

            //ExceptionLogging System.IO.File.WriteAllText("C:\\Releases\\Ucb MI\\MI_Error_Log1.txt", string.Format("DataTableList = '{0}'", DataTableList.Select(t => t.Name).Aggregate((x, y) => x + "," + y)));

			// If there is only one table involved in the query, then DetailTable holds it
			if (DataTableList.Count == 1)
			{
				DetailTable = (DataTable) DataTableList[0];
			}
			else
			{
				/* 
				 * There is one and only one table at the lowest level in the set of joins. This
				 * table is therefore the only table to appear exclusively on the 'to' side of
				 * the joins only.
				 * Therefore, build a list of 'to' table names that appear in the joins, then remove
				 * all tables from it that appear as a 'from' table, leaving one table.
                 * One to one joins should not be included until after the detail table
                 * has been identified (one to one tables are child tables in our system
                 * but should not be treated as detail tables).
				 */
				List<TableJoin> TableJoinList = QueryBuilder.ConstructTableJoins(DataTableList,joinList,CurrentDataModel);
                List<TableJoin> OneToOneJoinList = new List<TableJoin>();
                List<TableJoin> OneToOneJoinRemovedList = new List<TableJoin>();
				
				foreach (TableJoin CurrentJoin in TableJoinList)
				{
                    // Build a list of all child tables
                    if (! CompleteDataTableNameList.Contains(CurrentJoin.ToTable))
						CompleteDataTableNameList.Add(CurrentJoin.ToTable);

                    // Build a list of one to one joins
                    if (CurrentJoin.IsOneToOne)
                        OneToOneJoinList.Add(CurrentJoin);
				}

				foreach (TableJoin CurrentJoin in TableJoinList)
				{
                    // Remove any child tables that appear as parent tables in other joins.
					if (CompleteDataTableNameList.Contains(CurrentJoin.FromTable))
						CompleteDataTableNameList.Remove(CurrentJoin.FromTable);
				}

                // This i is a failsafe against infinite loops.
                int i = 0;

                // In the complete table list we leave one child table from the one-to-one joins.
                while (CompleteDataTableNameList.Count > 1 && i < 50)
                {
                    foreach (TableJoin CurrentOneToOneJoin in OneToOneJoinList)
                    {
                        if ( CompleteDataTableNameList.Contains(CurrentOneToOneJoin.ToTable) )
                        {
                            OneToOneJoinRemovedList.Add(CurrentOneToOneJoin);
                            CompleteDataTableNameList.Remove(CurrentOneToOneJoin.ToTable);
                            break;
                        }
                    }
                    i++;
                }
                    

				// The remaining table is the detail table.
				if (CompleteDataTableNameList.Count != 1)
				{
					throw new Exception(Resource.GetString(
						"RES_EXCEPTION_QUERYWRAPPER_SINGLETABLENOTRETURNED"));
				}
				else
				{
                    string tableForFilter = CompleteDataTableNameList[0];
                    DetailTable = CurrentDataModel.DataTables.Single(x=>x.Name == tableForFilter);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().FirstOrDefault(x => x.Name == tableForFilter);
					DataTableHierarchyList = BuildTableHierarchy(DetailTable.Name);

                    // Now we can insert into the hierarchy the children of any one-to-one joins.
                    InsertOneToOnes(DataTableHierarchyList, OneToOneJoinRemovedList);
				}
			}
									

			/* 
			 * Each data item, i, must have its IsGroupIDPresent property set; this property
			 * is set to false if i is a summable item which belongs to a non-details
			 * table. If i meets these criteria, but it is also the case that a common table group 
			 * item has been selected from the same table as i, then the property is set to true, 
			 * because a group item has been chosen to distinguish between the data items.
			 */
			_wrapperRequired = false;

			foreach (DataItem CurrentField in _fieldList)
			{
				DataTable CurrentFieldsTable =
                    CurrentDataModel.DataTables.Single(x=>x.Code==CurrentField.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(CurrentField.DataTableCode);

				// If current field is summable and doesn't belong do the details table
				if (CurrentField.IsSummable && !CurrentFieldsTable.Equals(DetailTable))
				{
					_wrapperRequired = true;
					CurrentField.IsGroupIDPresent = false;

					// Go through each group item
					foreach (DataItem CurrentGroup in _groupList)
					{
						DataTable CurrentGroupsTable =
                            CurrentDataModel.DataTables.Single(x=>x.Code==CurrentGroup.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(CurrentGroup.DataTableCode);

						// if current group's table is also the current field's table and
						// the current group is a common table grouping
						if (CurrentGroupsTable.Equals(CurrentFieldsTable) &&
							CurrentGroup.IsCommonTableGrouping)
						{
							CurrentField.IsGroupIDPresent = true;
						}
					}
				}
			} // foreach



			if (! _wrapperRequired)
			{
				_query = new Query(_fieldList, _groupList, _filterList, _parameterList, isParameterQuery,_joinList,CurrentDataModel);
			}
			else
			{
                List<DataItem> _newFieldList = new List<DataItem>(_fieldList);
				_newGroupList = new List<DataItem>(_groupList);

				foreach (DataItem CurrentField in _fieldList)
				{
					if (CurrentField.IsSummable && !CurrentField.IsGroupIDPresent)
					{
						// The item with IsGroupIDPresent == true for this table needs adding to
						// the fieldlist and grouplist. Find it on the table the CurrentField
						// belongs to.
						DataTable CurrentTable =
                            CurrentDataModel.DataTables.Single(x=>x.Code==CurrentField.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(CurrentField.DataTableCode);
                        List<DataItem> CurrentTableItemList =
                            CurrentTable.DataItems.ToList();
                            //DataAccessUtilities.RepositoryLocator<IDataItemRepository>().GetByTableCode(CurrentTable.Code);

						foreach (DataItem CurrentTableItem in CurrentTableItemList)
						{
							if (CurrentTableItem.IsCommonTableGrouping)
							{
								// This needs adding to the fieldList and groupList (as a
								// business object)
                                //DataItem CurrentTableItemBO = (DataItem)
                                //    ObjectUtility.TransferLikeNamedProperties(CurrentTableItem,
                                //    typeof(DataItemBusinessObject));

                                if (!_newFieldList.Contains(CurrentTableItem))
                                    _newFieldList.Add(CurrentTableItem);

                                if (!_newGroupList.Contains(CurrentTableItem))
                                    InsertInOrder(CurrentTableItem, _newGroupList);									

								// We need only one of these fields, so we can safely break out
								break;
							}
						}
					}
				} // foreach

				// Now that the new group list has been compiled, each data item can have its
				// grouping level set
				foreach (DataItem CurrentField in _fieldList)
				{
					foreach (DataItem CurrentGroup in _newGroupList)
					{
						if (CurrentGroup.DataTableCode == CurrentField.DataTableCode &&
							CurrentGroup.IsCommonTableGrouping)
						{
							CurrentField.GroupingLevel = _newGroupList.IndexOf(CurrentGroup) + 1;
						}
					}
				}

				_query = new Query(_newFieldList, _newGroupList, _filterList, _parameterList, isParameterQuery,_joinList,CurrentDataModel);
				_tempTableObject  = new TempTable(_newFieldList);
				_loopObject       = new Loop(_newFieldList, _newGroupList);
				_tempSelectObject = new TempSelect(_fieldList, _groupList);

				// Set the member fieldList and groupList of this object to the new lists
				_fieldList = new List<DataItem>(_newFieldList);
                _groupList = new List<DataItem>(_newGroupList);
			} // if


			// Create the parameters for the query, if any
			if (parameterList != null)
			{
				foreach (DataItem Item in parameterList)
				{
					// To ensure query name is unique in case of data items with same name,
					// concatenate the tablename and itemname
                    DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x=>x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
					string TableName = ThisDataTable.Name;

					_queryParameterList.Add(TableName + Item.Name);
				}
			}
            _queryParameterList.Add("ReportUserCode");
		}

		#endregion

		#region Render method

		/// <summary>
		/// Stub.
		/// </summary>
		/// <returns></returns>
		public void Render(XmlWriter xmlWriter)
		{
			if (! _wrapperRequired)
			{
                _query.Render(xmlWriter);
			}
			else
			{
				xmlWriter.WriteStartElement("Query");
				RdlRender.AddLine( xmlWriter, "DataSourceName", "DataSource1");
				RdlRender.AddLine( xmlWriter, "CommandType", "Text");
				
				xmlWriter.WriteStartElement("CommandText");
				xmlWriter.WriteString(GetQueryText());
				xmlWriter.WriteEndElement(); //CommandText

				RdlRender.AddLine( xmlWriter, "Timeout", "120");

				// Each queryParameter must render itself
                if (_parameterList == null || _parameterList.Count > 0 || _queryParameterList.Count > 0)
				{
					xmlWriter.WriteStartElement("QueryParameters");
					foreach (string Item in _queryParameterList)
					{
						xmlWriter.WriteStartElement("QueryParameter");
						xmlWriter.WriteAttributeString("Name", "@" + Item);
						RdlRender.AddLine( xmlWriter, "Value", "=Parameters!" + 
							Item + ".Value");
						xmlWriter.WriteEndElement(); // QueryParameter
					}
					xmlWriter.WriteEndElement(); // QueryParameters
				}
			
				xmlWriter.WriteEndElement(); // Query
			}
		}

		#endregion

		#region Properties

		/// <summary>
		/// A list of grouping data items including those added by the wrapper.
		/// </summary>
		public List<DataItem> NewGroupList
		{
			get { return _newGroupList; }
		}

		/// <summary>
		/// The query used by this object.
		/// </summary>
		public string QueryText
		{
			get { return _query.QueryText; }
		}

		#endregion

		#region Private methods

		private string GetQueryText()
		{
			StringBuilder sqlString = new StringBuilder();

            sqlString.Append("EXECUTE [SetContextInfo] @ReportUserCode ");

            foreach (DataItem CurrentField in _fieldList)
			{
				sqlString.Append("DECLARE @" + CurrentField.Name + CurrentField.Code.ToString().Replace("-", "") + "Holder " + CurrentField.DataType + " ");
                if (CurrentField.DataType == "char" || CurrentField.DataType == "varchar" || CurrentField.DataType == "nchar" || CurrentField.DataType == "nvarchar") 
					sqlString.Append("(250) ");

                if (CurrentField.IsLink)
                {
                    sqlString.Append("DECLARE @" + CurrentField.LinkAssociatedField + CurrentField.Code.ToString().Replace("-", "") + "Holder " + "nvarchar" + " ");
                    sqlString.Append("(250) ");
                }
                
			}

			sqlString.Append(" DECLARE @GroupingLevelHolder INT ");

			foreach (DataItem CurrentGroup in _groupList)
			{
				sqlString.Append(" DECLARE @" + CurrentGroup.Name + CurrentGroup.Code.ToString().Replace("-", "") + "Group " + CurrentGroup.DataType + " ");
                if (CurrentGroup.DataType == "char" || CurrentGroup.DataType == "varchar" || CurrentGroup.DataType == "nchar" || CurrentGroup.DataType == "nvarchar") 
					sqlString.Append("(250) ");
				sqlString.Append(" SET @" + CurrentGroup.Name + CurrentGroup.Code.ToString().Replace("-", "") + "Group = ");
                switch (CurrentGroup.DataType)
                {
                    case "int":
                    case "decimal":
                    case "money":
                        sqlString.Append("0 ");
                        break;
                    default:
                        sqlString.Append("'' ");
                        break;
                }
                

				
			}

			sqlString.Append(_tempTableObject.Render() + " ");

			sqlString.Append(" DECLARE curAddGroupingLevel CURSOR FORWARD_ONLY READ_ONLY FOR ");

			// Extract the user's query from the Query object
			sqlString.Append(_query.QueryText + " ");

			sqlString.Append(_loopObject.Render() + " ");
			sqlString.Append(" DEALLOCATE curAddGroupingLevel ");
			sqlString.Append(_tempSelectObject.Render() );
			sqlString.Append(" DROP TABLE ##tblGlobalTempExport");

			return sqlString.ToString();
		}

		/// <summary>
		/// Builds a hierarchical path through the database graph structure, from the 
		/// lowest level table up to the highest level table. It is done in the style of a 
		/// breadth-first search so each element in the list is higher than or 
		/// at an equal level to any previous element only.
		/// </summary>
		/// <param name="startTableName">The lowest level table at which to begin the hierarchy.</param>
		/// <returns>A hierarchical list of tables.</returns>
		private List<string> BuildTableHierarchy(string startTableName)
		{
			// List of table names to be returned.
            List<string> TableList = new List<string>();

			// List to hold immediate joins pending traversal.
            List<DataTableJoin> JoinList = new List<DataTableJoin>();

			// Pointer to index of TableList; points to next table that has not yet been visited
			// (all its predecessors in the list have been visited).
			int index = 1;

			// Add first table, then get all joins to this table
			TableList.Add(startTableName);
            JoinList.AddRange(DataAccessUtilities.RepositoryLocator<IDataTableJoinRepository>().Find(x => x.ToTable == startTableName));

			// This loop continues whilst there are joins to any of the tables at the current level.
			while (JoinList.Count > 0)
			{
				// Add each table that joins to the current tables.
				foreach (DataTableJoin CurrentJoin in JoinList)
					TableList.Add(CurrentJoin.FromTable);

				JoinList.Clear();

				// Obtain the joins to all tables not yet visited.
				while (index < TableList.Count)
				{
                    string tableForFilter = TableList[index];
					JoinList.AddRange(CurrentDataModel.DataTableJoins.Where(x=>x.ToTable == tableForFilter));
 //                       DataAccessUtilities.RepositoryLocator<IDataTableJoinRepository>().Find(x => x.ToTable == tableForFilter));
					index++;
				}
			}

			// Reverse the list, so tables are in descending order
			TableList.Reverse();
			return TableList;
		}

		/// <summary>
		/// Inserts a data item into a list of data items ensuring that it is inserted
		/// into a position in accordance with the hierarchy of the database structure.
		/// Items are placed according to the table they belong to (before tables lower
		/// down the hierarchy and after tables higher up than it).
		/// </summary>
		/// <param name="dataItem">The data item to insert.</param>
		/// <param name="itemList">The list into which to insert the data item.</param>
		private void InsertInOrder(DataItem dataItem, List<DataItem> itemList)
		{
			//22/07/05 LL - TIR0222 exit if no item in list
			if(itemList.Count == 0) return;

			// Index of current item in item list being compared against.
			int ComparatorListItemIndex = 0;

			// Has item been inserted.
			bool InsertSuccessful = false;

			// Table name of item to be inserted.
			string DataItemTableName = CurrentDataModel.DataTables.Single(x=>x.Code== dataItem.DataTableCode).Name;//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(dataItem.DataTableCode).Name;

			// This loop continues until item has been inserted.
			while (! InsertSuccessful)
			{
				// Current item in the item list
				DataItem ComparatorListItem = 
					itemList[ComparatorListItemIndex];
				DataTable ComparatorListItemTable =
                    CurrentDataModel.DataTables.Single(x=>x.Code==ComparatorListItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(ComparatorListItem.DataTableCode);

				// Position of current table under consideration in the hierarchy list
				int HierarchyIndex = 0;

				// This loop continues until we reach the same table in the hierarchy
				// list as the comparator (we therefore know that the item to be inserted 
				// should not be inserted before this item).
				while (ComparatorListItemTable.Name != (string) DataTableHierarchyList[HierarchyIndex])
				{
					if (DataItemTableName == (string) DataTableHierarchyList[HierarchyIndex])
					{
						// We have reached the table to be inserted, so we can insert the 
						// data item at the index of the comparator.
						itemList.Insert(ComparatorListItemIndex, dataItem);
						InsertSuccessful = true;
						break;
					}
					HierarchyIndex++;
				}
                
				// Now we must check if we have reached the end of the list. If so, we can insert
				// the item at the end of the item list
				ComparatorListItemIndex++;
				if (ComparatorListItemIndex >= itemList.Count)
				{
					itemList.Insert(ComparatorListItemIndex, dataItem);
					InsertSuccessful = true;
				}
			}
		}

        private void InsertOneToOnes(List<string> hierarchyList, List<TableJoin> oneToOneList)
        {
            // Insert each child of a one-to-one directly after its parent in the
            // hierarchy list.

            foreach (TableJoin OneToOneJoin in oneToOneList)
            {
                // Index of current element in hierarchyList
                int i = 0;

                // Have we found the 'from' table for the this one to one join?
                bool FromFound = false;

                foreach (string TableName in hierarchyList)
                {
                    if (TableName.Equals(OneToOneJoin.FromTable) )
                    {
                        FromFound = true;
                        break;
                    }
                    else
                        i++;
                }

                // Insert to table name directly after its from table.
                if (FromFound)
                    hierarchyList.Insert(i + 1, OneToOneJoin.ToTable);
            }
        }

		#endregion

        public void Render2010(XmlWriter xmlWriter)
        {
            Render(xmlWriter);
        }
    }
}
