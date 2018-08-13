/*----------------------------------------------------------------------
Name:			TableFooter

Description:	Generates a table Footer rendered in RDL for use in a
				TabularReport object.

History:
--------
24 Feb 2005		1.00 KSB	Created.
11 May 2005		1.01 LL		Code Review.
24 May 2005		1.02 KB		Code review amendments.
21 Jul 2005		1.03 LL		TIR0200 - Modified constructor to allow proper
							text alignment
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
//using Dwp.Esf.Mis.CrossLayer;
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Server.Utilities;


namespace UcbManagementInformation.Server.RDL2003Engine
{
	/// <summary>
	/// Generates a table Footer for use in a TabularReport object.
	/// </summary>
	public class TableFooter : IRdlRenderable,IDataModelAware
	{
		#region Private data members
		
		// A collection of TableFooterTextBox objects
		private String _reportDescription ;
		private String _Filter;
		private String _FilterDescription;
		private int FilterCount=0;
		private String _colSpan;

        private List<ReportFilterBusinessObject> _filterItemList = new List<ReportFilterBusinessObject>();
        public DataModel CurrentDataModel { get; set; }
		
		#endregion
		#region Constants
		//The value for Current Month passed in through filter value
		private const string CURRENTMONTH = "Current Month";
		#endregion
		#region Constructors

		/// <summary>
		/// Generates a table Footer for use in a TabularReport object.
		/// </summary>
		/// <param name="fieldList">List of selected data items.</param>
		public TableFooter(string reportDescription, string colSpan, List<ReportFilterBusinessObject> filterList,DataModel currentDataModel)
		{
            CurrentDataModel = currentDataModel;
				 _reportDescription = "Report Description : " + reportDescription;
				_colSpan = colSpan;

			foreach (ReportFilterBusinessObject CurrentFilter in filterList)
			{
				//TODO: Code Review Issue 11/05/05: Do not use 'my' in variable names. 
				//Choose a meaningful name. Pascal casing for variables.
				// Done 24/05/2005
                Guid CodeForRepository = Guid.Parse(CurrentFilter.DataItemCode);
                DataItem CurrentItem = CurrentDataModel.DataTables.SelectMany(x => x.DataItems).Single(y => y.Code == CodeForRepository);//DataAccessUtilities.RepositoryLocator<IDataItemRepository>().GetByCode(CodeForRepository);

                DataTable CurrentTable = CurrentDataModel.DataTables.Single(x=>x.Code==CurrentItem.DataTableCode);//DataAccessUtilities.RepositoryLocator<IDataTableRepository>().GetByCode(CurrentItem.DataTableCode);

				//25/08/05 LL - TIR0289 convert 'Current Month' to appropriate data format 
				if(CurrentFilter.FilterValue == CURRENTMONTH)
					CurrentFilter.FilterValue = DateUtility.GetCurrentYearMonth();
				ReportFilterBusinessObject ReportFilter = new ReportFilterBusinessObject() ;
				ReportFilter.DataItemCaption=CurrentItem.Caption;
				ReportFilter.Operand=CurrentFilter.Operand;
				ReportFilter.FilterValue =CurrentFilter.FilterValue;
				_filterItemList.Add(ReportFilter);
			}
		}
		#endregion

		//TODO: Code Review Issue 11/05/05: Give a more detailed summary.
		// Done 24/05/2005
		/// <summary>
		/// Makes object render itself as an RDL Footer type to an XmlWriter instance.
		/// </summary>
		/// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
		public void Render( XmlWriter xmlWriter)
		{
			//TODO: Code Review Issue 11/05/05: indent the code as in XML to increase readability
			// Done 24/05/2005

			ReportingServicesStyle _textboxStyle10pt = new ReportingServicesStyle(
				ReportingServicesStyle.TextBoxStyle.TabularReport10pt);
			//Footer start
			xmlWriter.WriteStartElement("Footer");
			RdlRender.AddLine( xmlWriter, "RepeatOnNewPage", "true");
			xmlWriter.WriteStartElement("TableRows");
			//Report Description
			xmlWriter.WriteStartElement("TableRow");
			xmlWriter.WriteStartElement("TableCells");
			xmlWriter.WriteStartElement("TableCell");
			xmlWriter.WriteStartElement("ColSpan");
			xmlWriter.WriteString(_colSpan);
			xmlWriter.WriteEndElement();
			xmlWriter.WriteStartElement("ReportItems");
			RdlRender.AddTextbox(xmlWriter,"reportDescription",_reportDescription,_textboxStyle10pt, "", "0.25cm", "15.00cm", "3.00cm", "true", "0.5cm", "1",
				"", "", "", "", "", "", "", "");
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
			RdlRender.AddLine( xmlWriter, "Height", ".25in");
			xmlWriter.WriteEndElement();
			


			//Report Filters
			foreach (ReportFilterBusinessObject _CurrentFilter in _filterItemList)
			{
				FilterCount++;
				_Filter = "Filter" + FilterCount.ToString();
				xmlWriter.WriteStartElement("TableRow");
				xmlWriter.WriteStartElement("TableCells");
				xmlWriter.WriteStartElement("TableCell");
				xmlWriter.WriteStartElement("ColSpan");
				xmlWriter.WriteString(_colSpan);
				xmlWriter.WriteEndElement();
				xmlWriter.WriteStartElement("ReportItems");
				_FilterDescription = "Data Filter Applied : " + _CurrentFilter.DataItemCaption + " " + _CurrentFilter.Operand + " " + _CurrentFilter.FilterValue;
				RdlRender.AddTextbox(xmlWriter,_Filter,_FilterDescription,_textboxStyle10pt, "", "0.25cm", "15.00cm", "3.00cm", "true", "0.5cm", "1",
					"", "", "", "", "", "", "", "");	
				xmlWriter.WriteEndElement();  //ReportItems
				xmlWriter.WriteEndElement();//Tablecell
				xmlWriter.WriteEndElement();//Tablecells
				RdlRender.AddLine( xmlWriter, "Height", ".25in");
				xmlWriter.WriteEndElement();//TableRow
				
			
			}
			
		


			//Footer end
			xmlWriter.WriteEndElement();
			xmlWriter.WriteEndElement();
		}


        /// <summary>
        /// Makes object render itself as an RDL Footer type to an XmlWriter instance.
        /// </summary>
        /// <param name="xmlWriter">The XMLWriter to be rendered to.</param>
        public void Render2010(XmlWriter xmlWriter)
        {
            //TODO: Code Review Issue 11/05/05: indent the code as in XML to increase readability
            // Done 24/05/2005

            ReportingServicesStyle _textboxStyle10pt = new ReportingServicesStyle(
                ReportingServicesStyle.TextBoxStyle.TabularReport10pt);
            //RdlRender.AddLine(xmlWriter, "RepeatOnNewPage", "true");
            //Report Description
            xmlWriter.WriteStartElement("TablixRow");
            RdlRender.AddLine(xmlWriter, "Height", ".25in");
            xmlWriter.WriteStartElement("TablixCells");
            xmlWriter.WriteStartElement("TablixCell");
            xmlWriter.WriteStartElement("CellContents");
            xmlWriter.WriteStartElement("ColSpan");
            xmlWriter.WriteString(_colSpan);
            xmlWriter.WriteEndElement();//ColSpan
            RdlRender.AddTextbox2010(xmlWriter, "reportDescription", _reportDescription, _textboxStyle10pt, "", "0.25cm", "15.00cm", "3.00cm", "true", "0.5cm", "1",
                "", "", "", "", "", "", "", "");
            xmlWriter.WriteEndElement();//CellContents
            xmlWriter.WriteEndElement();//TablixCell
            for (int x = 0; x < (int.Parse(_colSpan) - 1); x++)
            {
                xmlWriter.WriteStartElement("TablixCell");
                xmlWriter.WriteEndElement();//TablixCell
            }
            xmlWriter.WriteEndElement();//TablixCells
            xmlWriter.WriteEndElement();//TablixRow



            //Report Filters
            foreach (ReportFilterBusinessObject _CurrentFilter in _filterItemList)
            {
                FilterCount++;
                _Filter = "Filter" + FilterCount.ToString();
                xmlWriter.WriteStartElement("TablixRow");
                RdlRender.AddLine(xmlWriter, "Height", ".25in");
                xmlWriter.WriteStartElement("TablixCells");
                xmlWriter.WriteStartElement("TablixCell");
                xmlWriter.WriteStartElement("CellContents");
                xmlWriter.WriteStartElement("ColSpan");
                xmlWriter.WriteString(_colSpan);
                xmlWriter.WriteEndElement();//ColSpan
                _FilterDescription = "Data Filter Applied : " + _CurrentFilter.DataItemCaption + " " + _CurrentFilter.Operand + " " + _CurrentFilter.FilterValue;
                RdlRender.AddTextbox2010(xmlWriter, _Filter, _FilterDescription, _textboxStyle10pt, "", "0.25cm", "15.00cm", "3.00cm", "true", "0.5cm", "1",
                    "", "", "", "", "", "", "", "");
                xmlWriter.WriteEndElement();//CellContents
                xmlWriter.WriteEndElement();//TablixCell
                for (int x = 0; x < (int.Parse(_colSpan) - 1); x++)
                {
                    xmlWriter.WriteStartElement("TablixCell");
                    xmlWriter.WriteEndElement();//TablixCell
                }
                xmlWriter.WriteEndElement();//TablixCells
                xmlWriter.WriteEndElement();//TablixRow


            }




            
        }
	}
}
