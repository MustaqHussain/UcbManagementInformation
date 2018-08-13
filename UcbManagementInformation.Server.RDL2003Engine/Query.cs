/*----------------------------------------------------------------------
Name:			Query

Description:	Object representing a query of a DataSet, able to render
				itself in RDL.

History:
--------
02 Mar 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
10 Jun 2005		1.03 KB		Added QueryText property.
30 Jan 2006     1.04 DS     Release 3
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
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Object representing a query of a DataSet, able to render
	/// itself in RDL.
	/// </summary>
    public class Query : IRdlRenderable,IDataModelAware
	{
		#region Private data members
		
		// The query in SQL
		private string _queryText;

		// The list of parameters in the query
        private List<string> _queryParameters = new List<string>();

        public DataModel CurrentDataModel { get; set; }
		

		#endregion
		
		#region Constructors

		/// <summary>
		/// Constructs a new Query
		/// </summary>
		/// <param name="fieldList">List of fields used in the query.</param>
		/// <param name="groupList">List of fields for the query to be grouped by.</param>
		/// <param name="filterItems">List of filters for a WHERE clause.</param>
		/// <param name="parametersList">List of data items parameters to be used as
		/// parameters.</param>
		/// <param name="isParameterQuery">Denotes whether this query is used as a 
		/// dataset query for a ValidValues tag in RDL (i.e. a parameter query) or 
		/// is a query for data selection.</param>
		/// <param name="joinMethod">Inner or Outer joins on tables
        public Query(List<DataItem> fieldList, List<DataItem> groupList, List<ReportFilterBusinessObject> filterItems,
            List<DataItem> parametersList, bool isParameterQuery, List<ReportDataTableJoin> joinList,DataModel currentDataModel)
		{
           

            CurrentDataModel = currentDataModel;
			// _queryText becomes the return value of QueryBuilder Render()
            QueryBuilder MyQueryBuilder = new QueryBuilder(fieldList, groupList, filterItems,
                parametersList, isParameterQuery,joinList,CurrentDataModel);//, joinMethod);

			_queryText = MyQueryBuilder.Render();

			if (parametersList != null)
			{
				foreach (DataItem item in parametersList)
				{
					// To ensure query name is unique in case of data items with same name,
					// concatenate the tablename and itemname
					DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x=>x.Code == item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(item.DataTableCode);
					string TableName = ThisDataTable.Name;

					_queryParameters.Add(TableName + item.Name);
				}
			}
            _queryParameters.Add("ReportUserCode");
		}

		#endregion

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		/// <summary>
		/// Makes object render itself as an RDL Query to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: Add necessary comment on each code block and 
			//indent the code as in XML where possible to increase readability
			// Done 24/05/2005
			xmlWriter.WriteStartElement("Query");
			RdlRender.AddLine( xmlWriter, "DataSourceName", "DataSource1");
			RdlRender.AddLine( xmlWriter, "CommandType", "Text");
				
			xmlWriter.WriteStartElement("CommandText");
            //Puts the current user code into the SQL context info. The SetContextInfo stored procedure must exist on the database.
            xmlWriter.WriteString("EXECUTE [SetContextInfo] @ReportUserCode ");
            xmlWriter.WriteString(_queryText);
			xmlWriter.WriteEndElement(); //CommandText

			RdlRender.AddLine( xmlWriter, "Timeout", "120");

			// Each queryParameter must render itself
			if (_queryParameters == null || _queryParameters.Count > 0)
			{
				xmlWriter.WriteStartElement("QueryParameters");
				foreach (string Item in _queryParameters)
				{
					xmlWriter.WriteStartElement("QueryParameter");
					xmlWriter.WriteAttributeString("Name", "@" + Item);
					RdlRender.AddLine( xmlWriter, "Value", "=Parameters!" + 
						Item + ".Value");
					xmlWriter.WriteEndElement(); // QueryParameter
				}
				xmlWriter.WriteEndElement(); // QueryParameters
			}
			
			xmlWriter.WriteEndElement(); // Query
		}

		public string QueryText
		{
			get { return _queryText; }
		}


        public void Render2010(XmlWriter xmlWriter)
        {
            Render(xmlWriter);
        }
    }
}
