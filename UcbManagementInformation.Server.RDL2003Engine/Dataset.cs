/*----------------------------------------------------------------------
Name:			Dataset

Description:	A class to hold fields and query information, able to 
				render itself in RDL.
			
History:
--------
02 Mar 2005		1.00 KSB	Created.
10 May 2005		1.01 LL		Code Review
24 May 2005		1.02 KB		Code review amendments.
09 Jun 2005		1.03 KB		Changed ref to Query to ref to QueryWrapper.
15 Jun 2005		1.04 KB		QueryWrapper now passed to this object as a 
								parameter.
---------------------------------------------------------------------- */
//Date Last Code Review: 10/05/2005

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
	/// A class to hold fields and query information, able to 
	/// render itself in RDL.
	/// </summary>
	public class Dataset : IRdlRenderable,IDataModelAware
	{
		#region Private data members

		// Name of this dataset
		private string _name;

		// List of DataItems representing fields in the query
        private List<Field> _fieldList = new List<Field>();

		// An object to wrap the actual query
		private QueryWrapper _queryWrapper;

        public DataModel CurrentDataModel { get; set; }
        

		#endregion

		#region Constructors

		/// <summary>
		/// Constructs a new dataset.
		/// </summary>
		/// <param name="fieldList">List of fields being queried.</param>
		/// <param name="groupList">List of fields the query is to be grouped by.</param>
		/// <param name="filterItems">List of filters to be applied to the resultant
		/// data.</param>
		/// <param name="parametersList">List of parameters to be applied to the 
		/// resultant data.</param>
		/// <param name="name">Name of this dataset.</param>
		/// <param name="isParameterQuery">Denotes whether this query is used as a 
		/// dataset query for a ValidValues tag in RDL (i.e. a parameter query) or 
		/// is a query for data selection.</param>
		public Dataset(List<DataItem> fieldList, string name, QueryWrapper queryWrapper,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;
			_name = name;

			//TODO: Code Review Issue : 10/05/2005 - Casing.	 
			foreach (DataItem Item in fieldList)
			{
				// To ensure field name is unique in case of data items with same name,
				// concatenate the tablename and itemname
                DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
				string TableName = ThisDataTable.Name;

				_fieldList.Add(new Field(TableName + Item.Name, Item.Caption));
			}

            
            DataItem FieldRequiringLink = fieldList.SingleOrDefault(x => x.IsLink == true);
            if (FieldRequiringLink != null)
            {
                _fieldList.Add(new Field(FieldRequiringLink.LinkAssociatedField,FieldRequiringLink.LinkAssociatedField));
            }

			_queryWrapper = queryWrapper;
		}

		#endregion

		/// <summary>
		/// Makes object render itself in RDL to an XmlWriter object.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("DataSet");
			xmlWriter.WriteAttributeString("Name", _name + "DataSet");
			
			xmlWriter.WriteStartElement("Fields");
			//TODO: Code Review Issue : 10/05/2005 - Casing for method variable. 
			//Also should use {} for loop statement for clarity
			foreach (Field f in _fieldList)
				f.Render( xmlWriter);

			xmlWriter.WriteStartElement("Field");
			xmlWriter.WriteAttributeString("Name", "GroupingLevel");
			RdlRender.AddLine(xmlWriter, "DataField", "GroupingLevel");
			xmlWriter.WriteEndElement(); // Field

			xmlWriter.WriteEndElement(); // Fields
			
			_queryWrapper.Render( xmlWriter);

			xmlWriter.WriteEndElement(); // DataSet
		}
        public void Render2010(XmlWriter xmlWriter)
        {
            Render(xmlWriter);
        }
	}
}
