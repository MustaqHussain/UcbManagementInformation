/*----------------------------------------------------------------------
Name:			MatrixReport

Description:	Generates a matrix-style report renderable in RDL for
				Reporting Services.

History:
--------
08 Mar 2005		1.00 KSB	Created.
20 Apr 2005		1.01 LL     Changed ReportParameterBusinessObject to 
								ReportDefinitionBusinessObject
11 May 2005		1.02 LL		Code Review.
23 May 2005		1.03 KB		Matrix now renders a 'key' table explaining what 
								data item each dynamic row/column represents
24 May 2005		1.04 KB		Code review amendments.
01 Jun 2005		1.05 DH		Changed Integrated Security to False
30 Jun 2005		1.07 LL		TIR0163 Parameter query needs to be built
							for each data item	
14 Sept 2005	1.08 LL		TIR0304 - Repositioned Dynamic Table.	
30 Jan  2006    1.09 DS     Release 3													
---------------------------------------------------------------------- */
//Last Date Code Reviewed: 11/05/05.

using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Text;
using System.Collections;
using System.Configuration;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.Utilities;
using UcbManagementInformation.Server.ResourceHelper;
using System.Web;
//using Dwp.Esf.Mis.BusinessObjects;
//using Dwp.Esf.DataAccess.DataTransferObject;
//using Dwp.Esf.DataAccess.DataTransferManager;
//using Dwp.Esf.Mis.ResourceHelper;
//using Dwp.Esf.Mis.CrossLayer;

namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a matrix-style report renderable in RDL.
	/// </summary>
    public class MatrixReport : IRdlRenderable,IDataModelAware
	{
		#region Private data members
		
		// Database connection string
		private string _dbConnectionString;
        private string _dataSource;
		//report description etc DBS
		private string _reportDescription;
		private String _Filter;
		private String _FilterDescription;
		private int FilterCount=0;
		//private  FilterItem _CurrentFilter ;
        private List<ReportFilterBusinessObject> _filterItemList = new List<ReportFilterBusinessObject>();
		private const string CURRENTMONTH = "Current Month";

		private string _parameterName;
		private string _parameterCaption;
		private string _joinMethod;
		private int _parameterCount;
        private List<string> _parameterList = new List<string>();

		// Style of textboxes used by this report
		private ReportingServicesStyle _textboxStyle;
		private ReportingServicesStyle _cornerTextboxStyle;

		// Report constituents
        private List<MatrixRow> _matrixRowList = new List<MatrixRow>();
        private List<MatrixStaticRow> _matrixStaticRowList = new List<MatrixStaticRow>();
        private List<MatrixDynamicColumnGrouping> _dynamicColumnGroupingList = new List<MatrixDynamicColumnGrouping>();
        private List<MatrixDynamicRowGrouping> _dynamicRowGroupingList = new List<MatrixDynamicRowGrouping>();
        private List<Dataset> _datasetList = new List<Dataset>();
        private List<ReportParameter> _reportParameterList = new List<ReportParameter>();
        private List<DataItem> _columnTotalDataItemList = new List<DataItem>();
        private List<DataItem> _rowTotalDataItemList = new List<DataItem>();

		// Hold queries used in this report. The mainQueryWrapper holds the query that gets
		// the data for the report, the parameterQueryWrapper gets the parameters if parameters
		// are used.
		private QueryWrapper _mainQueryWrapper;
        private List<QueryWrapper> _parameterQueryWrapperArrayList = new List<QueryWrapper>();

		// Name of the datasets used in this report
		private const string _matrixDataSetName = "Matrix";

		// An XmlWriter that writes the RDL to an underlying stream
		private XmlWriter _xmlWriter;

		// A MemoryStream that holds the information written by the xmlWriter
		private MemoryStream _memoryStream;

		// The FileStream can be used as an alternative output for debugging purposes
		// private FileStream _stream = new FileStream("C:\\test.xml", FileMode.Create);
        public DataModel CurrentDataModel { get; set; }
		
		#endregion


		#region Constructors
		/// <summary>
		/// Constructs a MatrixReport
		/// </summary>
		/// <param name="reportConstituents">A ReportDefinitionBusinessObject
		/// encapsulating the input parameters.</param>
		public MatrixReport(ReportDefinitionBusinessObject reportConstituents,string modelDataSource)
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
            CurrentDataModel = DataAccessUtilities.RepositoryLocator<IDataModelRepository>().Single(x => x.DatasourceName == _dataSource,
               "DataTables",
               "DataTables.DataItems",
               "DataCategories",
               "DataTableJoins",
               "DataTableJoins.DataTableRelationshipJoins",
               "DataTableJoins.DataTableRelationshipJoins.DataTableRelashionship",
               "DataTableJoins.DataTableRelationshipJoins.DataTableRelashionship.DataTable",
               "DataTableJoins.DataTableRelationshipJoins.DataTableRelashionship.DataTable1");
			_textboxStyle = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.MatrixReport);

			_cornerTextboxStyle = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.MatrixReportCorner);

			_columnTotalDataItemList = reportConstituents.ColumnTotalDataItems;
			_rowTotalDataItemList    = reportConstituents.RowTotalDataItems;

			if (reportConstituents.IsOuterJoin == false)
			{
				_joinMethod = " FULL OUTER JOIN ";
			}
			else
			{
				_joinMethod = " INNER JOIN ";
			}


			// Temporary ArrayList used when instantiating the Dataset
			List<DataItem> TempGroupList = new List<DataItem>(reportConstituents.RowTotalDataItems);
			TempGroupList.AddRange(reportConstituents.ColumnTotalDataItems);

			// Initialise the querywrappers
			_mainQueryWrapper = new QueryWrapper(reportConstituents.SelectedDataItems, TempGroupList, 
				reportConstituents.FilterList, reportConstituents.ParameterDataItems, false,reportConstituents.JoinList,CurrentDataModel);

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
			
			// Iteration counter used in MatrixRow and MatrixStaticRow instantiations
			int MatrixRowCount = 0;

			/* Used when instantiating ColumnGroup and RowGroup lists:
			 * The bools Denote if current item is first in list, so should be set to
			 * false after first item dealt with.
			 * The PrevItemName is the name of the item previous to the current item.
			*/
			bool IsFirstInColGroupList = true, IsFirstInRowGroupList = true;
			string PrevItemName = "";		

			// MatrixRow
			foreach (DataItem SelectedItem in reportConstituents.SelectedDataItems)
			{
				if ( !reportConstituents.RowTotalDataItems.Contains(SelectedItem) && 
					 !reportConstituents.ColumnTotalDataItems.Contains(SelectedItem) )
				{
					_matrixRowList.Add(
						new MatrixRow("MatrixRowTextBox" + MatrixRowCount, SelectedItem,CurrentDataModel));
				}
				
				MatrixRowCount++;
			}

			// MatrixDynamicColumnGrouping
			foreach (DataItem ColumnItem in reportConstituents.ColumnTotalDataItems)
			{
				if (IsFirstInColGroupList)
				{
					// To ensure drilldown groupname is unique in case of data items with same name,
					// concatenate the tablename and itemname
                    DataTable SubtotalDataTable
                        = CurrentDataModel.DataTables.Single(x => x.Code == ColumnItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(ColumnItem.DataTableCode);
					string SubtotalTableName = SubtotalDataTable.Name;
					string SubtotalName = SubtotalTableName + ColumnItem.Name + "Subtotal";

					_dynamicColumnGroupingList.Add(
						new MatrixDynamicColumnGrouping(ColumnItem, SubtotalName, true,CurrentDataModel));
					IsFirstInColGroupList = false;
				}
				else
				{
					_dynamicColumnGroupingList.Add(
						new MatrixDynamicColumnGrouping(ColumnItem, PrevItemName, false,CurrentDataModel));
				}
				
				// To ensure field name is unique in case of data items with same name,
				// concatenate the tablename and itemname
                DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == ColumnItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(ColumnItem.DataTableCode);
				string TableName = ThisDataTable.Name;
				PrevItemName = TableName + ColumnItem.Name;				
			}

			// MatrixDynamicRowGrouping
			foreach (DataItem RowItem in reportConstituents.RowTotalDataItems)
			{
				if (IsFirstInRowGroupList)
				{
					// To ensure drilldown groupname is unique in case of data items with same name,
					// concatenate the tablename and itemname
					DataTable SubtotalDataTable
                        = CurrentDataModel.DataTables.Single(x => x.Code == RowItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(RowItem.DataTableCode);
					string SubtotalTableName = SubtotalDataTable.Name;
					string SubtotalName = SubtotalTableName + RowItem.Name + "Subtotal";

					_dynamicRowGroupingList.Add(
						new MatrixDynamicRowGrouping(RowItem, SubtotalName, true,CurrentDataModel));
					IsFirstInRowGroupList = false;
				}
				else
				{
					_dynamicRowGroupingList.Add(
						new MatrixDynamicRowGrouping(RowItem, PrevItemName, false,CurrentDataModel));
				}

				// To ensure field name is unique in case of data items with same name,
				// concatenate the tablename and itemname
                DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == RowItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(RowItem.DataTableCode);
				string TableName = ThisDataTable.Name;
				PrevItemName = TableName + RowItem.Name;				
			}

			// MatrixStaticRow
			MatrixRowCount = 0;
			foreach (DataItem SelectedItem in reportConstituents.SelectedDataItems)
			{
				if ( !(reportConstituents.RowTotalDataItems.Contains(SelectedItem)) && 
					 !(reportConstituents.ColumnTotalDataItems.Contains(SelectedItem)) )
					_matrixStaticRowList.Add(new MatrixStaticRow(
						"MatrixStaticRowTextBox" + MatrixRowCount, SelectedItem,CurrentDataModel));

				MatrixRowCount++;
			}

			// Dataset
			_datasetList.Add(new Dataset(reportConstituents.SelectedDataItems, _matrixDataSetName, 
				_mainQueryWrapper,CurrentDataModel));

			//30/06/05 LL - TIR0163 Parameter query needs to be build for each data item
			int i = 0;
			foreach (DataItem Item in reportConstituents.ParameterDataItems)
			{
				if (!Item.IsValueType)
				{
					// We only require a query for the current data item, so put it into
					// an arraylist to pass to the Dataset constructor.
					List<DataItem> ParameterList = new List<DataItem>();
					ParameterList.Add(Item);

					// To ensure this parameter's datasetname is unique in case of data items 
					// with same name, concatenate the tablename and itemname
                    DataTable ThisDataTable = CurrentDataModel.DataTables.Single(x => x.Code == Item.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(Item.DataTableCode);
					string TableName = ThisDataTable.Name;

					_datasetList.Add(new Dataset(ParameterList, TableName + Item.Name + "Parameter", 
						(QueryWrapper)_parameterQueryWrapperArrayList[i],CurrentDataModel));
					_parameterName= TableName+Item.Name;
					_parameterCaption=Item.Caption;
					_parameterName= "=\"Selected " + _parameterCaption +  " :\"+Parameters!" + _parameterName + ".Value";
					_parameterList.Add(_parameterName);
					i++;
				}
			}


			// ReportParameter
			foreach (DataItem ParameterItem in reportConstituents.ParameterDataItems)
			{
				_reportParameterList.Add(new ReportParameter(ParameterItem,CurrentDataModel));
			}

			//Report Description DBS
			_reportDescription = "Report Description : " + reportConstituents.ReportDescription + " . Show only data with a common denominator = " + reportConstituents.IsOuterJoin.ToString();

			foreach (ReportFilterBusinessObject CurrentFilter in reportConstituents.FilterList)
			{
				//TODO: Code Review Issue 11/05/05: Do not use 'my' in variable names. 
				//Choose a meaningful name. Pascal casing for variables.
				// Done 24/05/2005
                Guid DataItemCode = Guid.Parse(CurrentFilter.DataItemCode);
                DataItem CurrentItem = CurrentDataModel.DataTables.SelectMany(x => x.DataItems).Single(y => y.Code == DataItemCode); //DataAccessUtilities.RepositoryLocator<IDataItemRepository>().GetByCode((CurrentFilter.DataItemCode));

                DataTable CurrentTable = CurrentDataModel.DataTables.Single(x => x.Code == CurrentItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(CurrentItem.DataTableCode);

				//25/08/05 LL - TIR0289 convert 'Current Month' to appropriate data format 
				if(CurrentFilter.FilterValue == CURRENTMONTH)
					CurrentFilter.FilterValue = DateUtility.GetCurrentYearMonth();
				ReportFilterBusinessObject ReportFilter = new ReportFilterBusinessObject() ;
				ReportFilter.DataItemCaption=CurrentItem.Caption;
				ReportFilter.Operand=CurrentFilter.Operand;
				ReportFilter.FilterValue =CurrentFilter.FilterValue;
				_filterItemList.Add(ReportFilter);
			}

			// Initialise the MemoryStream and the XmlWriter
			_memoryStream = new MemoryStream();
			_xmlWriter = new XmlTextWriter(
				_memoryStream, Encoding.UTF8);
				// Either of the lines below may be used as alternative outputs for debugging
				//_stream, Encoding.UTF8);
				// Console.Out);
		}

		#endregion


		#region Render method
		/// <summary>
		/// Renders this MatrixReport in RDL.
		/// </summary>
		/// <returns>A Byte array containing the raw RDL data.</returns>
		public Byte[] Render()
		{
			//TODO: Code Review Issue 11/05/05: Add necessary comment on each code block and 
			//indent the code as in XML where possible to increase readability
			// Done 24/05/2005
			_xmlWriter.WriteStartDocument();

			_xmlWriter.WriteStartElement("Report");
				_xmlWriter.WriteAttributeString("xmlns", 
					"http://schemas.microsoft.com/sqlserver/reporting/2003/10/reportdefinition");

				RdlRender.AddLine( _xmlWriter, "Width", "1in");
				RdlRender.AddLine( _xmlWriter, "PageWidth", "11.5in");
				RdlRender.AddLine( _xmlWriter, "TopMargin", "0.25in");
				RdlRender.AddLine( _xmlWriter, "BottomMargin", "0.25in");
				RdlRender.AddLine( _xmlWriter, "LeftMargin", "0.25in");
				RdlRender.AddLine( _xmlWriter, "RightMargin", "0.25in");
				RdlRender.AddLine( _xmlWriter, "PageHeight", "7.9375in");

				_xmlWriter.WriteStartElement("PageHeader");
				_xmlWriter.WriteStartElement("ReportItems");
					RdlRender.AddImage( _xmlWriter, "Image1", "2.59259cm", "3.8cm",
						"1.0cm", "Embedded", "Esf", "AutoSize");
					RdlRender.AddTextbox(_xmlWriter, "TitleTextbox", "DWP Small Systems Ad Hoc Reporting System",
						_textboxStyle, "Left", "0.3cm", "5.0cm", "10cm", "true", "0.95cm", "2", "",
						"", "", "", "", "", "", "");
					RdlRender.AddTextbox(_xmlWriter, "PageHeader", "=Globals!ReportName", 
						_textboxStyle, "Left", "2.85714cm", "1.26984cm", "14.92063cm", "true", 
						"", "", "", "", "", "", "", "", "", "");

			
				foreach (String _parameterName in _parameterList)
				{
					_parameterCount++;
				
					RdlRender.AddTextbox( _xmlWriter, "Parameter"+_parameterCount.ToString(),_parameterName, _textboxStyle,
						"Left", "2.85714cm", "1.26984cm", "14.92063cm", "true", "", "", "", "", 
						"", "", "", "", "", "");
				}
				_xmlWriter.WriteEndElement(); // ReportItems
				RdlRender.AddLine(_xmlWriter, "PrintOnLastPage", "true");
				RdlRender.AddLine(_xmlWriter, "PrintOnFirstPage", "true");
				RdlRender.AddLine(_xmlWriter, "Height", "1.375in");
				_xmlWriter.WriteEndElement(); // PageHeader

				_xmlWriter.WriteStartElement("Body");

				RdlRender.AddLine(_xmlWriter, "Height", "5in");
				_xmlWriter.WriteStartElement("ReportItems");

					// Add table of keys to dynamic rows/columns
					ReportingServicesStyle BoldBorderedTextBoxStyle = new ReportingServicesStyle(ReportingServicesStyle.TextBoxStyle.BoldBordered);
//						"", "Solid", "Black", "", "", "", "", "10pt", "Bold", "", "", "", "");
					ReportingServicesStyle PlainBorderedTextBoxStyle = new ReportingServicesStyle(ReportingServicesStyle.TextBoxStyle.PlainBordered);
//						"", "Solid", "Black", "", "", "", "", "10pt", "", "", "", "", "");

					// Table's header
					_xmlWriter.WriteStartElement("Table");
						_xmlWriter.WriteAttributeString("Name", "KeyTable");
						RdlRender.AddLine(_xmlWriter, "Left", "1.26984cm");

						RdlRender.AddLine(_xmlWriter, "DataSetName", _matrixDataSetName + "DataSet");
						_xmlWriter.WriteStartElement("Style");
							_xmlWriter.WriteStartElement("BorderStyle");
								RdlRender.AddLine(_xmlWriter, "Default", PlainBorderedTextBoxStyle.BorderStyle);
							_xmlWriter.WriteEndElement(); // BorderStyle
						_xmlWriter.WriteEndElement(); // Style
						_xmlWriter.WriteStartElement("Header");
							_xmlWriter.WriteStartElement("TableRows");
								_xmlWriter.WriteStartElement("TableRow");
								RdlRender.AddLine(_xmlWriter, "Height", "1cm");
									_xmlWriter.WriteStartElement("TableCells");
										_xmlWriter.WriteStartElement("TableCell");
											_xmlWriter.WriteStartElement("ReportItems");
												RdlRender.AddTextbox(_xmlWriter, "KeyTableRows", "Dynamic Rows",
													BoldBorderedTextBoxStyle, "Center", "", "", "", "", "", "", "", "", "",
													"", "", "", "", "");
											_xmlWriter.WriteEndElement(); // ReportItems
										_xmlWriter.WriteEndElement(); // TableCell
										_xmlWriter.WriteStartElement("TableCell");
											_xmlWriter.WriteStartElement("ReportItems");
												RdlRender.AddTextbox(_xmlWriter, "KeyTableColumns", "Dynamic Columns",
													BoldBorderedTextBoxStyle, "Center", "", "", "", "", "", "", "", "", "",
													"", "", "", "", "");
										_xmlWriter.WriteEndElement(); // ReportItems
									_xmlWriter.WriteEndElement(); // TableCell
								_xmlWriter.WriteEndElement(); // TableCells
							_xmlWriter.WriteEndElement(); // TableRow		

							// Add the names of the rows and columns to the table
							for (int RowNum = 0; 
								RowNum < _columnTotalDataItemList.Count && RowNum < _rowTotalDataItemList.Count;
								RowNum++)
							{
								string ColumnItem = _columnTotalDataItemList[RowNum] != null ?
									(  _columnTotalDataItemList[RowNum]).Caption : "";
								string RowItem = _rowTotalDataItemList[RowNum] != null ?
									(  _rowTotalDataItemList[RowNum]).Caption : "";
								
								_xmlWriter.WriteStartElement("TableRow");
								RdlRender.AddLine(_xmlWriter, "Height", "0.8cm");
									_xmlWriter.WriteStartElement("TableCells");
										_xmlWriter.WriteStartElement("TableCell");
											_xmlWriter.WriteStartElement("ReportItems");

											// Column text box
											RdlRender.AddTextbox(_xmlWriter, "KeyTableRow" + RowNum, RowItem,
												PlainBorderedTextBoxStyle, "", "", "", "", "", "", "", "", "", "", "",
												"", "", "", "");

											_xmlWriter.WriteEndElement(); // ReportItems
										_xmlWriter.WriteEndElement(); // TableCell
										_xmlWriter.WriteStartElement("TableCell");
											_xmlWriter.WriteStartElement("ReportItems");

											// Row text box
											RdlRender.AddTextbox(_xmlWriter, "KeyTableColumn" + RowNum, ColumnItem,
												PlainBorderedTextBoxStyle, "", "", "", "", "", "", "", "", "", "", "",
												"", "", "", "");

											_xmlWriter.WriteEndElement(); // ReportItems
										_xmlWriter.WriteEndElement(); // TableCell
									_xmlWriter.WriteEndElement(); // TableCells
								_xmlWriter.WriteEndElement(); // TableRow
							}

							_xmlWriter.WriteEndElement(); // TableRows
						_xmlWriter.WriteEndElement(); // Header
						_xmlWriter.WriteStartElement("TableColumns");
							_xmlWriter.WriteStartElement("TableColumn");
								RdlRender.AddLine(_xmlWriter, "Width", "5.0cm");
							_xmlWriter.WriteEndElement(); // TableColumn
							_xmlWriter.WriteStartElement("TableColumn");
								RdlRender.AddLine(_xmlWriter, "Width", "5.0cm");
							_xmlWriter.WriteEndElement(); // TableColumn
						_xmlWriter.WriteEndElement(); // TableColumns
					_xmlWriter.WriteEndElement(); // Table

					// End of table of keys rendering


					_xmlWriter.WriteStartElement("Matrix");
					_xmlWriter.WriteAttributeString("Name", "Matrix1");
						RdlRender.AddLine(_xmlWriter, "DataSetName", _matrixDataSetName + "DataSet");
						RdlRender.AddLine(_xmlWriter, "Top", "2.5cm");
						RdlRender.AddLine(_xmlWriter, "Left", ".5in");
						RdlRender.AddLine(_xmlWriter, "Height", ".5in");
						RdlRender.AddLine(_xmlWriter, "Width", "6in");
					
						_xmlWriter.WriteStartElement("Corner");
							_xmlWriter.WriteStartElement("ReportItems");

								RdlRender.AddTextbox(_xmlWriter, "CornerTextbox", "", _cornerTextboxStyle, 
									"", "", "", "", "true", "", "", "", "", "", "", "", "", "", "");
							
							_xmlWriter.WriteEndElement(); // ReportItems
						_xmlWriter.WriteEndElement(); // Corner


						// Render MatrixRows
						_xmlWriter.WriteStartElement("MatrixRows");
							foreach (MatrixRow CurrentRow in _matrixRowList)
								CurrentRow.Render(_xmlWriter);
						_xmlWriter.WriteEndElement(); // MatrixRows

					_xmlWriter.WriteStartElement("MatrixColumns");
						_xmlWriter.WriteStartElement("MatrixColumn");
							RdlRender.AddLine(_xmlWriter, "Width", "3.25cm");
						_xmlWriter.WriteEndElement(); // MatrixColumns
					_xmlWriter.WriteEndElement(); // MatrixColumn


					// Render ColumnGroupings
					_xmlWriter.WriteStartElement("ColumnGroupings");
						foreach (MatrixDynamicColumnGrouping CurrentColGroup in _dynamicColumnGroupingList)
							CurrentColGroup.Render(_xmlWriter);
					_xmlWriter.WriteEndElement();


					// Render RowGroupings
					_xmlWriter.WriteStartElement("RowGroupings");
						foreach (MatrixDynamicRowGrouping CurrentRowGroup in _dynamicRowGroupingList)
							CurrentRowGroup.Render(_xmlWriter);
					
						_xmlWriter.WriteStartElement("RowGrouping");
							RdlRender.AddLine(_xmlWriter, "Width", "3.5cm");
							_xmlWriter.WriteStartElement("StaticRows");
								foreach (MatrixStaticRow CurrentRow in _matrixStaticRowList)
									CurrentRow.Render(_xmlWriter);
							_xmlWriter.WriteEndElement(); // StaticRows
						_xmlWriter.WriteEndElement(); // RowGrouping
					_xmlWriter.WriteEndElement(); // RowGroupings
					RdlRender.AddLine(_xmlWriter, "NoRows", Resource.GetString("RES_REPORT_NOROWSRETURNED"));
					_xmlWriter.WriteEndElement(); // Matrix
				_xmlWriter.WriteEndElement(); // ReportItems

				RdlRender.AddLine(_xmlWriter, "ColumnSpacing", "1cm");

				_xmlWriter.WriteEndElement(); // Body


				// DataSources
				_xmlWriter.WriteStartElement("DataSources");
					_xmlWriter.WriteStartElement("DataSource");
					_xmlWriter.WriteAttributeString("Name", "DataSource1");
				//		_xmlWriter.WriteStartElement("ConnectionProperties");
						RdlRender.AddLine( _xmlWriter, "DataSourceReference", "/" + _dataSource);
		
		//					RdlRender.AddLine(_xmlWriter, "DataProvider", "SQL");
		//					RdlRender.AddLine(_xmlWriter, "ConnectString", _dbConnectionString);
		//					RdlRender.AddLine(_xmlWriter, "IntegratedSecurity", "false");
		//					RdlRender.AddLine(_xmlWriter, "Prompt", "false");
		
		//				_xmlWriter.WriteEndElement(); // ConnectionProperties
					_xmlWriter.WriteEndElement(); // DataSource
				_xmlWriter.WriteEndElement(); // DataSources


				// Datasets
				_xmlWriter.WriteStartElement("DataSets");
					foreach (Dataset CurrentDataset in _datasetList)
						CurrentDataset.Render(_xmlWriter);
				_xmlWriter.WriteEndElement();

				// ReportParameters
				if (_reportParameterList != null && _reportParameterList.Count > 0)
				{
					_xmlWriter.WriteStartElement("ReportParameters");
					foreach (ReportParameter CurrentParameter in _reportParameterList)
						CurrentParameter.Render(_xmlWriter);
					_xmlWriter.WriteEndElement();
				}

				// EmbeddedImages
				_xmlWriter.WriteStartElement("EmbeddedImages");
					_xmlWriter.WriteStartElement("EmbeddedImage");
					_xmlWriter.WriteAttributeString("Name", "Esf");
					RdlRender.AddLine(_xmlWriter, "MIMEType", "image/bmp");
						_xmlWriter.WriteStartElement("ImageData");

						// Image data
						FileStream ImageStream = new FileStream(HttpContext.Current.Server.MapPath(
                            ConfigurationManager.AppSettings["SmallSystemsLogoImage"]), FileMode.Open, FileAccess.Read);
						byte[] ImageByteArray = new Byte[ImageStream.Length];
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

			RdlRender.AddTextbox(_xmlWriter, "ReportDescriptionTextBox", _reportDescription,
				BoldBorderedTextBoxStyle, "", "0.25cm", "1.0cm", "16.00cm", "true", "0.5cm", "1",
				"", "", "", "", "", "", "", "");
			
	
			//Report Filters
		
			foreach (ReportFilterBusinessObject _CurrentFilter in _filterItemList)
			{
				FilterCount++;
				_Filter = "Filter" + FilterCount.ToString();
				
				_FilterDescription = "Data Filter Applied : " + _CurrentFilter.DataItemCaption + " " + _CurrentFilter.Operand + " " + _CurrentFilter.FilterValue;
				RdlRender.AddTextbox(_xmlWriter,_Filter,_FilterDescription,BoldBorderedTextBoxStyle, "", "0.25cm", "1.00cm", "16.00cm", "true", "0.5cm", "1",
					"", "", "", "", "", "", "", "");
			}
			

			_xmlWriter.WriteEndElement(); // Report
			_xmlWriter.WriteEndDocument();
			_xmlWriter.Flush();

			_memoryStream.Close();
			return _memoryStream.GetBuffer();

			//_stream.Close();
			//return new byte[0];
		}

		#endregion


        public void Render2010(XmlWriter xmlWriter)
        {
            throw new NotImplementedException();
        }
    }
}