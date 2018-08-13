/*----------------------------------------------------------------------
Name:			MatrixStaticRow

Description:	Generates a matrix static row rendered in RDL for use in a
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
	/// A matrix static row rendered in RDL for use in a MatrixReport object.
	/// </summary>
    public class MatrixStaticRow : IRdlRenderable,IDataModelAware
	{
		#region Private data members

		// Name of this row
		private string _name;

		// Name of field used in this row
		private string _fieldName;

		// Style of textbox used by this row
		private ReportingServicesStyle _textboxStyle;
        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		/// <summary>
		/// Constructs a new MatrixStaticRow.
		/// </summary>
		/// <param name="name">Name of this static row.</param>
		/// <param name="Item">The data item represented in this row.</param>
		public MatrixStaticRow(string name, DataItem Item,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;
		
			_name      = name;

			// To ensure field name is unique in case of data items with same name,
			// concatenate the tablename and itemname
            DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x=>x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
			string TableName = ThisDataTable.Name;
			_fieldName = Item.Caption;//TableName + Item.Name;

			_textboxStyle = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.MatrixRow);
		}

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Renders this object as an RDL StaticRow to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XmlWriter to render the object to.</param>
		public void Render(XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: give comment and/or indent the code as in XML 
			//to increase readability
			// Done 24/05/2005
			xmlWriter.WriteStartElement("StaticRow");
			xmlWriter.WriteStartElement("ReportItems");
			RdlRender.AddTextbox(xmlWriter, _name, _fieldName, _textboxStyle, "Right", 
				"", "", "", "true", "", "", "", "", "", "", "", "", "", "");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}

        public void Render2010(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}
