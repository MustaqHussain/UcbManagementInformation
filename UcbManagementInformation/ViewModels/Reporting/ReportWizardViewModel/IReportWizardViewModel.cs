using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.Models;
using System.Collections.ObjectModel;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web;
using UcbManagementInformation.Server.DataAccess;

namespace UcbManagementInformation.ViewModels
{
    public interface IReportWizardViewModel
    {

        DataModel SelectedDataModel { get; set; }

        ObservableCollection<Operand> AllOperands { get; set; }
        ObservableCollection<string> AvailableCharts { get; set; }
        ObservableCollection<Guid> AvailableTables { get; set; }
        ObservableCollection<ConsoleChart> ChartList { get; set; }
        ObservableCollection<DataTableJoin> JoinList { get; set; }
        ObservableCollection<DataCategory> DataCategories { get; set; }
        ObservableCollection<DataItem> SelectedDataItems { get; set; }
        ObservableCollection<DataModel> DataModels { get; set; }
        ObservableCollection<DataTableRelationship> DataTableRelationships { get; set; }
        ObservableCollection<UcbManagementInformation.Models.Filter> Filters { get; set; }

        DataItem CurrentDataItem { get; set; }
        DataItem CurrentDataItemForCategory { get; set; }
        DataItem CurrentSelectedDataItem { get; set; }

        ConsoleChart SelectedReportChart { get; set; }

        bool IsDataMap { get; set; }
        bool IsDrillDown { get; set; }
        bool IsGeneratingReport { get; set; }
        bool IsLoadingModel { get; set; }
        bool IsPageOnFirstGroup { get; set; }
        bool IsSummary { get; set; }
        
        RelayCommand ItemDroppedOnChartCategoryCommand { get; set; }
        RelayCommand ItemDroppedOnChartSeriesCommand { get; set; }
        RelayCommand AddChart { get; set; }
        RelayCommand AdvancedJoin { get; set; }
        RelayCommand RemoveDataItem { get; set; }
        RelayCommand RemoveFilter { get; set; }
        RelayCommand DataItemDroppedOnFilter { get; set; }
        RelayCommand DataItemDroppedOnSelected { get; set; }
        RelayCommand DataItemDroppedOnSelectedSource { get; set; }
        RelayCommand DragStartedFrom { get; set; }
        RelayCommand GenerateReport { get; set; }
        RelayCommand QuickView
        {
            get;
            set;
        }RelayCommand NewReportCommand { get; set; }

        
    }
}