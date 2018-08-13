using System;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web;
using UcbManagementInformation.Server.DataAccess;
using System.ServiceModel.DomainServices.Client;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using UcbManagementInformation.Web.Server;
using System.Collections.Generic;


namespace UcbManagementInformation.ViewModels
{
    public class FakeReportWizardViewModel : ViewModel, IReportWizardViewModel
    {

        static FakeReportWizardViewModel()
        {
            _selectedDataModel = new DataModel { Name = "Managing Authority Data Model", Description = "A Model for producing MA Reports" };
            _allOperands = new ObservableCollection<Operand>()
            {
                new Operand { Name="Greater Than",Value=">" },
                new Operand { Name="Equal to",Value="="} 
            };
            _availableCharts = new ObservableCollection<string>()
            {
                "Column","Pie","Line"
            };
            _availableTables = new ObservableCollection<Guid>()
            {
                Guid.NewGuid()
            };
            _chartList = new ObservableCollection<ConsoleChart>()
            {
                new ConsoleChart{ Title="Chart1",ChartStyle="Column",ChartType="Column",Description="My Chart",
                    CategoryDataItems = new ObservableCollection<DataItem>
                    { 
                        new DataItem{Name="Field1",Caption="Field 1"}
                    },
                    SeriesDataItems= new ObservableCollection<DataItem>
                    { 
                        new DataItem{Name="Field2",Caption="Field 2"},
                        new DataItem{Name="Field3",Caption="Field 3"}
                    },
                },
                new ConsoleChart{ Title="Chart2",ChartStyle="Pie",ChartType="Pie",Description="My Chart 2",
                    CategoryDataItems = new ObservableCollection<DataItem>
                    { 
                        new DataItem{Name="Field1",Caption="Field 1"}
                    },
                    SeriesDataItems= new ObservableCollection<DataItem>
                    { 
                        new DataItem{Name="Field2",Caption="Field 2"},
                        new DataItem{Name="Field3",Caption="Field 3"}
                    },
                }
            };
            _joinList = new ObservableCollection<DataTableJoin>()
            {
                new DataTableJoin(){FromField="Code",FromTable="Priority",ToField="PriorityCode",ToTable="Agreement"},
                new DataTableJoin(){FromField="Code",FromTable="Region",ToField="RegionCode",ToTable="Agreement"},
            };
            _dataCategories = new ObservableCollection<DataCategory>
                     {
                       new DataCategory 
                       {
                           Name = "Agreement Data"
                       },
                     
                       new DataCategory {Name = "Payment Data"}
                     };
            //_dataCategories[0].DataItems.Add(
            //                new DataItem
            //                {
            //                    Name = "Agreement Number",
            //                    DataCategory = _dataCategories[0],
            //                    Description = "The identifier for the Agreement",
            //                    IsRowGrouping = true,
            //                    IsColumnGrouping = true
            //                }
            //                    );
            //_dataCategories[1].DataItems.Add(
            //                new DataItem {Name = "End Date", DataCategory=_dataCategories[1],Description="The end date for the Agreement"}
                       
            //                    );

            _selectedDataItems = new ObservableCollection<DataItem>
                     {
                       new DataItem {Caption="Agreement Number",Name = "Agreement Number", DataCategory=_dataCategories[0],Description="The identifier for the Agreement",IsColumnGrouping=true,IsGroup=true},
                       new DataItem {Caption = "Agreement Status",Name = "Agreement Status", DataCategory=_dataCategories[0],Description="The status for the Agreement",IsColumnGrouping=true,IsGroup=true},
                       new DataItem {Caption = "Agreement Description",Name = "Agreement Description", DataCategory=_dataCategories[0],Description="The description of the Agreement",IsRowGrouping=true,IsGroup=true},
                       new DataItem {Caption = "Start Date",Name = "Start Date", DataCategory=_dataCategories[0],Description="The start date for the Agreement",IsRowGrouping=true,IsGroup=true},
                       new DataItem {Caption = "End Date",Name = "End Date", DataCategory=_dataCategories[1],Description="The end date for the Agreement"},
                       new DataItem {Caption = "Payment Amount",Name = "Payment Amount", DataCategory=_dataCategories[1],Description="The payment amount"},
                       new DataItem {Caption = "Invoice Number",Name = "Invoice Number", DataCategory=_dataCategories[1],Description="The identifier for the payment invoice"}
                     };
            _dataModels = new ObservableCollection<DataModel>
             {
                 new DataModel {Name="Managing Authority",Description="A Data Model For managing Authority" },
                 new DataModel {Name="Finance",Description="A Data Model Forfinance" },
                 new DataModel {Name="Model 3",Description="A Data Model For managing 3" }
             };
            _filters = new ObservableCollection<Models.Filter>
            {
                new Models.Filter 
                {
                    DataItemToFilter=_selectedDataItems[0],
                    DropDownVisibility=true,
                    ExistingValues=new ObservableCollection<string>{"1","2"},
                    OperandChosen= new Operand{ Name="Equal",Value="Equals"},
                    PossibleOperands= new ObservableCollection<Operand>
                    {
                        new Operand{ Name="Equal",Value="Equals"},
                        new Operand{ Name="GreaterThan",Value="Greater Than"}
                    },
                    TextBoxVisibility=false,
                    CalendarVisibility=false,
                    Value="2"
                }
            };

           
        }

        private static DataModel _selectedDataModel;
        public DataModel SelectedDataModel
        {
            get
            {
                return _selectedDataModel;
            }
            set
            {
                _selectedDataModel = value;
            }
        }

        private static ObservableCollection<Operand> _allOperands = new ObservableCollection<Operand>();

        public ObservableCollection<Operand> AllOperands
        {
            get
            {
                return _allOperands;
            }
            set
            {
                _allOperands = value;
            }
        }

        private static ObservableCollection<string> _availableCharts = new ObservableCollection<string>();

        public ObservableCollection<string> AvailableCharts
        {
            get
            {
                return _availableCharts;
            }
            set
            {
                _availableCharts = value;
            }
        }

        private static ObservableCollection<Guid> _availableTables = new ObservableCollection<Guid>();
        public ObservableCollection<Guid> AvailableTables
        {
            get
            {
                return _availableTables;
            }
            set
            {
                _availableTables = value;
            }
        }
        private static ObservableCollection<ConsoleChart> _chartList = new ObservableCollection<ConsoleChart>();
        public ObservableCollection<ConsoleChart> ChartList
        {
            get
            {
                return _chartList;
            }
            set
            {
                _chartList = value;
            }
        }
        private static ObservableCollection<DataTableJoin> _joinList = new ObservableCollection<DataTableJoin>();
        public ObservableCollection<DataTableJoin> JoinList
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
        private static ObservableCollection<DataCategory> _dataCategories = new ObservableCollection<DataCategory>();
        public ObservableCollection<DataCategory> DataCategories
        {
            get
            {
                return _dataCategories;
            }
            set
            {
                _dataCategories = value;
            }
        }

        private static ObservableCollection<DataItem> _selectedDataItems = new ObservableCollection<DataItem>();
        public ObservableCollection<DataItem> SelectedDataItems
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

        private static ObservableCollection<DataModel> _dataModels = new ObservableCollection<DataModel>();
        public ObservableCollection<DataModel> DataModels
        {
            get
            {
                return _dataModels;
            }
            set
            {
                _dataModels = value;
            }
        }

        private static ObservableCollection<DataTableRelationship> _dataTableRelationships = new ObservableCollection<DataTableRelationship>();
        public ObservableCollection<DataTableRelationship> DataTableRelationships
        {
            get
            {
                return _dataTableRelationships;
            }
            set
            {
                _dataTableRelationships = value;
            }
        }

        private static ObservableCollection<Models.Filter> _filters = new ObservableCollection<Models.Filter>();
        public ObservableCollection<Models.Filter> Filters
        {
            get
            {
                return _filters;
            }
            set
            {
                _filters = value;
            }
        }
        private static DataItem _currentDataItem;
        public DataItem CurrentDataItem
        {
            get
            {
                return _currentDataItem;
            }
            set
            {
                _currentDataItem = value;
            }
        }

        private static DataItem _currentDataItemForCategory;
        public DataItem CurrentDataItemForCategory
        {
            get
            {
                return _currentDataItemForCategory;
            }
            set
            {
                _currentDataItemForCategory = value;
            }
        }
        private static DataItem _currentSelectedDataItem;
        public DataItem CurrentSelectedDataItem
        {
            get
            {
                return _currentSelectedDataItem;
            }
            set
            {
                _currentSelectedDataItem = value;
            }
        }
        private static ConsoleChart _selectedReportChart;
        public ConsoleChart SelectedReportChart
        {
            get
            {
                return _selectedReportChart;
            }
            set
            {
                _selectedReportChart = value;
            }
        }
        private static bool isExpanded;
        public  bool IsExpanded
        {
            get { return isExpanded; }
            set { isExpanded = value; }
        }
        private static bool _isDataMap;
        public bool IsDataMap
        {
            get
            {
                return _isDataMap;
            }
            set
            {
                _isDataMap = value;
            }
        }
        private static bool _isDrillDown;
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
        private static bool _isGeneratingReport;
        public bool IsGeneratingReport
        {
            get
            {
                return _isGeneratingReport;
            }
            set
            {
                _isGeneratingReport = value;
            }
        }
        private static bool isLoadingModel;
        public bool IsLoadingModel
        {
            get
            {
                return isLoadingModel;
            }
            set
            {
                isLoadingModel = value;
            }
        }
        private static bool isPageOnFirstGroup;
        public bool IsPageOnFirstGroup
        {
            get
            {
                return isPageOnFirstGroup;
            }
            set
            {
                isPageOnFirstGroup = value;
            }
        }
        private static bool isSummary;
        public bool IsSummary
        {
            get
            {
                return isSummary;
            }
            set
            {
                isSummary = value;
            }
        }

        public RelayCommand ItemDroppedOnChartCategoryCommand
        {
            get;
            set;
        }

        public RelayCommand ItemDroppedOnChartSeriesCommand
        {
            get;
            set;
        }

        public RelayCommand AddChart
        {
            get;
            set;
        }

        public RelayCommand AdvancedJoin
        {
            get;
            set;
        }

        public RelayCommand RemoveDataItem
        {
            get;
            set;
        }

        public RelayCommand RemoveFilter
        {
            get;
            set;
        }

        public RelayCommand DataItemDroppedOnFilter
        {
            get;
            set;
        }

        public RelayCommand DataItemDroppedOnSelected
        {
            get;
            set;
        }

        public RelayCommand DataItemDroppedOnSelectedSource
        {
            get;
            set;
        }

        public RelayCommand DragStartedFrom
        {
            get;
            set;
        }

        public RelayCommand GenerateReport
        {
            get;
            set;
        }
        public RelayCommand QuickView
        {
            get;
            set;
        }
        public RelayCommand NewReportCommand
        {
            get;
            set;
        }
    }
}
            /*
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(FakeReportWizardViewModel_PropertyChanged);

            
            dataItems = new ObservableCollection<DataItem>
                     {
                       new DataItem {Name = "Agreement Number", DataCategory="Agreement Data",Description="The identifier for the Agreement"},
                       new DataItem {Name = "Agreement Status", DataCategory="Agreement Data",Description="The status for the Agreement"},
                       new DataItem {Name = "Agreement Description", DataCategory="Agreement Data",Description="The description of the Agreement"},
                       new DataItem {Name = "Start Date", DataCategory="Agreement Data",Description="The start date for the Agreement"},
                       new DataItem {Name = "End Date", DataCategory="Agreement Data",Description="The end date for the Agreement"},
                       new DataItem {Name = "Payment Amount", DataCategory="Payment Data",Description="The payment amount"},
                       new DataItem {Name = "Invoice Number", DataCategory="Payment Data",Description="The identifier for the payment invoice"}
                     };
             
            dataCategories = new ObservableCollection<DataCategory>
                     {
                       new DataCategory {Name = "Agreement Data",Description="Data Associated with Agreements",ChildDataItems=GetDataItemsByCategory("Agreement Data")},
                       new DataCategory {Name = "Payment Data",Description="Data Associated with Payments",ChildDataItems=GetDataItemsByCategory("Payment Data")}
                     };*/
 
