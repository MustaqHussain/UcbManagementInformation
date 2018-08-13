/*----------------------------------------------------------------------
Name:			TableHeader

Description:	Generates a table header rendered in RDL for use in a
				TabularReport object.

History:
--------
24 Feb 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
21 Jul 2005		1.03 LL		TIR0200 - Modified constructor to allow proper
							text alignment
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.IoC.ServiceLocation;

namespace UcbManagementInformation.Server.RDL2003Engine
{
    
	/// <summary>
	/// Generates a table header for use in a TabularReport object.
	/// </summary>
	public class TableHeader : IRdlRenderable,IDataModelAware
	{
		#region Private data members
		
        

		// A collection of TableHeaderTextBox objects
        private List<ITableHeaderTextBox> _tableHeaderTextBoxList = new List<ITableHeaderTextBox>();

        public List<ITableHeaderTextBox> TableHeaderTextBoxList
        {
            get { return _tableHeaderTextBoxList; }
            set { _tableHeaderTextBoxList = value; }
        }
        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		#region Constructors

		/// <summary>
		/// Generates a table header for use in a TabularReport object.
		/// </summary>
		/// <param name="fieldList">List of selected data items.</param>
		public TableHeader(List<DataItem> fieldList,IDataTableRepository dataTableRepository,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;
            foreach (DataItem item in fieldList)
			{
				// To ensure textbox name is unique in case of data items with same name,
				// concatenate the tablename and itemname
                DataTable ThisDataTable = dataTableRepository.GetByCode(item.DataTableCode);
     			string TableName = ThisDataTable.Name;

				//21/07/05 LL - TIR0200 - Align all columns to their headings
                if (item.DataType.ToLower() == "char" || item.DataType.ToLower() == "varchar" || item.DataType.ToLower() == "nchar" || item.DataType.ToLower() == "nvarchar")
                {
                    Dictionary<string, object> parameterHolder = new Dictionary<string, object>();
                    parameterHolder.Add("name", TableName + item.Name);
                    parameterHolder.Add("caption", item.Caption);
                    _tableHeaderTextBoxList.Add(SimpleServiceLocator.Instance.Get<ITableHeaderTextBox>("TableHeaderTextBox", parameterHolder));
                
				}
				else
				{
                    Dictionary<string, object> parameterHolder = new Dictionary<string, object>();
                    parameterHolder.Add("name", TableName + item.Name);
                    parameterHolder.Add("caption", item.Caption);
                    parameterHolder.Add("isRightAligned", true);

                    _tableHeaderTextBoxList.Add(SimpleServiceLocator.Instance.Get<ITableHeaderTextBox>("TableHeaderTextBoxRight",parameterHolder));
				}
			}
		}
		#endregion

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL Header type to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: indent the code as in XML to increase readability
			// Done 24/05/2005
			xmlWriter.WriteStartElement("Header");
				RdlRender.AddLine( xmlWriter, "RepeatOnNewPage", "true");
					xmlWriter.WriteStartElement("TableRows");
						xmlWriter.WriteStartElement("TableRow");
							xmlWriter.WriteStartElement("TableCells");

							foreach (TableHeaderTextBox TextBox in _tableHeaderTextBoxList)
								TextBox.Render( xmlWriter);

							xmlWriter.WriteEndElement();
						RdlRender.AddLine( xmlWriter, "Height", ".25in");
					xmlWriter.WriteEndElement();
				xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

        /// <summary>
        /// Makes object render itself as an RDL Header type to an XmlWriter instance.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("TablixRow");
            RdlRender.AddLine(xmlWriter, "Height", ".25in");
            xmlWriter.WriteStartElement("TablixCells");
            foreach (ITableHeaderTextBox TextBox in _tableHeaderTextBoxList)
                TextBox.Render2010(xmlWriter);
            xmlWriter.WriteEndElement();//TablixCells
            xmlWriter.WriteEndElement();//TablixRow
            


//            RdlRender.AddLine(xmlWriter, "RepeatOnNewPage", "true");
        }
	}
}
