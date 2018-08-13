/*----------------------------------------------------------------------
Name:			TableDetails

Description:	Generates table details rendered in RDL for use in a
				TabularReport object.

History:
--------
02 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
12 Jan 2006     1.03 DM     Added isDataMapDisplayedGroup references.
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
using System.Configuration;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates table details rendered in RDL for use in a
	/// TabularReport object.
	/// </summary>
	public class TableDetails : IRdlRenderable,IDataModelAware
	{
		#region Private data members

		// Name of DrillDownGroup
		private string _drillDownGroup;

		// Name of IsDataMapDisplayedGroup
		//private string _isDataMapDisplayedGroup;


		// List of TableDetailsTextBoxes
        private List<TableDetailsTextBox> _tableDetailsTextBoxList = new List<TableDetailsTextBox>();

        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		/// <summary>
		/// Generates table details rendered in RDL for use in a
		/// TabularReport object.
		/// </summary>
		/// <param name="fieldList">List of selected fields to be used.</param>
		/// <param name="groupList">List of fields for the report to be grouped by.</param>
		/// <param name="drillDownGroup">Indicates if this is a drill down group.</param>
		/// <param name="dataItemsPromotedToGroup">Records data items that are moved from details
		/// <param name="isDataMapDisplayedGroup">Indicates if this is a is data map displayed group.</param>
		/// level into the grouping.</param>
        public TableDetails(List<DataItem> fieldList, List<DataItem> groupList, string drillDownGroup,
            List<DataItem> dataItemsPromotedToGroup, bool isDataMapDisplayedGroup,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;

			_drillDownGroup = drillDownGroup;
			
            //if (isDataMapDisplayedGroup)
            //{
            //    _isDataMapDisplayedGroup = "Y";
            //}
            //else
            //    _isDataMapDisplayedGroup = "N";


			foreach (DataItem Item in fieldList)
			{
                if (groupList.Contains(Item) || dataItemsPromotedToGroup.Contains(Item))
                {
                    // This textbox should have a uniquely random name, hence it is 
                    // named using a GUID (without dashes).
                    string GuidName = Guid.NewGuid().ToString().Replace("-", "");
                    _tableDetailsTextBoxList.Add(new TableDetailsTextBox(
                        GuidName, true, Item.DataType));
                }
                else
                {
                    // To ensure textbox name is unique in case of data items with same name,
                    // concatenate the tablename and itemname
                    DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
                    string TableName = ThisDataTable.Name;
                    if (Item.IsLink)
                    {
                        var url = ConfigurationManager.AppSettings["EmailLinkURL"];
                        string hyperlink = "=\"" + url +"\" & Fields!" + Item.LinkAssociatedField + ".Value.ToString()";
                        
                        _tableDetailsTextBoxList.Add(new TableDetailsTextBox(
                        TableName + Item.Name, false, Item.DataType, hyperlink));
                    }
                    else
                    {
                        _tableDetailsTextBoxList.Add(new TableDetailsTextBox(
                            TableName + Item.Name, false, Item.DataType));
                    }
                }
			}
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL Details type to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("Details");
			xmlWriter.WriteStartElement("TableRows");
			xmlWriter.WriteStartElement("TableRow");
			xmlWriter.WriteStartElement("TableCells");

			foreach (TableDetailsTextBox Textbox in _tableDetailsTextBoxList)
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
			xmlWriter.WriteEndElement(); // Details
		}


        /// <summary>
        /// Makes object render itself as an RDL Details type to an XmlWriter instance.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TablixRow");
            RdlRender.AddLine(xmlWriter, "Height", ".25in");
            xmlWriter.WriteStartElement("TablixCells");

            foreach (TableDetailsTextBox Textbox in _tableDetailsTextBoxList)
                Textbox.Render2010(xmlWriter);

            

            xmlWriter.WriteEndElement(); // TablixCells
            xmlWriter.WriteEndElement(); // TablixRow
        }
	}
}
