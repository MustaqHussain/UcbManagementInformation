using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects.DataClasses;
using System.ComponentModel.DataAnnotations;

namespace UcbManagementInformation.Server.DataAccess.BusinessObjects
{
    /*----------------------------------------------------------------------
  Name: ReportFilterBusinessObject

  Description: Implements a Business Object (data abstraction) for Filter
               This Business Object has all public properties ready for use 
               for XML Serialization.
             
  History:
  --------
  16 Mar 2005   1.00 LL Genesis. 
  13 Apr 2005   1.01 LL Changed spelling for word 'public'   
------------------------------------------------------------------------*/
    // Date last code review: 11/04/2005 KB


    /// <summary>
    /// Contains fields and properties for ReportFilterBusinessObject,
    /// which is used as a placeholder for report filter definition.
    /// </summary>
    public class ReportFilterBusinessObject : IComparable
    {
        #region Private Fields (Data Store) Definitions

        /// <summary>
        /// Private field that contains The unique code for the filter.
        /// </summary>
        private string _code;

        /// <summary>
        /// Private field that contains The operand for the filter.
        /// </summary>
        private string _operand;

        /// <summary>
        /// Private field that contains The value for the filter
        /// </summary>
        private string _filterValue;

        /// <summary>
        /// Private field that contains The reportItem to which the filter applies
        /// </summary>
        private string _reportItemCode;

        /// <summary>
        /// Private field that contains Order of Report Filter within the report
        /// </summary>
        private int _sortField;

        /// <summary>
        /// Private field that contains The DataItem to which the filter applies
        /// </summary>
        private string _dataItemCode;

        /// <summary>
        /// Private field that contains The caption of DataItem to which the filter applies
        /// </summary>
        private string _dataItemCaption;

        /// <summary>
        /// Private field that contains The IsValueType to indicate if the filter value
        /// is entered or selected
        /// </summary>
        private bool _isValueType;

        /// <summary>
        /// Private field that contains The Filter definition
        /// </summary>
        private string _filterDefinition;

        
        /// <summary>
        /// Private field that contains Timestamp for optimistic locking
        /// </summary>
        private byte[] _rowIdentifier;

        #endregion

        #region Public Property Definitions

        // Code review issue 11/04/2005 - Spelling error in summary
        //13/04/2005 changed spelling for word 'public'   
        /// <summary>
        /// Public property for getting and setting the The unique code for the filter.
        /// Do not ever assign a value to this property except for XML Serialization as unexpected results may occur.
        /// </summary>
       
        public string Code
        {
            get
            {
                return _code;

            }
            set
            {
                _code = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the The operand for the filter.
        /// </summary>
        public string Operand
        {
            get
            {
                return _operand;

            }
            set
            {
                _operand = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the The value for the filter
        /// </summary>
        public string FilterValue
        {
            get
            {
                return _filterValue;

            }
            set
            {
                _filterValue = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the The reportItem to which the filter applies
        /// </summary>
        public string ReportItemCode
        {
            get
            {
                return _reportItemCode;

            }
            set
            {
                _reportItemCode = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the Order of Report Filter within the report
        /// </summary>
        public int SortField
        {
            get
            {
                return _sortField;

            }
            set
            {
                _sortField = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the The DataItem to which the filter applies
        /// </summary>
        public string DataItemCode
        {
            get
            {
                return _dataItemCode;

            }
            set
            {
                _dataItemCode = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the caption of DataItem to which the filter applies
        /// </summary>
        public string DataItemCaption
        {
            get
            {
                return _dataItemCaption;

            }
            set
            {
                _dataItemCaption = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the Is Value Type indicator
        /// </summary>
        public bool IsValueType
        {
            get
            {
                return _isValueType;

            }
            set
            {
                _isValueType = value;
            }
        }

        /// <summary>
        /// Public property for getting and setting the Filter Definition
        /// </summary>
        public string FilterDefinition
        {
            get
            {
                return _filterDefinition;

            }
            set
            {
                _filterDefinition = value;
            }
        }
        
        /// <summary>
        /// Public property for getting and setting the Timestamp for optimistic locking
        /// </summary>
        public byte[] RowIdentifier
        {
            get
            {
                return _rowIdentifier;

            }
            set
            {
                _rowIdentifier = value;
            }
        }


        #endregion

        #region Construction Definitions

        /// <summary>
        /// Public Constructor for an empty Report Filter Business Object
        /// </summary>
        public ReportFilterBusinessObject()
        {
        }
        #endregion

        #region IComparable Members
        /// <summary>
        /// Method required by IComparable interface
        /// To sort the Report Filter arraylist by SortField column
        /// </summary>
        /// <param name="obj">Object to be sorted</param>
        /// <returns>an indication of their relative values</returns>
        public int CompareTo(object obj)
        {
            return this.SortField.CompareTo(((ReportFilterBusinessObject)obj).SortField);
        }
        #endregion

    }
}



