/*----------------------------------------------------------------------
Name:			ReportParameter

Description:	Object representing a report parameter in a TabularReport, 
				able to render itself in RDL.

History:
--------
02 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.Xml;
using System.Linq;
//using Dwp.Esf.Mis.BusinessObjects;
//using Dwp.Esf.Mis.ResourceHelper;
//using Dwp.Esf.DataAccess.DataTransferManager;
//using Dwp.Esf.DataAccess.DataTransferObject;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.ResourceHelper;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Object representing a report parameter in a TabularReport, 
	/// able to render itself in RDL.
	/// </summary>
	public class ReportParameter : IRdlRenderable,IDataModelAware
	{
		#region Private data members
		
		// Name of this report parameter
		private string _name;

		// Data type of this parameter
		private string _type;

		// Caption that this parameter is selected as
		private string _caption;

		// Denotes if this parameter represents a value type data item
		private bool _isValueType;

		// TODO: Code Review Issue 11/05/05: Remove TODO and code if no longer needed.
		// Done 24/05/2005

        public DataModel CurrentDataModel { get; set; }
		
		#endregion

		#region Constructors

		// TODO: Code Review Issue 11/05/05: Remove unnecessary para tags
		/// <summary>
		/// Object representing a report parameter in a TabularReport, 
		/// able to render itself in RDL.
		/// </summary>
		/// <param name="item">The data item being represented by this parameter.</param>
		public ReportParameter(DataItem item,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;
		
			// To ensure grouping name is unique in case of data items with same name,
			// concatenate the tablename and itemname
			DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x=>x.Code == item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(item.DataTableCode);
			string TableName  = ThisDataTable.Name;
			_name             = TableName + item.Name;
			_caption          = item.Caption;
			_type             = item.DataType;
			_isValueType      = item.IsValueType;
		}

		#endregion


		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL ReportParameter to an XmlWriter object.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			xmlWriter.WriteStartElement("ReportParameter");
			xmlWriter.WriteAttributeString("Name", _name);

			// Render relevant data type
			switch (_type)
			{
				case "char":
				case "varchar":
                case "nchar":
                case "nvarchar":
                    RdlRender.AddLine(xmlWriter, "DataType", "String");
					break;
				case "datetime":
                case "date":
                case "time":
                case "timestamp":
					RdlRender.AddLine(xmlWriter, "DataType", "DateTime");
					break;
				case "money":
				case "decimal":
				case "numeric":
					RdlRender.AddLine(xmlWriter, "DataType", "Float");
					break;
				case "int":
					RdlRender.AddLine(xmlWriter, "DataType", "Integer");
					break;
				default:
					//TODO: Code Review Issue 11/05/05: Replace the hard-coded message by a Resource Helper Text
					// Done 24/05/2005
					throw new Exception(Resource.GetString("RES_EXCEPTION_REPORTPARAMETER_INVALIDDATATYPE"));
			}

			// ValidValues: This displays parameters with values in a drop-down list
			// if the item is not a value type
			if (! _isValueType)
			{
				xmlWriter.WriteStartElement("ValidValues");
				xmlWriter.WriteStartElement("DataSetReference");
				RdlRender.AddLine(xmlWriter, "DataSetName", _name + "ParameterDataSet");
				RdlRender.AddLine(xmlWriter, "ValueField", _name);
				xmlWriter.WriteEndElement(); // DataSetReference
				xmlWriter.WriteEndElement(); // ValidValues
			}
			
			RdlRender.AddLine( xmlWriter, "Prompt", _caption);
			RdlRender.AddLine( xmlWriter, "AllowBlank", "true");
			RdlRender.AddLine( xmlWriter, "Nullable", "true");
			xmlWriter.WriteEndElement(); // ReportParameter
		}
        /// <summary>
        /// Makes object render itself as an RDL ReportParameter to an XmlWriter object.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            xmlWriter.WriteStartElement("ReportParameter");
            xmlWriter.WriteAttributeString("Name", _name);

            // Render relevant data type
            switch (_type)
            {
                case "char":
                case "varchar":
                case "nchar":
                case "nvarchar":
                    RdlRender.AddLine(xmlWriter, "DataType", "String");
                    break;
                case "datetime":
                case "date":
                case "time":
                case "timestamp":
                    RdlRender.AddLine(xmlWriter, "DataType", "DateTime");
                    break;
                case "money":
                case "decimal":
                case "numeric":
                    RdlRender.AddLine(xmlWriter, "DataType", "Float");
                    break;
                case "int":
                    RdlRender.AddLine(xmlWriter, "DataType", "Integer");
                    break;
                case "bit":
                    RdlRender.AddLine(xmlWriter, "DataType", "Boolean");
                    break;
                default:
                    //TODO: Code Review Issue 11/05/05: Replace the hard-coded message by a Resource Helper Text
                    // Done 24/05/2005
                    throw new Exception(Resource.GetString("RES_EXCEPTION_REPORTPARAMETER_INVALIDDATATYPE"));
            }

            // ValidValues: This displays parameters with values in a drop-down list
            // if the item is not a value type
            if (!_isValueType)
            {
                xmlWriter.WriteStartElement("ValidValues");
                xmlWriter.WriteStartElement("DataSetReference");
                RdlRender.AddLine(xmlWriter, "DataSetName", _name + "ParameterDataSet");
                RdlRender.AddLine(xmlWriter, "ValueField", _name);
                xmlWriter.WriteEndElement(); // DataSetReference
                xmlWriter.WriteEndElement(); // ValidValues
            }
            else if (_type == "bit")
            {
                xmlWriter.WriteStartElement("ValidValues");
                xmlWriter.WriteStartElement("ParameterValues");
                xmlWriter.WriteStartElement("ParameterValue");
                RdlRender.AddLine(xmlWriter, "Value", "true");
                RdlRender.AddLine(xmlWriter, "Label", "Yes");
                xmlWriter.WriteEndElement(); // ParameterValue
                xmlWriter.WriteStartElement("ParameterValue");
                RdlRender.AddLine(xmlWriter, "Value", "false");
                RdlRender.AddLine(xmlWriter, "Label", "No");
                xmlWriter.WriteEndElement(); // ParameterValue
                xmlWriter.WriteStartElement("ParameterValue");
                RdlRender.AddLine(xmlWriter, "Label", "None");
                xmlWriter.WriteEndElement(); // ParameterValue
                xmlWriter.WriteEndElement(); // ParameterValues
                xmlWriter.WriteEndElement(); // ValidValues
            }

            RdlRender.AddLine(xmlWriter, "Prompt", _caption);
            RdlRender.AddLine(xmlWriter, "AllowBlank", "true");
            RdlRender.AddLine(xmlWriter, "Nullable", "true");
            xmlWriter.WriteEndElement(); // ReportParameter
      
        }

	}
}
