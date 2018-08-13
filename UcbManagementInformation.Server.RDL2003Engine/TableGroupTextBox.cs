/*----------------------------------------------------------------------
Name:			TableGroupTextBox

Description:	Generates a table group text box rendered in RDL for 
				use in a TabularReport object.

History:
--------
01 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
15 Jun 2005		1.03 KB		Altered Render() to enable it to render
								GroupingLevel if necessary
17 Jun 2005		1.04 KB		Render() now tests if index of this textbox's
								group item and compares it to this textbox's
								grouping level.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;
using System.Collections;
using System.Linq;
//using Dwp.Esf.Mis.BusinessObjects;
//using Dwp.Esf.DataAccess.DataTransferObject;
//using Dwp.Esf.DataAccess.DataTransferManager;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a table group text box rendered in RDL for 
	/// use in a TabularReport object.
	/// </summary>
	public class TableGroupTextBox : IRdlRenderable,IDataModelAware
	{
		#region Private data members
		
		// TableGroupTextBox name
		private string _name;

		// Denotes if this textbox is empty
		private bool   _isEmpty;

		// Denotes if the value in this textbox is summable
		private bool _isSummable;

		// Name of the data item used by this textbox
		private string _itemName;

		// Data type of data item used by this textbox
		private string _dataType;

		// Grouping that this textbox belongs to
		private string _groupName;

        private string _hyperlink;

		// Style of this textbox
		private ReportingServicesStyle _textboxStyle;

		// Grouping level for this table group
		private int _groupingLevel;

		// Index (within new group list created by QueryWrapper) of the item that 
		// this textbox is grouped by
		private int _newGroupListIndex;

        public DataModel CurrentDataModel { get; set; }
		#endregion

		/// <summary>
		/// Generates a table group text box rendered in RDL for 
		/// use in a TabularReport object.
		/// </summary>
		/// <param name="dataItem">The data item being represented in this textbox.</param>
		/// <param name="isEmpty">Denotes whether this textbox is empty.</param>
		/// <param name="groupName">Name of the grouping to which this data item belongs.</param>
		public TableGroupTextBox(DataItem dataItem, bool isEmpty, 
			string groupName, List<DataItem> newGroupList, DataItem groupByItem, DataModel currentDataModel,string hyperlink=null)
		{
            CurrentDataModel = currentDataModel;
            _hyperlink = hyperlink;
			// To ensure textbox name is unique in case of data items with same name,
			// concatenate the tablename and itemname
            DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x=>x.Code==dataItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(dataItem.DataTableCode);
			string TableName = ThisDataTable.Name;

			// If this textbox is empty, its name should be random but unique
			if (isEmpty)
			{
				string GuidName = Guid.NewGuid().ToString();
				GuidName = GuidName.Replace("-", "");
				_name = GuidName;
			}
			else
			{				
				_name = TableName + dataItem.Name;
			}
			
			_isEmpty      = isEmpty;
			_itemName     = TableName + dataItem.Name;
			_isSummable   = dataItem.IsSummable;
			_dataType     = dataItem.DataType;
			_groupName    = groupName;

			if (newGroupList != null)
				_newGroupListIndex = newGroupList.IndexOf(groupByItem) + 1;

			if (_isSummable)
				_groupingLevel = dataItem.GroupingLevel;

			_textboxStyle = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.TableGroupTextBox);
		}
		
		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL TableCell to an XmlWriter object.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			string TextboxValue = "";

			// If this textbox is empty and the data that its column represents may
			// be summed, then it displays the sum of the values in its column 
			// that are within its group.
			if (_isEmpty)
			{
				/* TEMPORARILY REMOVED */
				if (_isSummable)
				{
					if (_groupingLevel == 0)
					{
						TextboxValue = @"=Sum(Fields!" + _itemName + ".Value)";
					}
					else
					{
						// Only give this textbox a value if the field this textbox is grouped by
						// is at a lower or equal grouping level than this textbox
						if (_newGroupListIndex <= _groupingLevel)
						{
							TextboxValue = @"=Sum(iif(Fields!GroupingLevel.Value <= " + _groupingLevel + 
								", cdbl(Fields!" + _itemName + ".Value), cdbl(0)), " + '\u0022' + "Grouping" + _groupName + '\u0022' + ")";
						}
					}
				}
				/* */
			}
			else
			{
				TextboxValue = "=Fields!" + _name + ".Value";
			}
			_textboxStyle.Format = RdlFormatter.FormatData(_dataType);

			xmlWriter.WriteStartElement("TableCell");
			xmlWriter.WriteStartElement("ReportItems");
			RdlRender.AddTextbox( xmlWriter, "Group" + _name, TextboxValue, _textboxStyle,
				RdlFormatter.FormatAlignment(_dataType), "0in", "0in", ".5in", "true", 
				"", "", "", "", "", "", "", "", "", "");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}


        /// <summary>
        /// Makes object render itself as an RDL TableCell to an XmlWriter object.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            string TextboxValue = "";

            // If this textbox is empty and the data that its column represents may
            // be summed, then it displays the sum of the values in its column 
            // that are within its group.
            if (_isEmpty)
            {
                /* TEMPORARILY REMOVED */
                if (_isSummable)
                {
                    if (_groupingLevel == 0)
                    {
                        TextboxValue = @"=Sum(Fields!" + _itemName + ".Value)";
                    }
                    else
                    {
                        // Only give this textbox a value if the field this textbox is grouped by
                        // is at a lower or equal grouping level than this textbox
                        if (_newGroupListIndex <= _groupingLevel)
                        {
                            TextboxValue = @"=Sum(iif(Fields!GroupingLevel.Value <= " + _groupingLevel +
                                ", cdbl(Fields!" + _itemName + ".Value), cdbl(0)), " + '\u0022' + "Grouping" + _groupName + '\u0022' + ")";
                        }
                    }
                }
                /* */
            }
            else
            {
                TextboxValue = "=Fields!" + _name + ".Value";
            }
            _textboxStyle.Format = RdlFormatter.FormatData(_dataType);

            xmlWriter.WriteStartElement("TablixCell");
            xmlWriter.WriteStartElement("CellContents");
            RdlRender.AddTextbox2010(xmlWriter, "Group" + _name, TextboxValue, _textboxStyle,
                RdlFormatter.FormatAlignment(_dataType), "0in", "0in", ".5in", "true",
                "", "", "", "", "", "", "", "", "", "",_hyperlink);
            xmlWriter.WriteEndElement();//CellContents
            xmlWriter.WriteEndElement();//TablixCell
        }
	}
}
