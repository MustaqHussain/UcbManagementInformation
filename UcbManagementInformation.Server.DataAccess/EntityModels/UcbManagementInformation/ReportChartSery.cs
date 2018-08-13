//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel.DataAnnotations;
using System.ServiceModel.DomainServices.Server;

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class ReportChartSery
    {
        #region Primitive Properties
    
        public virtual System.Guid Code
        {
            get;
            set;
        }
    
        public virtual System.Guid ReportChartCode
        {
            get { return _reportChartCode; }
            set
            {
                if (_reportChartCode != value)
                {
                    if (ReportChart != null && ReportChart.Code != value)
                    {
                        ReportChart = null;
                    }
                    _reportChartCode = value;
                }
            }
        }
        private System.Guid _reportChartCode;
    
        public virtual System.Guid SeriesReportItemCode
        {
            get { return _seriesReportItemCode; }
            set
            {
                if (_seriesReportItemCode != value)
                {
                    if (ReportItem != null && ReportItem.Code != value)
                    {
                        ReportItem = null;
                    }
                    _seriesReportItemCode = value;
                }
            }
        }
        private System.Guid _seriesReportItemCode;
    
        public virtual byte[] RowIdentifier
        {
            get;
            set;
        }
    
        public virtual int SortOrder
        {
            get;
            set;
        }

        #endregion
        #region Navigation Properties
    
        public virtual ReportChart ReportChart
        {
            get { return _reportChart; }
            set
            {
                if (!ReferenceEquals(_reportChart, value))
                {
                    var previousValue = _reportChart;
                    _reportChart = value;
                    FixupReportChart(previousValue);
                }
            }
        }
        private ReportChart _reportChart;
    
        public virtual ReportItem ReportItem
        {
            get { return _reportItem; }
            set
            {
                if (!ReferenceEquals(_reportItem, value))
                {
                    var previousValue = _reportItem;
                    _reportItem = value;
                    FixupReportItem(previousValue);
                }
            }
        }
        private ReportItem _reportItem;

        #endregion
        #region Association Fixup
    
        private void FixupReportChart(ReportChart previousValue)
        {
            if (previousValue != null && previousValue.ReportChartSeries.Contains(this))
            {
                previousValue.ReportChartSeries.Remove(this);
            }
    
            if (ReportChart != null)
            {
                if (!ReportChart.ReportChartSeries.Contains(this))
                {
                    ReportChart.ReportChartSeries.Add(this);
                }
                if (ReportChartCode != ReportChart.Code)
                {
                    ReportChartCode = ReportChart.Code;
                }
            }
        }
    
        private void FixupReportItem(ReportItem previousValue)
        {
            if (previousValue != null && previousValue.ReportChartSeries.Contains(this))
            {
                previousValue.ReportChartSeries.Remove(this);
            }
    
            if (ReportItem != null)
            {
                if (!ReportItem.ReportChartSeries.Contains(this))
                {
                    ReportItem.ReportChartSeries.Add(this);
                }
                if (SeriesReportItemCode != ReportItem.Code)
                {
                    SeriesReportItemCode = ReportItem.Code;
                }
            }
        }

        #endregion
    }
}
