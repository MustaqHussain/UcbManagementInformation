/*----------------------------------------------------------------------
Name:			MatrixDynamicColumnGrouping

Description:	Generates a dynamic column grouping rendered in RDL for 
			use in a MatrixReport object.

History:
--------
07 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.
using System;
using System.Xml;
using System.Collections;
using System.Linq;
//using Dwp.Esf.Mis.BusinessObjects;
//using Dwp.Esf.DataAccess.DataTransferObject;
//using Dwp.Esf.DataAccess.DataTransferManager;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a dynamic column grouping rendered in RDL for use in
	/// a MatrixReport object.
	/// </summary>
    public class MatrixDynamicColumnGrouping : IRdlRenderable,IDataModelAware
	{
		#region Private data members

		// Report components
		private MatrixDynamicColumnTextBox  _matrixDynamicColumnTextBox;
		private MatrixDynamicColumnSubTotal _matrixDynamicColumnSubTotal;

		// Name of this column grouping
		private string _name;

		// Denotes whether this is a drill down group
		private string _drillDownGroup;

		// Denotes whether a subtotal is included within this grouping
		private bool _isSubTotal;
        public DataModel CurrentDataModel { get; set; }
		#endregion

		/// <summary>
		/// Constructs a new MatrixDynamicColumnGrouping.
		/// </summary>
		/// <param name="columnGroup">Data item used in this column grouping.</param>
		/// <param name="drillDownGroup">Denotes whether this is a drilldown group.</param>
		/// <param name="isSubTotal">Denotes whether a subtotal is included in this column 
		/// group.</param>
		public MatrixDynamicColumnGrouping(DataItem columnGroup, string drillDownGroup,
			bool isSubTotal,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;

			_drillDownGroup = drillDownGroup;

			// To ensure textbox refers to a fieldname that is unique (in case of data items 
			// with same name) concatenate the tablename and itemname
            DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == columnGroup.DataTableCode);// DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(columnGroup.DataTableCode);
			string TableName = ThisDataTable.Name;
			_name           = TableName + columnGroup.Name;
			_isSubTotal     = isSubTotal;
			
			_matrixDynamicColumnTextBox = new MatrixDynamicColumnTextBox(_name);

			if (isSubTotal)
				_matrixDynamicColumnSubTotal =
					new MatrixDynamicColumnSubTotal(_name + "Subtotal");
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this as a column grouping in RDL to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XmlWriter to render the object to.</param>
		public void Render(XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: Indent blocks of code as in XML to increase readability.
			// Done 24/05/2005
			xmlWriter.WriteStartElement("ColumnGrouping");
				RdlRender.AddLine(xmlWriter, "Height", "1cm");
				xmlWriter.WriteStartElement("DynamicColumns");
					xmlWriter.WriteStartElement("Grouping");
					xmlWriter.WriteAttributeString("Name", _name + "ColumnGrouping");
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

				if (_drillDownGroup != "")
				{
					xmlWriter.WriteStartElement("Visibility");
					RdlRender.AddLine(xmlWriter, "ToggleItem", _drillDownGroup);
					RdlRender.AddLine(xmlWriter, "Hidden", "true");
					xmlWriter.WriteEndElement();
				}

				_matrixDynamicColumnTextBox.Render(xmlWriter);

				if (_isSubTotal)
					_matrixDynamicColumnSubTotal.Render(xmlWriter);

				xmlWriter.WriteEndElement(); // DynamicColumns
			xmlWriter.WriteEndElement(); // ColumnGrouping
		}

        public void Render2010(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}
