/*----------------------------------------------------------------------
Name:			TabularReport

Description:	Generates a tabular report rendered in RDL for 
				Reporting Services.

History:
--------
24 Feb 2005		1.00 KSB	Created.
20 Apr 2005		1.01 LL     Changed ReportParameterBusinessObject to 
								ReportDefinitionBusinessObject
11 May 2005		1.02 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
01 Jun 2005		1.05 DH		Changed Integrated Security to False
17 Jun 2005		1.06 KB		TableGroup now takes the new grouplist of 
								the querywrapper as an argument.
30 Jun 2005		1.07 LL		TIR0163 Parameter query needs to be build
							for each data item		
12 Jan 2006		1.08 DM     Added IsDataMapDisplayed references.
30 Jan 2006     1.09 DS     Release 3					
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.IO;
using System.Xml;
using System.Text;
using System.Linq;
using System.Collections;
using System.Web;
using System.Configuration;
//using Dwp.Esf.Mis.ResourceHelper;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.Generic;
using UcbManagementInformation.Server.ResourceHelper;
//using Dwp.Esf.DataAccess.DataTransferObject;
//using Dwp.Esf.DataAccess.DataTransferManager;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a tabular report for Reporting Services.
	/// </summary>
	public class TabularReport : IDataModelAware
	{
		#region Private data members
		
		// Database connection string
		private string _dbConnectionString;

		// The actual report constituent objects
		private TableHeader  _tableHeader;

		//DBS 16/1/05
		private TableFooter  _tableFooter;
		private string _parameterName;
		private string _parameterCaption;
		private string _joinMethod;
		
		private List<string> _parameterList = new List<string>();
        private List<TableGroup> _tableGroupList = new List<TableGroup>();
		private TableDetails _tableDetails;
        private List<Dataset> _datasetList = new List<Dataset>();
        private List<ReportParameter> _reportParametersList = new List<ReportParameter>();
        private List<Chart> _chartList = new List<Chart>();
        private bool _isSummaryReport;
		private bool _isDrillDown;
        private bool _isPageOnFirstItem;
			// The parameters passed to this TabularReport
        private List<DataItem> _selectedDataItems = new List<DataItem>();
        private List<DataItem> _fieldDataItems = new List<DataItem>();
        private string _dataSource;
		// Hold the queries that this report utilises
		private QueryWrapper _mainQueryWrapper;
        private List<QueryWrapper> _parameterQueryWrapperArrayList = new List<QueryWrapper>();
        private int _noOfFilters;
		// A MemoryStream that holds the information written by the xmlWriter
		private MemoryStream _memoryStream;

		// The FileStream can be used as an alternative output for debugging purposes
		// private FileStream _stream = new FileStream("C:\\test.xml", FileMode.Create);
			
		// The XmlWriter that holds the RDL describing this report
		private XmlWriter _xmlWriter;
        public DataModel CurrentDataModel { get; set; }
		
		#endregion


		#region Constructors
		
		/// <summary>
		/// Constructs a TabularReport.
		/// </summary>
		/// <param name="reportConstituents">A ReportDefinitionBusinessObject
		/// encapsulating the input parameters.</param>
		public TabularReport(ReportDefinitionBusinessObject reportConstituents,string modelDataSource)
		{
            _dataSource = modelDataSource;
            //switch (model)
            //{
            //    case "INES":
            //        _dataSource = "ESF2007";
            //        break;
            //    case "MI Participant":
            //        _dataSource = "ESFMIBUSINESSDATA";
            //        break;
            //    case "Managing Authority":
            //        _dataSource = "MANAGINGAUTHORITYDATA";
            //        break;
                
            //}
            //_dbConnectionString
            //    = ConfigurationManager.ConnectionStrings["BusinessDataConnectionString"].ConnectionString;

			// When instantiating the TableGroups, we must record whether we are
			// instantiating the first group or not AND the name of the group previous
			// to the one we are currently dealing with.

            CurrentDataModel = DataAccessUtilities.RepositoryLocator<IDataModelRepository>().Single(x => x.DatasourceName == _dataSource,
                "DataTables",
                "DataTables.DataItems",
                "DataCategories",
                "DataTableJoins",
                "DataTableJoins.DataTableRelationshipJoins",
                "DataTableJoins.DataTableRelationshipJoins.DataTableRelationship",
                "DataTableJoins.DataTableRelationshipJoins.DataTableRelationship.DataTable",
                "DataTableJoins.DataTableRelationshipJoins.DataTableRelationship.DataTable1");

			bool IsFirstGroup = true;
			string PreviousGroupName = ""; 
			bool IsDataMapDisplayed = false;
            IsDataMapDisplayed = reportConstituents.IsDataMapDisplayed;
            _noOfFilters = reportConstituents.FilterList.Count;
			//Table Join
			_isDrillDown = reportConstituents.IsDrillDown;
            _isPageOnFirstItem = reportConstituents.IsPageBreak;
			if (reportConstituents.IsOuterJoin == false)
			{
				_joinMethod = " FULL OUTER JOIN ";
			}
			else
			{
				_joinMethod = " INNER JOIN ";
			}


			// Get the selectedDataItems
			_selectedDataItems = reportConstituents.SelectedDataItems;
            _fieldDataItems = reportConstituents.FieldDataItems;
			// Instantiate the querywrapper for the main query
			_mainQueryWrapper = new QueryWrapper(reportConstituents.SelectedDataItems,
				reportConstituents.RowTotalDataItems, reportConstituents.FilterList,
				reportConstituents.ParameterDataItems, false,reportConstituents.JoinList,CurrentDataModel);

			// Instantiate the querywrapper for the parameter query
			//30/06/05 LL - TIR0163 Parameter query needs to be build for each data item
			foreach(DataItem CurrentParameter in reportConstituents.ParameterDataItems)
			{
				if (!CurrentParameter.IsValueType)
				{
                    List<DataItem> ParameterList = new List<DataItem>();
					ParameterList.Add(CurrentParameter);
					_parameterQueryWrapperArrayList.Add(new QueryWrapper(ParameterList,
						null, null, null, true,null,CurrentDataModel));
				}
			}
						
			// Call the constructors of each of the report constituents
            // TableHeader
			_tableHeader = new TableHeader(reportConstituents.FieldDataItems,DataAccessUtilities.RepositoryLocator<IDataTableRepository>(),CurrentDataModel);


            // TableFooter

            #region Join Description Appended
            //Add any joins in the list that are in the JoinList
            StringBuilder JoinDescription = new StringBuilder();
            foreach (ReportDataTableJoin currentReportJoin in reportConstituents.JoinList)
            {
                DataTableJoin currentJoin = CurrentDataModel.DataTableJoins.Single(x=>x.Code == currentReportJoin.DataTableJoinCode);
                string fromTablePlural = currentJoin.FromTable.Substring(currentJoin.FromTable.Length - 1).ToUpper() == "Y" ? currentJoin.FromTable.Substring(0, currentJoin.FromTable.Length - 1) + "ies" : currentJoin.FromTable + "s";
                string toTablePlural = currentJoin.ToTable.Substring(currentJoin.ToTable.Length - 1).ToUpper() == "Y" ? currentJoin.ToTable.Substring(0, currentJoin.ToTable.Length - 1) + "ies" : currentJoin.ToTable + "s";

                switch (currentReportJoin.JoinType)
                {
                    case "INNER":
                        JoinDescription.Append("INNER: Select only " + fromTablePlural + " that have related " + toTablePlural + " and " +
                    "select only " + toTablePlural + " that have related " + fromTablePlural + Environment.NewLine);
                        break;
                    case "LEFT":
                        JoinDescription.Append("LEFT: Select all " + fromTablePlural + " and only related " +
                    toTablePlural + Environment.NewLine);
                        break;
                    case "RIGHT":
                        JoinDescription.Append("RIGHT: Select all " + toTablePlural + " and only related " +
                            fromTablePlural + Environment.NewLine);
                        break;
                    case "FULL":
                        JoinDescription.Append("FULL: Select all " + fromTablePlural + " and " +
                    toTablePlural + " with or without relations" + Environment.NewLine);
                        break;

                }


            }
            #endregion
            _tableFooter = new TableFooter(reportConstituents.ReportDescription +  Environment.NewLine + JoinDescription.ToString(),reportConstituents.FieldDataItems.Count.ToString(),reportConstituents.FilterList,CurrentDataModel);
            
            // TableGroup


			// This list contains data items that are not grouped by, but are moved into
			// being displayed within a group by the TableGroup constructor.
            List<DataItem> DataItemsPromotedToGroup = new List<DataItem>();

			// This list contains the extra groups added by the wrapper. They are ordered, so
			// each group's index in the list (plus 1) is its grouping level.
            List<DataItem> NewGroupList = _mainQueryWrapper.NewGroupList;

			foreach (DataItem Item in reportConstituents.RowTotalDataItems)
			{
				if (reportConstituents.IsDrillDown)
				{
					if (IsFirstGroup)
					{
						_tableGroupList.Add(new TableGroup(reportConstituents.FieldDataItems, 
							reportConstituents.RowTotalDataItems, Item, "", DataItemsPromotedToGroup,
							_mainQueryWrapper.NewGroupList, IsDataMapDisplayed,CurrentDataModel));
						IsFirstGroup = false;
					}
					else
					{
                        _tableGroupList.Add(new TableGroup(reportConstituents.FieldDataItems, 
							reportConstituents.RowTotalDataItems, Item, PreviousGroupName,
							DataItemsPromotedToGroup, _mainQueryWrapper.NewGroupList, IsDataMapDisplayed,CurrentDataModel));
					}
				}
				else
				{
                    _tableGroupList.Add(new TableGroup(reportConstituents.FieldDataItems, 
						reportConstituents.RowTotalDataItems, Item, "", DataItemsPromotedToGroup,
						_mainQueryWrapper.NewGroupList, IsDataMapDisplayed,CurrentDataModel));
				}

				// To ensure grouping name is unique in case of data items with same name,
				// concatenate the tablename and itemname
				DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x=>x.Code==Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
				string TableName = ThisDataTable.Name;
				PreviousGroupName = TableName + Item.Name;
			}

			// TableDetails
			if (reportConstituents.IsDrillDown)
                _tableDetails = new TableDetails(reportConstituents.FieldDataItems, 
					reportConstituents.RowTotalDataItems, PreviousGroupName,
					DataItemsPromotedToGroup, IsDataMapDisplayed,CurrentDataModel);
			else
                _tableDetails = new TableDetails(reportConstituents.FieldDataItems, 
					reportConstituents.RowTotalDataItems, "", DataItemsPromotedToGroup, reportConstituents.IsDataMapDisplayed,CurrentDataModel);

			// IsSummaryReport
				_isSummaryReport = reportConstituents.IsSummaryReport;
			// Dataset for main query
			_datasetList.Add(new Dataset(reportConstituents.SelectedDataItems, "Table",
				_mainQueryWrapper,CurrentDataModel));

			//30/06/05 LL - TIR0163 Parameter query needs to be build for each data item
			int i = 0;
			foreach (DataItem Item in reportConstituents.ParameterDataItems)
			{
				// Add a dataset for each reference-type parameter
				if (!Item.IsValueType || Item.DataType == "bit")
				{
					// We only require a query for the current data item, so put it into
					// an arraylist to pass to the Dataset constructor.
                    List<DataItem> ParameterList = new List<DataItem>();
					ParameterList.Add(Item);

					// To ensure this parameter's datasetname is unique in case of data items 
					// with same name, concatenate the tablename and itemname
                    DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
					string TableName = ThisDataTable.Name;
                    if (Item.DataType != "bit")
					{
                        _datasetList.Add(new Dataset(ParameterList, TableName + Item.Name + "Parameter", 
						(QueryWrapper)_parameterQueryWrapperArrayList[i],CurrentDataModel));
                        i++;
                    }
					_parameterName= TableName+Item.Name;
					_parameterCaption=Item.Caption;
                    _parameterName = "=\"Selected " + _parameterCaption + " :\"+Parameters!" + _parameterName + ".Label";
					_parameterList.Add(_parameterName);
					
				}

				// ReportParameter
				_reportParametersList.Add(new ReportParameter(Item,CurrentDataModel));
			}
            reportConstituents.ChartList = (from x in reportConstituents.ChartList orderby x.SortOrder select x).ToList(); 
                
            foreach (ReportChartBusinessObject ChartItem in reportConstituents.ChartList)
            {
                Guid CategoryDataItemCode = Guid.Parse(ChartItem.CategoryDataItemCode);
                DataItem CategoryDataItem = CurrentDataModel.DataTables.SelectMany(x => x.DataItems).Single(y=>y.Code == CategoryDataItemCode);//DataAccessUtilities.RepositoryLocator<IDataItemRepository>().GetByCode(CategoryDataItemCode);

                
                List<DataItem> ChartSeriesDataItems = new List<DataItem>();
                ChartItem.ChartSeriesList = (from x in ChartItem.ChartSeriesList orderby x.SortOrder select x).ToList(); 
                foreach(ReportChartSeriesBusinessObject currentSeries in ChartItem.ChartSeriesList)
                {
                    Guid seriesDataItemCode = Guid.Parse(currentSeries.SeriesDataItemCode);

                    DataItem SeriesDataItem = CurrentDataModel.DataTables.SelectMany(x => x.DataItems).Single(y => y.Code == seriesDataItemCode);//DataAccessUtilities.RepositoryLocator<IDataItemRepository>().GetByCode(seriesDataItemCode);

                    ChartSeriesDataItems.Add(SeriesDataItem);
                }

                Chart newChart = new Chart(ChartItem.Type, CategoryDataItem, "TableDataSet", ChartSeriesDataItems[0].DataType, ChartSeriesDataItems,ChartItem.SortOrder,ChartItem.Description,CurrentDataModel);
                _chartList.Add(newChart);
            }
		
			// Initialise the MemoryStream and the XmlWriter
			_memoryStream = new MemoryStream();
			_xmlWriter = new XmlTextWriter(
				_memoryStream, Encoding.UTF8);
				// Either of the lines below may be used as alternative outputs for debugging
				// _stream, Encoding.UTF8);
				// Console.Out);
		}


		#endregion


		#region Render method
		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes this object render itself as an RDL Report type to an XmlWriter instance.
		/// It calls upon all objects immediately beneath it (TableHeader, TableGroup, 
		/// TableDetails, Datasets, ReportParameters) to render themselves also.
		/// </summary>
		/// <returns>A Byte array containing the raw RDL data.</returns>
		public Byte[] Render()
		{
			// Styles for the textboxes
			ReportingServicesStyle _textboxStyle14pt = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.TabularReport14pt);
			ReportingServicesStyle _textboxStyle10pt = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.TabularReport10pt);

			_xmlWriter.WriteStartDocument();

			_xmlWriter.WriteStartElement("Report");
			_xmlWriter.WriteAttributeString("xmlns", 
				"http://schemas.microsoft.com/sqlserver/reporting/2003/10/reportdefinition");

			RdlRender.AddLine( _xmlWriter, "Width", "1.0in");
			RdlRender.AddLine( _xmlWriter, "PageWidth", "11.5in");
			RdlRender.AddLine( _xmlWriter, "TopMargin", "0.25in");
			RdlRender.AddLine( _xmlWriter, "BottomMargin", "0.25in");
			RdlRender.AddLine( _xmlWriter, "LeftMargin", "0.25in");
			RdlRender.AddLine( _xmlWriter, "RightMargin", "0.25in");
			RdlRender.AddLine( _xmlWriter, "PageHeight", "7.9375in");
            RdlRender.AddLine(_xmlWriter, "Language", "en-GB");

			_xmlWriter.WriteStartElement("PageHeader");
			_xmlWriter.WriteStartElement("ReportItems");

            RdlRender.AddTextbox(_xmlWriter, "TitleTextbox", "DWP Small Systems Ad Hoc Reporting System",
				_textboxStyle14pt, "Left", "0.3cm", "5.0cm", "10cm", "true", "0.95cm", "2", "",
				"", "", "", "", "", "", "");
			
			RdlRender.AddTextbox( _xmlWriter, "PageHeader", "=Globals!ReportName", _textboxStyle14pt,
				"Left", "2.85714cm", "1.26984cm", "14.92063cm", "true", "1cm", "", "", "", 
				"", "", "", "", "", "");

			RdlRender.AddImage( _xmlWriter, "Image1", "2.59259cm", "3.8cm",
				"1.0cm", "Embedded", "Esf", "AutoSize");
			int ParameterCount =0;
			int X=0;
			string ParameterHeight="";
			foreach (String _parameterName in _parameterList)
			{
				ParameterCount++;
				X =ParameterCount;
				X=X+3;
				ParameterHeight = X.ToString()+"cm";
				RdlRender.AddTextbox( _xmlWriter, "Parameter"+ParameterCount.ToString(),_parameterName, _textboxStyle14pt,
					"Left", ParameterHeight, "1.26984cm", "14.92063cm", "true", "1cm", "", "", "", 
					"", "", "", "", "", "");
			}
			_xmlWriter.WriteEndElement(); // ReportItems
			X =ParameterCount;
			X=X+4;
			ParameterHeight = X.ToString()+".8cm";
			RdlRender.AddLine( _xmlWriter, "PrintOnLastPage", "true");
			RdlRender.AddLine( _xmlWriter, "PrintOnFirstPage", "true");
			RdlRender.AddLine( _xmlWriter, "Height", ParameterHeight);
			_xmlWriter.WriteEndElement(); // PagHeader

			_xmlWriter.WriteStartElement("Body");
			RdlRender.AddLine( _xmlWriter, "Height", "5in");

			_xmlWriter.WriteStartElement("ReportItems");
			_xmlWriter.WriteStartElement("Table");
			_xmlWriter.WriteAttributeString("Name", "Table1");
			RdlRender.AddLine( _xmlWriter, "DataSetName", "TableDataSet");
			RdlRender.AddLine( _xmlWriter, "Top", "0in");
			RdlRender.AddLine( _xmlWriter, "Left", ".5in");
			RdlRender.AddLine( _xmlWriter, "Height", "0in");
			RdlRender.AddLine( _xmlWriter, "Width", "6in");

			// TableHeader
			_tableHeader.Render( _xmlWriter);

			// TableGroups
			if (_tableGroupList == null || _tableGroupList.Count > 0)
			{
				_xmlWriter.WriteStartElement("TableGroups");
			
				foreach (TableGroup group in _tableGroupList)
					group.Render( _xmlWriter);

				_xmlWriter.WriteEndElement(); // TableGroups
			}

			_xmlWriter.WriteStartElement("Style");
			_xmlWriter.WriteEndElement();

			// Write table columns
			_xmlWriter.WriteStartElement("TableColumns");
			for (int i = 0; i < _selectedDataItems.Count; i++)
			{
				_xmlWriter.WriteStartElement("TableColumn");
				RdlRender.AddLine( _xmlWriter, "Width", "1.5in");
				_xmlWriter.WriteEndElement();
			}
			_xmlWriter.WriteEndElement(); // TableColumns


			// TableDetails
			if (_isSummaryReport == false)
			{
				_tableDetails.Render( _xmlWriter);
			}
			RdlRender.AddLine(_xmlWriter, "NoRows", Resource.GetString("RES_REPORT_NOROWSRETURNED"));
			// TableFooter
			_tableFooter.Render( _xmlWriter);

			_xmlWriter.WriteEndElement(); // Table
			_xmlWriter.WriteEndElement(); // ReportItems
			_xmlWriter.WriteEndElement(); // Body

			// DataSources
			_xmlWriter.WriteStartElement("DataSources");
			_xmlWriter.WriteStartElement("DataSource");
			_xmlWriter.WriteAttributeString("Name", "DataSource1");
			RdlRender.AddLine( _xmlWriter, "DataSourceReference", "/" + _dataSource);
			/*_xmlWriter.WriteStartElement("ConnectionProperties");
			RdlRender.AddLine( _xmlWriter, "DataProvider", "SQL");
			RdlRender.AddLine( _xmlWriter, "ConnectString", _dbConnectionString);
			RdlRender.AddLine( _xmlWriter, "IntegratedSecurity", "false");
			RdlRender.AddLine( _xmlWriter, "Prompt", "false");
			_xmlWriter.WriteEndElement(); // ConnectionProperties */
			_xmlWriter.WriteEndElement(); // DataSource
			_xmlWriter.WriteEndElement(); // DataSources

			// Datasets
			_xmlWriter.WriteStartElement("DataSets");
			foreach (Dataset CurrentDataSet in _datasetList)
				CurrentDataSet.Render( _xmlWriter);
			_xmlWriter.WriteEndElement();

			// ReportParameters
			if (_reportParametersList != null && _reportParametersList.Count > 0)
			{
				_xmlWriter.WriteStartElement("ReportParameters");
				foreach (ReportParameter CurrentParameter in _reportParametersList)
					CurrentParameter.Render( _xmlWriter);
				_xmlWriter.WriteEndElement();
			}

			// EmbeddedImages
			_xmlWriter.WriteStartElement("EmbeddedImages");
			_xmlWriter.WriteStartElement("EmbeddedImage");
			_xmlWriter.WriteAttributeString("Name", "Esf");
			RdlRender.AddLine(_xmlWriter, "MIMEType", "image/bmp");
			_xmlWriter.WriteStartElement("ImageData");

			// Image data
			FileStream ImageStream = new FileStream(
                ConfigurationManager.AppSettings["SmallSystemsLogoImage"], FileMode.Open, FileAccess.Read);
			byte[] ImageByteArray = new byte[ImageStream.Length];
			ImageStream.Read(ImageByteArray, 0, (int) ImageStream.Length);
			ImageStream.Close();
			_xmlWriter.WriteBase64(ImageByteArray, 0, ImageByteArray.Length);

			_xmlWriter.WriteEndElement(); // ImageData
			_xmlWriter.WriteEndElement(); // EmbeddedImage
			_xmlWriter.WriteEndElement(); // EmbeddedImages

			
			// Report footer
			_xmlWriter.WriteStartElement("PageFooter");
			RdlRender.AddLine(_xmlWriter, "Height", "1.0cm");
			RdlRender.AddLine(_xmlWriter, "PrintOnFirstPage", "true");
			RdlRender.AddLine(_xmlWriter, "PrintOnLastPage", "true");
			_xmlWriter.WriteStartElement("ReportItems");

			RdlRender.AddTextbox(_xmlWriter, "UserTextBox", "=User!UserID",
				_textboxStyle10pt, "", "0.25cm", "1.0cm", "6.00cm", "true", "0.5cm", "1",
				"", "", "", "", "", "", "", "");
			RdlRender.AddTextbox(_xmlWriter, "ExecutionTimeTextBox", "=Globals!ExecutionTime",
				_textboxStyle10pt, "", "0.25cm", "7.00cm", "6.00cm", "true", "0.5cm", "1",
				"", "", "", "", "", "", "", "");
			RdlRender.AddTextbox(_xmlWriter, "PageNumberTextBox", "=Globals!PageNumber",
				_textboxStyle10pt, "", "0.25cm", "15.00cm", "3.00cm", "true", "0.5cm", "1",
				"", "", "", "", "", "", "", "");

            _xmlWriter.WriteEndElement(); // ReportItems
			_xmlWriter.WriteEndElement(); // PageFooter

			_xmlWriter.WriteEndElement(); // Report
			_xmlWriter.WriteEndDocument();

			_xmlWriter.Flush();
			
			_memoryStream.Close();
			return _memoryStream.GetBuffer();

			//_stream.Close();
			// return new byte[0];
		}

        /// <summary>
        /// Makes this object render itself as an RDL Report type to an XmlWriter instance.
        /// It calls upon all objects immediately beneath it (TableHeader, TableGroup, 
        /// TableDetails, Datasets, ReportParameters) to render themselves also.
        /// </summary>
        /// <returns>A Byte array containing the raw RDL data.</returns>
        public Byte[] Render2010()
        {
            // Styles for the textboxes
            ReportingServicesStyle _textboxStyle14pt = new ReportingServicesStyle(
                ReportingServicesStyle.TextBoxStyle.TabularReport14pt);
            ReportingServicesStyle _textboxStyle10pt = new ReportingServicesStyle(
                ReportingServicesStyle.TextBoxStyle.TabularReport10pt);

            _xmlWriter.WriteStartDocument();

            _xmlWriter.WriteStartElement("Report");
            _xmlWriter.WriteAttributeString("xmlns",
                "http://schemas.microsoft.com/sqlserver/reporting/2010/01/reportdefinition");
            _xmlWriter.WriteAttributeString("xmlns:cl",
                "http://schemas.microsoft.com/sqlserver/reporting/2010/01/componentdefinition");

            RdlRender.AddLine(_xmlWriter, "AutoRefresh", "0");
            // DataSources
            _xmlWriter.WriteStartElement("DataSources");
            _xmlWriter.WriteStartElement("DataSource");
            _xmlWriter.WriteAttributeString("Name", "DataSource1");
            RdlRender.AddLine(_xmlWriter, "DataSourceReference", "/" + _dataSource);
            /*_xmlWriter.WriteStartElement("ConnectionProperties");
            RdlRender.AddLine( _xmlWriter, "DataProvider", "SQL");
            RdlRender.AddLine( _xmlWriter, "ConnectString", _dbConnectionString);
            RdlRender.AddLine( _xmlWriter, "IntegratedSecurity", "false");
            RdlRender.AddLine( _xmlWriter, "Prompt", "false");
            _xmlWriter.WriteEndElement(); // ConnectionProperties */
            _xmlWriter.WriteEndElement(); // DataSource
            _xmlWriter.WriteEndElement(); // DataSources

            // Datasets
            _xmlWriter.WriteStartElement("DataSets");
            foreach (Dataset CurrentDataSet in _datasetList)
                CurrentDataSet.Render2010(_xmlWriter);
            _xmlWriter.WriteEndElement(); //DataSets

            //ReportSections
            _xmlWriter.WriteStartElement("ReportSections");
            //ReportSection
            _xmlWriter.WriteStartElement("ReportSection");

           

            _xmlWriter.WriteStartElement("Body");
            RdlRender.AddLine(_xmlWriter, "Height", "5in");

            _xmlWriter.WriteStartElement("ReportItems");
            _xmlWriter.WriteStartElement("Tablix");
            _xmlWriter.WriteAttributeString("Name", "Table1");
            RdlRender.AddLine(_xmlWriter, "DataSetName", "TableDataSet");
            RdlRender.AddLine(_xmlWriter, "NoRowsMessage", Resource.GetString("RES_REPORT_NOROWSRETURNED"));
            RdlRender.AddLine(_xmlWriter, "Top", "0in");
            RdlRender.AddLine(_xmlWriter, "Left", ".5in");
            RdlRender.AddLine(_xmlWriter, "Height", "0in");
            RdlRender.AddLine(_xmlWriter, "Width", "6in");

            _xmlWriter.WriteStartElement("TablixBody");

            // Write table columns
            _xmlWriter.WriteStartElement("TablixColumns");
            for (int i = 0; i < _fieldDataItems.Count; i++)
            {
                _xmlWriter.WriteStartElement("TablixColumn");
                RdlRender.AddLine(_xmlWriter, "Width", "1.5in");
                _xmlWriter.WriteEndElement();// TablixColumn
            }
            _xmlWriter.WriteEndElement(); // TablixColumns

            // Write table rows
            _xmlWriter.WriteStartElement("TablixRows");
            
            // TableHeader
            _tableHeader.Render2010(_xmlWriter);

            // TableGroups
            if (_tableGroupList != null && _tableGroupList.Count > 0)
            {

                foreach (TableGroup group in _tableGroupList)
                    group.Render2010(_xmlWriter);

            }

            // TableDetails
            if (_isSummaryReport == false)
            {
                _tableDetails.Render2010(_xmlWriter);
            }



            // TableFooter
            _tableFooter.Render2010(_xmlWriter);

            _xmlWriter.WriteEndElement(); // TablixRows

            _xmlWriter.WriteEndElement(); // TablixBody
            _xmlWriter.WriteStartElement("TablixColumnHierarchy");
            _xmlWriter.WriteStartElement("TablixMembers");
            for (int i = 0; i < _fieldDataItems.Count; i++)
            {
                _xmlWriter.WriteStartElement("TablixMember");
                _xmlWriter.WriteEndElement();// TablixMember
            }
            _xmlWriter.WriteEndElement(); // TablixMembers

            _xmlWriter.WriteEndElement(); // TablixColumnHierarchy


            _xmlWriter.WriteStartElement("TablixRowHierarchy");
            _xmlWriter.WriteStartElement("TablixMembers");
            
            //Add Header row member
            _xmlWriter.WriteStartElement("TablixMember");
            RdlRender.AddLine(_xmlWriter, "KeepWithGroup", "After");
            RdlRender.AddLine(_xmlWriter, "FixedData", "true");
            RdlRender.AddLine(_xmlWriter, "RepeatOnNewPage", "true");
            RdlRender.AddLine(_xmlWriter, "KeepTogether", "true");
            _xmlWriter.WriteEndElement(); // TablixMember

            //Add Group and details hierarchy members
            if (_tableGroupList != null && _tableGroupList.Count > 0)
            {
                for (int i = 0; i < _tableGroupList.Count; i++)//
                {

                    _xmlWriter.WriteStartElement("TablixMember");
                    _xmlWriter.WriteStartElement("Group");
                    _xmlWriter.WriteAttributeString("Name", "Grouping" + _tableGroupList[i].Name);
                    if (_tableGroupList[i].IsDataMapDisplayedGroup) // check if data map neds displaying
                    {
                        RdlRender.AddLine(_xmlWriter, "DocumentMapLabel", "=Fields!" + _tableGroupList[i].Name + ".Value");
                    }
                    if (_isPageOnFirstItem && i == 0)
                    {

                        _xmlWriter.WriteStartElement("PageBreak");
                        RdlRender.AddLine(_xmlWriter,"BreakLocation","Between");
                        _xmlWriter.WriteEndElement(); // PageBreak
                    
                    }

                    _xmlWriter.WriteStartElement("GroupExpressions");
                    RdlRender.AddLine(_xmlWriter, "GroupExpression", "=Fields!" + _tableGroupList[i].Name + ".Value");
                    _xmlWriter.WriteEndElement(); // GroupExpressions
                    _xmlWriter.WriteEndElement(); // Group

                    _xmlWriter.WriteStartElement("SortExpressions");
                    _xmlWriter.WriteStartElement("SortExpression");

                    RdlRender.AddLine(_xmlWriter, "Value", "=Fields!" + _tableGroupList[i].Name + ".Value");
                    //RdlRender.AddLine(xmlWriter, "Direction", "Ascending");
                    _xmlWriter.WriteEndElement(); // SortExpression
                    _xmlWriter.WriteEndElement(); // SortExpressions
                    _xmlWriter.WriteStartElement("TablixMembers");
                    _xmlWriter.WriteStartElement("TablixMember");
                    if (i > 0 && _isDrillDown)
                    {
                        _xmlWriter.WriteStartElement("Visibility");
                        RdlRender.AddLine(_xmlWriter, "Hidden", "true");
                        RdlRender.AddLine(_xmlWriter, "ToggleItem", "Group" + _tableGroupList[i - 1].Name);
                        _xmlWriter.WriteEndElement(); // Visibility

                    }
                    RdlRender.AddLine(_xmlWriter, "KeepWithGroup", "After");
                    RdlRender.AddLine(_xmlWriter, "KeepTogether", "true");
                    _xmlWriter.WriteEndElement(); // TablixMember
                }
            }

            // TableDetails
            if (_isSummaryReport == false)
            {
                 _xmlWriter.WriteStartElement("TablixMember");
                _xmlWriter.WriteStartElement("Group");
                _xmlWriter.WriteAttributeString("Name", "Table1_Details_Group");
                //if (_isDataMapDisplayedGroup == true) // check if data map neds displaying
                //{
                //    RdlRender.AddLine(xmlWriter, "Label", "=Fields!" + _name + ".Value");
                //}
                RdlRender.AddLine(_xmlWriter, "DataElementName", "Detail");
                _xmlWriter.WriteEndElement(); // Group

                
                _xmlWriter.WriteStartElement("TablixMembers");
                _xmlWriter.WriteStartElement("TablixMember");
                if (_isDrillDown)
                {
                    _xmlWriter.WriteStartElement("Visibility");
                    RdlRender.AddLine(_xmlWriter, "Hidden", "true");
                    RdlRender.AddLine(_xmlWriter, "ToggleItem", "Group" + _tableGroupList[_tableGroupList.Count-1].Name);
                    _xmlWriter.WriteEndElement(); // Visibility

                }
                _xmlWriter.WriteEndElement(); // TablixMember
                _xmlWriter.WriteEndElement(); // TablixMembers
                RdlRender.AddLine(_xmlWriter, "DataElementName", "Detail_Collection");
                RdlRender.AddLine(_xmlWriter, "DataElementOutput", "Output");
                RdlRender.AddLine(_xmlWriter, "KeepTogether", "true");
                _xmlWriter.WriteEndElement(); // TablixMember
                
                
            }

            //Close down the group hierarchies
            if (_tableGroupList != null && _tableGroupList.Count > 0)
            {
                for (int i = 0; i < _tableGroupList.Count; i++)//
                {
                    _xmlWriter.WriteEndElement(); // TablixMembers
                    _xmlWriter.WriteEndElement(); // TablixMember

                }
            }
            
            //Add Footer report name member
            _xmlWriter.WriteStartElement("TablixMember");
            RdlRender.AddLine(_xmlWriter, "KeepWithGroup", "Before");
            RdlRender.AddLine(_xmlWriter, "RepeatOnNewPage", "true");
            RdlRender.AddLine(_xmlWriter, "KeepTogether", "true");
            _xmlWriter.WriteEndElement(); // TablixMember

            //Add Footer filter selections
            for(int i=0;i<_noOfFilters;i++)
            {
                _xmlWriter.WriteStartElement("TablixMember");
                RdlRender.AddLine(_xmlWriter, "KeepWithGroup", "Before");
                RdlRender.AddLine(_xmlWriter, "RepeatOnNewPage", "true");
                RdlRender.AddLine(_xmlWriter, "KeepTogether", "true");
                _xmlWriter.WriteEndElement(); // TablixMember
            }

            _xmlWriter.WriteEndElement(); // TablixMembers
            _xmlWriter.WriteEndElement(); // TablixRowHierarchy


            

            

            _xmlWriter.WriteStartElement("Style");
            _xmlWriter.WriteEndElement();

           

            _xmlWriter.WriteEndElement(); // Tablix

            foreach (Chart currentChart in _chartList)
            {
                currentChart.Render2010(_xmlWriter);
            }

            _xmlWriter.WriteEndElement(); // ReportItems
            _xmlWriter.WriteEndElement(); // Body
            
            RdlRender.AddLine(_xmlWriter,"Width",(_selectedDataItems.Count * 1.5 < 11.5)?"11.5in":(_selectedDataItems.Count * 1.5).ToString() + "in");

            

            _xmlWriter.WriteStartElement("Page");
            _xmlWriter.WriteStartElement("PageHeader");
            

            _xmlWriter.WriteStartElement("ReportItems");

            RdlRender.AddTextbox2010(_xmlWriter, "TitleTextbox", "DWP Small Systems Ad Hoc Reporting System",
                _textboxStyle14pt, "Left", "0.3cm", "2in", "6in", "true", "0.95cm", "2", "",
                "", "", "", "", "", "", "");

            RdlRender.AddTextbox2010(_xmlWriter, "PageHeader", "=Globals!ReportName", _textboxStyle14pt,
                "Left", "2.85714cm", "0.5in", "6in", "true", "1cm", "", "", "",
                "", "", "", "", "", "");

            RdlRender.AddImage2010(_xmlWriter, "Image1", "2.59259cm", "1.5in",
                "0.5in", "Embedded", "Esf", "AutoSize");
            int ParameterCount = 0;
            int X = 0;
            string ParameterHeight = "";
            foreach (String _parameterName in _parameterList)
            {
                ParameterCount++;
                X = ParameterCount;
                X = X + 3;
                ParameterHeight = X.ToString() + "cm";
                RdlRender.AddTextbox2010(_xmlWriter, "Parameter" + ParameterCount.ToString(), _parameterName, _textboxStyle14pt,
                    "Left", ParameterHeight, "0.5in","6in", "true", "1cm", "", "", "",
                    "", "", "", "", "", "");
            }
            _xmlWriter.WriteEndElement(); // ReportItems
            X = ParameterCount;
            X = X + 4;
            ParameterHeight = X.ToString() + ".8cm";
            RdlRender.AddLine(_xmlWriter, "PrintOnLastPage", "true");
            RdlRender.AddLine(_xmlWriter, "PrintOnFirstPage", "true");
            RdlRender.AddLine(_xmlWriter, "Height", ParameterHeight);
            _xmlWriter.WriteEndElement(); // PagHeader


            // Report footer
            _xmlWriter.WriteStartElement("PageFooter");
            RdlRender.AddLine(_xmlWriter, "Height", "1.0cm");
            RdlRender.AddLine(_xmlWriter, "PrintOnFirstPage", "true");
            RdlRender.AddLine(_xmlWriter, "PrintOnLastPage", "true");
            _xmlWriter.WriteStartElement("ReportItems");

            RdlRender.AddTextbox2010(_xmlWriter, "UserTextBox", "=Parameters!ReportUserName.Value",
                _textboxStyle10pt, "", "0.25cm", "1.0cm", "6.00cm", "true", "0.5cm", "1",
                "", "", "", "", "", "", "", "");
            RdlRender.AddTextbox2010(_xmlWriter, "ExecutionTimeTextBox", "=Globals!ExecutionTime",
                _textboxStyle10pt, "", "0.25cm", "7.00cm", "6.00cm", "true", "0.5cm", "1",
                "", "", "", "", "", "", "", "");
            RdlRender.AddTextbox2010(_xmlWriter, "PageNumberTextBox", "=Globals!PageNumber",
                _textboxStyle10pt, "", "0.25cm", "15.00cm", "3.00cm", "true", "0.5cm", "1",
                "", "", "", "", "", "", "", "");

            _xmlWriter.WriteEndElement(); // ReportItems
            _xmlWriter.WriteEndElement(); // PageFooter


            _xmlWriter.WriteEndElement(); // Page

            _xmlWriter.WriteEndElement(); // RepoetSection
            _xmlWriter.WriteEndElement(); // ReportSections
            
           
            //Add a report parameter for user id 

            _xmlWriter.WriteStartElement("ReportParameters");
                

            _xmlWriter.WriteStartElement("ReportParameter");
            _xmlWriter.WriteAttributeString("Name", "ReportUserName");
            // Render relevant data type
            RdlRender.AddLine(_xmlWriter, "DataType", "String");
            RdlRender.AddLine(_xmlWriter, "Prompt", "User Name");
            RdlRender.AddLine(_xmlWriter, "Hidden", "true");
            _xmlWriter.WriteEndElement(); // ReportParameter

            _xmlWriter.WriteStartElement("ReportParameter");
            _xmlWriter.WriteAttributeString("Name", "ReportUserCode");
            // Render relevant data type
            RdlRender.AddLine(_xmlWriter, "DataType", "String");
            RdlRender.AddLine(_xmlWriter, "Prompt", "User Name");
            RdlRender.AddLine(_xmlWriter, "Hidden", "true");
            _xmlWriter.WriteEndElement(); // ReportParameter

            // ReportParameters
            if (_reportParametersList != null && _reportParametersList.Count > 0)
            {
                foreach (ReportParameter CurrentParameter in _reportParametersList)
                    CurrentParameter.Render2010(_xmlWriter);
             }
            _xmlWriter.WriteEndElement();
          
            
          
            // EmbeddedImages
            _xmlWriter.WriteStartElement("EmbeddedImages");
            _xmlWriter.WriteStartElement("EmbeddedImage");
            _xmlWriter.WriteAttributeString("Name", "Esf");
            RdlRender.AddLine(_xmlWriter, "MIMEType", "image/bmp");
            _xmlWriter.WriteStartElement("ImageData");

            // Image data
            FileStream ImageStream = new FileStream(
                HttpContext.Current.Server.MapPath(ConfigurationManager.AppSettings["SmallSystemsLogoImage"]), FileMode.Open, FileAccess.Read);
            byte[] ImageByteArray = new byte[ImageStream.Length];
            ImageStream.Read(ImageByteArray, 0, (int)ImageStream.Length);
            ImageStream.Close();
            _xmlWriter.WriteBase64(ImageByteArray, 0, ImageByteArray.Length);

            _xmlWriter.WriteEndElement(); // ImageData
            _xmlWriter.WriteEndElement(); // EmbeddedImage
            _xmlWriter.WriteEndElement(); // EmbeddedImages

            RdlRender.AddLine(_xmlWriter, "ConsumeContainerWhitespace", "true");
            _xmlWriter.WriteEndElement(); // Report
            _xmlWriter.WriteEndDocument();
            //****TESTING*************
           // _xmlWriter.Flush();
           // XmlDocument doc = RdlRender.CreateXmlDocument(_memoryStream);
            //*****************/
            _xmlWriter.Flush();
            
            _memoryStream.Close();
            return _memoryStream.ToArray();// GetBuffer();

            //_stream.Close();
            // return new byte[0];
        }
		#endregion
	}
} 

