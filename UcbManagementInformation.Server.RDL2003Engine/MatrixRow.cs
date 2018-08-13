/*----------------------------------------------------------------------
Name:			MatrixRow

Description:	Generates a matrix row rendered in RDL for use in a
				MatrixReport object.

History:
--------
07 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;
using System.Linq;
//using Dwp.Esf.DataAccess.DataTransferManager;
//using Dwp.Esf.DataAccess.DataTransferObject;
//using Dwp.Esf.Mis.BusinessObjects;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// A matrix row rendered in RDL for use in a MatrixReport object.
	/// </summary>
    public class MatrixRow : IRdlRenderable,IDataModelAware
	{
		#region Private data members

		// Name of this row
		private string _name;

		// Name of field used in this row
		private string _fieldName;

		// Data type of the field
		private string _dataType;

		private ReportingServicesStyle _textboxStyle;

		// Denotes if the value for this row's textbox is summable
		private bool _isSummable;
		
		// The grouping level for this row
		private int _groupingLevel;
        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		/// <summary>
		/// Constructs a new MatrixRow.
		/// </summary>
		/// <param name="name">Name of this row.</param>
		/// <param name="Item">The data item represented on this row.</param>
		public MatrixRow(string name, DataItem Item,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;

			_name       = name;
            
			// To ensure the textbox referes to a field whose name is unique (in case of 
			// data items with same name) concatenate the tablename and itemname
            DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x=>x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
			string TableName = ThisDataTable.Name;
			_fieldName  = TableName + Item.Name;

			_dataType   = Item.DataType;
			_isSummable = Item.IsSummable;

			if (_isSummable)
				_groupingLevel = Item.GroupingLevel;

			_textboxStyle = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.MatrixRow);
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this object as an RDL MatrixRow to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XmlWriter to render the object to.</param>
		public void Render(XmlWriter xmlWriter)
		{
			string TextboxValue = "";
			if (_isSummable)
			{
				if (_groupingLevel == 0)
				{
					TextboxValue = "=Sum(Fields!" + _fieldName + ".Value)";
				}
				else
				{
					TextboxValue = @"=Sum(iif(Fields!GroupingLevel.Value <= " + _groupingLevel + 
						", cdbl(Fields!" + _fieldName + ".Value), cdbl(0)))";
				}

			}
			_textboxStyle.Format = RdlFormatter.FormatData(_dataType);
			
			//TODO: Code Review Issue 11/05/05: indent the code as in XML to increase readability
			// Done 24/05/2005
			xmlWriter.WriteStartElement("MatrixRow");
				RdlRender.AddLine(xmlWriter, "Height", "1cm");
				xmlWriter.WriteStartElement("MatrixCells");
					xmlWriter.WriteStartElement("MatrixCell");
						xmlWriter.WriteStartElement("ReportItems");

						RdlRender.AddTextbox(xmlWriter, _name, TextboxValue, _textboxStyle, "", 
							"", "", "", "true", "", "", "", "", "", "", "", "", "", "");

						xmlWriter.WriteEndElement(); // ReportItems
					xmlWriter.WriteEndElement(); // MatrixCell
				xmlWriter.WriteEndElement(); // MatrixCells
			xmlWriter.WriteEndElement(); // MatrixRow
		}

        public void Render2010(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}
