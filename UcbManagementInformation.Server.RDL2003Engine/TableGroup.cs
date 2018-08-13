/*----------------------------------------------------------------------
Name:			TableGroup

Description:	Generates a table group rendered in RDL for use in a
				TabularReport object.

History:
--------
01 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
17 Jun 2005		1.03 KB		TableGroup now takes the new grouplist of 
								the querywrapper as an argument and passes 
								it to the TableGroupTextBox.
09 Jan 2006		1.04 DM		Added isDataMapDisplayedGroup field.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;
using System.Linq;
using System.Collections;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.Generic;
using System.Configuration;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a table group rendered in RDL for use in a
	/// TabularReport object.
	/// </summary>
	public class TableGroup : IRdlRenderable,IDataModelAware
	{
		# region Private data members

		// Name of this TableGroup
		private string _name;
        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
		// Name of drill down group
		private string _drillDownGroup;

		// Name of DataMap group
		private bool _isDataMapDisplayedGroup;
        public bool IsDataMapDisplayedGroup
        {
            get { return _isDataMapDisplayedGroup; }
            set { _isDataMapDisplayedGroup = value; }
        }
		// Collection of TableGroupTextBox's in this object
        private List<TableGroupTextBox> _tableGroupTextBoxList = new List<TableGroupTextBox>();

        public DataModel CurrentDataModel { get; set; }
		

		#endregion
		
		
		#region Constructors

		/// <summary>
		/// Generates a table group rendered in RDL for use in a
		/// TabularReport object.
		/// </summary>
		/// <param name="fieldList">List of fields selected.</param>
		/// <param name="groupItems">List of fields to be grouped by.</param>
		/// <param name="drillDownGroup">Indicates if this is a drill down group.</param>
		/// <param name="groupByItem">A data item that groups other data items in the report.</param>
		/// <param name="dataItemsPromotedToGroup">Records data items that are moved from details
		/// level into the grouping.</param>
		/// <param name="isDataMapDisplayedGroup">Indicates if this is a is data map displayed.</param>
        public TableGroup(List<DataItem> fieldList, List<DataItem> groupItems, DataItem groupByItem,
             string drillDownGroup, List<DataItem> dataItemsPromotedToGroup, List<DataItem> newGroupList, bool isDataMapDisplayedGroup,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;

			// To ensure grouping name is unique in case of data items with same name,
			// concatenate the tablename and itemname
            DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == groupByItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(groupByItem.DataTableCode);
			string TableName = ThisDataTable.Name;
			_name           = TableName + groupByItem.Name;
			_drillDownGroup = drillDownGroup;
			_isDataMapDisplayedGroup = isDataMapDisplayedGroup;

			// If the item being grouped by is an item that should integrate other
			// non-group items from the same table...
			if (groupByItem.IsCommonTableGrouping)
			{
				// Search through the selected data items
				foreach (DataItem Item in fieldList)
				{
					// If this current item is not being grouped by and it is the on 
					// the same table as the current group item OR
					// This current item is actually the group item itself
					if (! groupItems.Contains(Item) && groupByItem.DataTableCode == Item.DataTableCode ||
						Item.Equals(groupByItem) )
					{
                        string hyperlink = null;
                        if (Item.IsLink)
                        {
                            var url = ConfigurationManager.AppSettings["EmailLinkURL"];
                            hyperlink = "=\"" + url + "\" & Fields!" + Item.LinkAssociatedField + ".Value.ToString()";
                        
                            //hyperlink = "=\"http://www.Ucb.com/incidentreport.aspx?incidentcode=\" & Fields!" + Item.LinkAssociatedField + ".Value.ToString()";
                        }
                        
                            // Integrate it into the grouping by adding a non-empty text box
                            _tableGroupTextBoxList.Add(new TableGroupTextBox(Item, false, _name,
                                newGroupList, groupByItem, CurrentDataModel,hyperlink));
                            dataItemsPromotedToGroup.Add(Item);
                        
					}
					else
					{
						_tableGroupTextBoxList.Add(new TableGroupTextBox(Item, true, _name,
                            newGroupList, groupByItem, CurrentDataModel));
					}
				}
			}
			else
			{
				// There is no need to integrate any other data items into this group item's
				// grouping, so add textboxes as normal for all selected fields
				foreach (DataItem Item in fieldList)
				{
					// If this Item is a 'group by' item, add a non-empty textbox for it
					if (groupItems.Contains(Item) && Item.Code == groupByItem.Code)
					{
                        string hyperlink = null;
                        if (Item.IsLink)
                        {
                            var url = ConfigurationManager.AppSettings["EmailLinkURL"];
                            hyperlink = "=\"" + url + "\" & Fields!" + Item.LinkAssociatedField + ".Value.ToString()";
                            //hyperlink = "=\"http://www.Ucb.com/incidentreport.aspx?incidentcode=\" & Fields!" + Item.LinkAssociatedField + ".Value.ToString()";
                        }
						_tableGroupTextBoxList.Add(new TableGroupTextBox(Item, false, _name,
                            newGroupList, groupByItem, CurrentDataModel,hyperlink));
					}
					else
					{
						_tableGroupTextBoxList.Add(new TableGroupTextBox(Item, true, _name,
                            newGroupList, groupByItem, CurrentDataModel));
					}
				}
			}
		}

		#endregion

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL TableGroup type to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: Add necessary comment on each code block and 
			//indent the code as in XML where possible to increase readability
			// Done 24/05/2005
			xmlWriter.WriteStartElement("TableGroup");
			
				xmlWriter.WriteStartElement("Grouping");
				xmlWriter.WriteAttributeString("Name", "Grouping" + _name);
				if (_isDataMapDisplayedGroup == true) // check if data map neds displaying
					{
						RdlRender.AddLine( xmlWriter, "Label", "=Fields!" + _name + ".Value");
					}
				xmlWriter.WriteStartElement("GroupExpressions");
				RdlRender.AddLine( xmlWriter, "GroupExpression", "=Fields!" + _name + ".Value");
				xmlWriter.WriteEndElement(); // GroupExpressions
				xmlWriter.WriteEndElement(); // Grouping

				xmlWriter.WriteStartElement("Sorting");
					xmlWriter.WriteStartElement("SortBy");
						RdlRender.AddLine( xmlWriter, "SortExpression", "=Fields!" + _name + ".Value");
						RdlRender.AddLine( xmlWriter, "Direction", "Ascending");
					xmlWriter.WriteEndElement(); // SortBy
				xmlWriter.WriteEndElement(); // Sorting

				xmlWriter.WriteStartElement("Header");
					xmlWriter.WriteStartElement("TableRows");
						xmlWriter.WriteStartElement("TableRow");
							xmlWriter.WriteStartElement("TableCells");

							// Render each TableGroupTextBox in this TableGroup
							foreach (TableGroupTextBox Textbox in _tableGroupTextBoxList)
								Textbox.Render( xmlWriter);

							xmlWriter.WriteEndElement(); // TableCells

						// Render footer RDL. Add Visibility information if drillDownGroup != ""
						RdlRender.AddLine( xmlWriter, "Height", ".25in");

						if (_drillDownGroup != "")
						{
							xmlWriter.WriteStartElement("Visibility");
							RdlRender.AddLine( xmlWriter, "Hidden", "true");
							RdlRender.AddLine( xmlWriter, "ToggleItem", "Group" + _drillDownGroup);
							xmlWriter.WriteEndElement(); // Visibility
						}

						xmlWriter.WriteEndElement(); // TableRow
					xmlWriter.WriteEndElement(); // TableRows
				xmlWriter.WriteEndElement(); // Header
			xmlWriter.WriteEndElement(); // TableGroup
		}

        /// <summary>
        /// Makes object render itself as an RDL TableGroup type to an XmlWriter instance.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            //TODO: Code Review Issue 11/05/05: Add necessary comment on each code block and 
            //indent the code as in XML where possible to increase readability
            // Done 24/05/2005
            xmlWriter.WriteStartElement("TablixRow");
            RdlRender.AddLine(xmlWriter, "Height", ".25in");
            xmlWriter.WriteStartElement("TablixCells");

            // Render each TableGroupTextBox in this TableGroup
            foreach (TableGroupTextBox Textbox in _tableGroupTextBoxList)
                Textbox.Render2010(xmlWriter);

            xmlWriter.WriteEndElement(); // TablixCells
            xmlWriter.WriteEndElement(); // TablixRow

            /*
            // Render footer RDL. Add Visibility information if drillDownGroup != ""
            
            if (_drillDownGroup != "")
            {
                xmlWriter.WriteStartElement("Visibility");
                RdlRender.AddLine(xmlWriter, "Hidden", "true");
                RdlRender.AddLine(xmlWriter, "ToggleItem", "Group" + _drillDownGroup);
                xmlWriter.WriteEndElement(); // Visibility
            }

            xmlWriter.WriteEndElement(); // TableRow
            xmlWriter.WriteEndElement(); // TableRows
            xmlWriter.WriteEndElement(); // Header
            xmlWriter.WriteEndElement(); // TableGroup
            */
           
            /*
            xmlWriter.WriteStartElement("Grouping");
            xmlWriter.WriteAttributeString("Name", "Grouping" + _name);
            if (_isDataMapDisplayedGroup == true) // check if data map neds displaying
            {
                RdlRender.AddLine(xmlWriter, "Label", "=Fields!" + _name + ".Value");
            }
            xmlWriter.WriteStartElement("GroupExpressions");
            RdlRender.AddLine(xmlWriter, "GroupExpression", "=Fields!" + _name + ".Value");
            xmlWriter.WriteEndElement(); // GroupExpressions
            xmlWriter.WriteEndElement(); // Grouping

            xmlWriter.WriteStartElement("Sorting");
            xmlWriter.WriteStartElement("SortBy");
            RdlRender.AddLine(xmlWriter, "SortExpression", "=Fields!" + _name + ".Value");
            RdlRender.AddLine(xmlWriter, "Direction", "Ascending");
            xmlWriter.WriteEndElement(); // SortBy
            xmlWriter.WriteEndElement(); // Sorting
            */
        }
        
	}
}
