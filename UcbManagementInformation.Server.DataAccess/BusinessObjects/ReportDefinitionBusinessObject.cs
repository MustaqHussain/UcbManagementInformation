using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ComponentModel.DataAnnotations;
using System.Data.Objects.DataClasses;
using System.ServiceModel.DomainServices.Server;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    /*----------------------------------------------------------------------
  Name: ReportDefinitionBusinessObject

  Description: Used as the placeholder for the parameters required for 
               generating a report. These parameters are held in
               state objects in the UI process layer.
             
  History:
  --------
  12 Mar 2005   1.00    LL  Genesis
  13 Apr 2005   1.01    LL  Change made to code review
  20 Apr 2005   1.02    LL  Renamed Class from ReportParameterBusinessObject
  09 Jan 2006   1.03    DM  Added IsDataMapDisplayed
  30 Jan 2006   1.04    DS  Added OuterJoin
------------------------------------------------------------------------*/
    // Date last code review: 11/04/2005 KB




    /// <summary>
    /// Contains fields for state objects on create report screens
    /// </summary>
    public class ReportDefinitionBusinessObject 
    {
        // Code review issue 11/04/2005 - No descriptions for these data members
        //13/04/05 LL added description 
        #region instance variable declaration
        /// <summary>
        /// Private field that contains an arraylist of selected Data Items for report
        /// </summary>
        private List<DataItem> _selectedDataItems;
        private List<DataItem> _fieldDataItems;
        /// <summary>
        /// Private field that contains an arraylist of specified Row Total Data Items for report
        /// </summary>
        private List<DataItem> _rowTotalDataItems;
        /// <summary>
        /// Private field that indicates if Page Break is allowed for report.
        /// </summary>
        private bool _isPageBreak;
        /// <summary>
        /// Private field that indicates if Drill Down is allowed for report.
        /// </summary>
        private bool _isSummaryReport;
        /// <summary>
        /// Private field that indicates if summary report.
        /// </summary>
        private bool _isDrillDown;
        /// <summary>
        /// Private field that indicates if Drill Down is allowed for report.
        /// </summary>
        private bool _isDataMapDisplayed;
        /// <summary>
        /// Private field that indicates if outer joins used for report.
        /// </summary>
        private bool _isOuterJoin;
        /// <summary>
        /// Private field that indicates if Column Total is allowed for report.
        /// </summary>
        private bool _isColumnTotal;
        /// <summary>
        /// Private field that contains an arraylist of specified Column Total Data Items for report
        /// </summary>
        private List<DataItem> _columnTotalDataItems;
        /// <summary>
        /// Private field that contains an arraylist of specified Parameter Data Items for report
        /// </summary>
        private List<DataItem> _parameterDataItems;
        /// <summary>
        /// Private field that contains an arraylist of specified Filter Data Items for report
        /// </summary>
        private List<ReportFilterBusinessObject> _filterList;
        /// <summary>
        /// Private field that contains the report join list
        /// </summary>
        private List<ReportDataTableJoin> _joinList;
        /// <summary>
        /// Private field that contains the report name
        /// </summary>
        private string _reportName;
        /// <summary>
        /// Private field that contains the report description
        /// </summary>
        private string _reportDescription;
        /// <summary>
        /// Private field that contains the report path/location
        /// </summary>
        private string _reportPath;
        /// <summary>
        /// Private field that contains the report group code
        /// </summary>
        private string _selectedReportGroupCode;
        /// <summary>
        /// Private field that contains the report code for the existing report
        /// </summary>
        private string _reportCode;
        #endregion

        #region private constructor
        /// <summary>
        ///Class constructor
        /// </summary>
        public ReportDefinitionBusinessObject()
        {
            //initialise instance variables
            _selectedDataItems = new List<DataItem>();
            _rowTotalDataItems = new List<DataItem>();
            _isPageBreak = false;
            _isDrillDown = false;
            _isSummaryReport = false;
            _isColumnTotal = false;
            _columnTotalDataItems = new List<DataItem>();
            _parameterDataItems = new List<DataItem>();
            _filterList = new List<ReportFilterBusinessObject>();
            _joinList = new List<ReportDataTableJoin>();
            _reportName = "";
            _reportDescription = "";
            _reportPath = "";
            _selectedReportGroupCode = "";
            _reportCode = null;
            _isDataMapDisplayed = false;
            _isOuterJoin = false;
        }
        #endregion

        // Code review issue 11/04/2005 - Spelling error in summaries
        //13/04/2005 changed spelling for word 'public'   
        #region public property
        /// <summary>
        /// Public property for getting and setting the selected Data Items for report
        /// </summary>
        public List<DataItem> SelectedDataItems
        {
            get
            {
                return _selectedDataItems;
            }
            set
            {
                _selectedDataItems = value;
            }
        }
        public List<DataItem> FieldDataItems
        {
            get
            {
                return _fieldDataItems;
            }
            set
            {
                _fieldDataItems = value;
            }
        }
        public List<ReportChartBusinessObject> ChartList
        { get; set; }
        /// <summary>
        /// Public property for getting and setting the RowTotalDataItems for report
        /// </summary>
        public List<DataItem> RowTotalDataItems
        {
            get
            {
                return _rowTotalDataItems;
            }
            set
            {
                _rowTotalDataItems = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the IsPageBreak for report
        /// </summary>
        public bool IsPageBreak
        {
            get
            {
                return _isPageBreak;
            }
            set
            {
                _isPageBreak = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the IsDrillDown for report
        /// </summary>
        public bool IsDrillDown
        {
            get
            {
                return _isDrillDown;
            }
            set
            {
                _isDrillDown = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the IsDrillDown for report
        /// </summary>
        public bool IsSummaryReport
        {
            get
            {
                return _isSummaryReport;
            }
            set
            {
                _isSummaryReport = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the IsColumnTotal for report
        /// </summary>
        public bool IsColumnTotal
        {
            get
            {
                return _isColumnTotal;
            }
            set
            {
                _isColumnTotal = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the ColumnTotalDataItems for report
        /// </summary>
        public List<DataItem> ColumnTotalDataItems
        {
            get
            {
                return _columnTotalDataItems;
            }
            set
            {
                _columnTotalDataItems = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the ParameterDataItems for report
        /// </summary>
        public List<DataItem> ParameterDataItems
        {
            get
            {
                return _parameterDataItems;
            }
            set
            {
                _parameterDataItems = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the FilterList for report
        /// </summary>
        public List<ReportFilterBusinessObject> FilterList
        {
            get
            {
                return _filterList;
            }
            set
            {
                _filterList = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the Join List for report
        /// </summary>
        public List<ReportDataTableJoin> JoinList
        {
            get
            {
                return _joinList;
            }
            set
            {
                _joinList = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the ReportName for report
        /// </summary>
        public string ReportName
        {
            get
            {
                return _reportName;
            }
            set
            {
                _reportName = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the ReportDescription for report
        /// </summary>
        public string ReportDescription
        {
            get
            {
                return _reportDescription;
            }
            set
            {
                _reportDescription = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the ReportPath for report
        /// </summary>
        public string ReportPath
        {
            get
            {
                return _reportPath;
            }
            set
            {
                _reportPath = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the selected ReportGroupCode for report
        /// </summary>
        public string SelectedReportGroupCode
        {
            get
            {
                return _selectedReportGroupCode;
            }
            set
            {
                _selectedReportGroupCode = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the ReportCode for report
        /// </summary>
        public string ReportCode
        {
            get
            {
                return _reportCode;
            }
            set
            {
                _reportCode = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the IsDataMapDisplayed for report
        /// </summary>
        public bool IsDataMapDisplayed
        {
            get
            {
                return _isDataMapDisplayed;
            }
            set
            {
                _isDataMapDisplayed = value;
            }
        }
        /// <summary>
        /// Public property for getting and setting the IsOuterJoin for report
        /// </summary>
        public bool IsOuterJoin
        {
            get
            {
                return _isOuterJoin;
            }
            set
            {
                _isOuterJoin = value;
            }
        }
        #endregion
    }
}


