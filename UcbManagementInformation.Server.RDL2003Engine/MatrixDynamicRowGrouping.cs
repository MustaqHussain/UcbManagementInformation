/*----------------------------------------------------------------------
Name:			MatrixDynamicRowGrouping

Description:	Generates a dynamic row grouping rendered in RDL for 
				use in a MatrixReport object.

History:
--------
08 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.
using System;
using System.Xml;
using System.Collections;
using System.Linq;
using UcbManagementInformation.Server.DataAccess;

//using Dwp.Esf.Mis.BusinessObjects;
//using Dwp.Esf.DataAccess.DataTransferObject;
//using Dwp.Esf.DataAccess.DataTransferManager;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a dynamic row grouping rendered in RDL for use in 
	/// a MatrixReport object.
	/// </summary>
    public class MatrixDynamicRowGrouping : IRdlRenderable,IDataModelAware
	{
		#region Private data members

		// Report components
		private MatrixDynamicRowTextBox  _matrixDynamicRowTextBox;
		private MatrixDynamicRowSubTotal _matrixDynamicRowSubTotal;

		// Name of this grouping
		private string _name;

		// Denotes whether this is a drill down group
		private string _drillDownGroup;

		// Denotes whether a subtotal is included in this group
		private bool _isSubTotal;
        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		/// <summary>
		/// Constructs a new MatrixDynamicRowGrouping.
		/// </summary>
		/// <param name="rowGroup">Data item included in this RowGrouping.</param>
		/// <param name="drillDownGroup">Denotes whether this is a drill down group.</param>
		/// <param name="isSubTotal">Denotes whether a subtotal is included in this group.</param>
		public MatrixDynamicRowGrouping(DataItem rowGroup, string drillDownGroup,
			bool isSubTotal,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;
			_drillDownGroup = drillDownGroup;

			// To ensure field name is unique in case of data items with same name,
			// concatenate the tablename and itemname
            DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == rowGroup.DataTableCode);// DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(rowGroup.DataTableCode);
			string TableName = ThisDataTable.Name;
			_name = TableName + rowGroup.Name;
			_isSubTotal = isSubTotal;
			
			_matrixDynamicRowTextBox =
				new MatrixDynamicRowTextBox(_name);

			if (isSubTotal)
				_matrixDynamicRowSubTotal = 
					new MatrixDynamicRowSubTotal(_name + "Subtotal");
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this object as a RowGrouping in RDL to an XmlWriter.
		/// </summary>
		/// <param name="xmlWriter">The XmlWriter to render the object to.</param>
		public void Render(XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: Indent blocks of code as in XML to increase readability.
			// Done 24/05/2005
			xmlWriter.WriteStartElement("RowGrouping");
				RdlRender.AddLine(xmlWriter, "Width", "3.5cm");
				xmlWriter.WriteStartElement("DynamicRows");
					xmlWriter.WriteStartElement("Grouping");
					xmlWriter.WriteAttributeString("Name", _name + "RowGrouping");
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

				_matrixDynamicRowTextBox.Render(xmlWriter);

				if (_isSubTotal)
					_matrixDynamicRowSubTotal.Render(xmlWriter);

				xmlWriter.WriteEndElement(); // DynamicRows
			xmlWriter.WriteEndElement(); // RowGrouping
		}

        public void Render2010(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}
