﻿#pragma checksum "C:\DWP_AD_WEB\DWP_ADEP_UCB_New\Main-Rel_1\Source\UcbManagementInformation\UcbManagementInformation\Views\Reporting\ReportWizard\ReportWizard.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "EA6D5E63AE7C3FF032D811DC2D39C663"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.1026
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Automation.Peers;
using System.Windows.Automation.Provider;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Resources;
using System.Windows.Shapes;
using System.Windows.Threading;


namespace UcbManagementInformation.Views.Reporting.ReportWizard {
    
    
    public partial class ReportWizard : System.Windows.Controls.Page {
        
        internal System.Windows.Controls.Page ThePage;
        
        internal System.Windows.Controls.Grid LayoutRoot;
        
        internal System.Windows.VisualStateGroup ChartStates;
        
        internal System.Windows.VisualState ShowingChart;
        
        internal System.Windows.VisualState HiddenChart;
        
        internal System.Windows.VisualStateGroup AdvancedPanelStates;
        
        internal System.Windows.VisualState Showing;
        
        internal System.Windows.VisualState Hidden;
        
        internal System.Windows.Controls.BusyIndicator ViewBusyIndicator;
        
        internal System.Windows.Controls.Grid SelectDataGrid;
        
        internal System.Windows.Controls.TreeView DataItemsTreeView;
        
        internal System.Windows.Controls.Button GenerateReport;
        
        internal System.Windows.Controls.Button QuickView;
        
        internal System.Windows.Controls.Primitives.ToggleButton ToggleAdvanced;
        
        internal System.Windows.Controls.ListBox SelectedDataItemListBox;
        
        internal System.Windows.Controls.StackPanel AdvancedPanel;
        
        internal System.Windows.Controls.Button AdvancedJoins;
        
        internal System.Windows.Controls.Button ChartingButton;
        
        internal System.Windows.Controls.Grid ChartGrid;
        
        internal System.Windows.Media.TranslateTransform ChartTranslateTransform;
        
        internal System.Windows.Controls.Button AddChartButton;
        
        internal System.Windows.Controls.Button CloseChartButton;
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Windows.Application.LoadComponent(this, new System.Uri("/UcbManagementInformation;component/Views/Reporting/ReportWizard/ReportWizard.xam" +
                        "l", System.UriKind.Relative));
            this.ThePage = ((System.Windows.Controls.Page)(this.FindName("ThePage")));
            this.LayoutRoot = ((System.Windows.Controls.Grid)(this.FindName("LayoutRoot")));
            this.ChartStates = ((System.Windows.VisualStateGroup)(this.FindName("ChartStates")));
            this.ShowingChart = ((System.Windows.VisualState)(this.FindName("ShowingChart")));
            this.HiddenChart = ((System.Windows.VisualState)(this.FindName("HiddenChart")));
            this.AdvancedPanelStates = ((System.Windows.VisualStateGroup)(this.FindName("AdvancedPanelStates")));
            this.Showing = ((System.Windows.VisualState)(this.FindName("Showing")));
            this.Hidden = ((System.Windows.VisualState)(this.FindName("Hidden")));
            this.ViewBusyIndicator = ((System.Windows.Controls.BusyIndicator)(this.FindName("ViewBusyIndicator")));
            this.SelectDataGrid = ((System.Windows.Controls.Grid)(this.FindName("SelectDataGrid")));
            this.DataItemsTreeView = ((System.Windows.Controls.TreeView)(this.FindName("DataItemsTreeView")));
            this.GenerateReport = ((System.Windows.Controls.Button)(this.FindName("GenerateReport")));
            this.QuickView = ((System.Windows.Controls.Button)(this.FindName("QuickView")));
            this.ToggleAdvanced = ((System.Windows.Controls.Primitives.ToggleButton)(this.FindName("ToggleAdvanced")));
            this.SelectedDataItemListBox = ((System.Windows.Controls.ListBox)(this.FindName("SelectedDataItemListBox")));
            this.AdvancedPanel = ((System.Windows.Controls.StackPanel)(this.FindName("AdvancedPanel")));
            this.AdvancedJoins = ((System.Windows.Controls.Button)(this.FindName("AdvancedJoins")));
            this.ChartingButton = ((System.Windows.Controls.Button)(this.FindName("ChartingButton")));
            this.ChartGrid = ((System.Windows.Controls.Grid)(this.FindName("ChartGrid")));
            this.ChartTranslateTransform = ((System.Windows.Media.TranslateTransform)(this.FindName("ChartTranslateTransform")));
            this.AddChartButton = ((System.Windows.Controls.Button)(this.FindName("AddChartButton")));
            this.CloseChartButton = ((System.Windows.Controls.Button)(this.FindName("CloseChartButton")));
        }
    }
}

