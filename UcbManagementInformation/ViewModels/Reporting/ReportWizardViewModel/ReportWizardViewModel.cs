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
using System.ComponentModel.DataAnnotations;
using UcbManagementInformation.TriggerActions;
using Microsoft.Windows;
using UcbManagementInformation.ServiceLocation;
using UcbManagementInformation.Controls;
using UcbManagementInformation.Helpers;
using System.Windows.Browser;
using System.Collections;


namespace UcbManagementInformation.ViewModels
{
    public class ReportWizardViewModel : ViewModel, IReportWizardViewModel
    {
        private ObservableCollection<DataItem> selectedDataItems = new ObservableCollection<DataItem>();
        private ObservableCollection<UcbManagementInformation.Models.Filter> filters = new ObservableCollection<UcbManagementInformation.Models.Filter>();
        private ObservableCollection<DataCategory> dataCategories = new ObservableCollection<DataCategory>();
        private ObservableCollection<Operand> allOperands = new ObservableCollection<Operand>();
        private DataCategory currentDataCategory = new DataCategory();
        private DataItem currentSelectedDataItem = new DataItem();
        private Report reportDefinition = new Report();
        private string dragFrom = "";
        private ReportWizardContext myContext = new ReportWizardContext();
        //private ReportWizardContext myContextForPopUp; 
        private IModalDialogService modalDialogService;
        private UcbManagementInformation.Models.ReportGroupFolder selectedReportGroup;
        private string selectedReportName;
        private ObservableCollection<string> availableCharts = new ObservableCollection<string>() { "Column"};
        private ObservableCollection<ConsoleChart> chartList = new ObservableCollection<ConsoleChart>();
        private ObservableCollection<DataTableJoin> joinList = new ObservableCollection<DataTableJoin>();
        private ObservableCollection<string> dataTableCodeList = new ObservableCollection<string>();
        private DataModel selectedDataModel = new DataModel();
        private ObservableCollection<DataModel> dataModels = new ObservableCollection<DataModel>();
        private ObservableCollection<DataTableRelationship> dataTableRelatiopnships = new ObservableCollection<DataTableRelationship>();
        private ObservableCollection<Guid> availableTables = new ObservableCollection<Guid>();
        private ObservableCollection<DataTable> allTables = new ObservableCollection<DataTable>();
        private bool isLoadingReport;
        private bool isCategoryComplete;
        private bool isRelationshipComplete;
        private bool isTableComplete;
        private bool isPopulatingReport;
        private ConsoleChart selectedReportChart;
        private Report loadedReport;
        private IMessageBoxService messageBoxService;
        private string editingReport;

        public string EditingReport
        {
            get { return editingReport; }
            set { editingReport = value; OnPropertyChanged("EditingReport"); }
        }
        private string editingReportGroup;

        public string EditingReportGroup
        {
            get { return editingReportGroup; }
            set { editingReportGroup = value; OnPropertyChanged("EditingReportGroup"); }
        }

        public ReportWizardViewModel()
        {
            if (WebContext.Current.User.IsAuthenticated)
            {
                PrepareViewModel();
            }
        }

        private void PrepareViewModel()
        {
            this.PropertyChanged += new System.ComponentModel.PropertyChangedEventHandler(ReportWizardViewModel_PropertyChanged);

            myContext.Load<DataModel>(myContext.GetDataModelsQuery(), CallbackFromModel, null);
            SelectedDataItems.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(SelectedDataItems_CollectionChanged);
            Filters.CollectionChanged += new System.Collections.Specialized.NotifyCollectionChangedEventHandler(Filters_CollectionChanged);

            allOperands = new ObservableCollection<Operand>
            {
                new Operand { Name = "Greater Than",Value=">"},
                new Operand { Name = "Less Than",Value="<" },
                new Operand { Name = "Greater Than and Equal To",Value=">="},
                new Operand { Name = "Less Than and Equal To",Value="<=" },
                new Operand { Name = "Equal To",Value="=" },
                new Operand { Name = "Not Equal To",Value="<>" },
                new Operand { Name = "Like",Value="LIKE"},
                new Operand { Name = "Is Null",Value="ISNULL"},
                new Operand { Name = "Contains",Value="CONTAINS"},
                new Operand { Name = "Starts With",Value="STARTS"}
            };

            //DataItemDroppedTarget = new RelayCommand<DataItem>(cmd => UpdateSelected(cmd));
            DataItemDroppedOnFilter = new RelayCommand(cmd => UpdateFilter(cmd));
            DataItemDroppedOnSelected = new RelayCommand(cmd => UpdateSelected(cmd));
            DataItemDroppedOnSelectedSource = new RelayCommand(cmd => UpdateSelectedSource(cmd));
            RemoveDataItem = new RelayCommand(cmd => RemoveDataItemFromSelected(cmd));
            RemoveFilter = new RelayCommand(cmd => RemoveFilterFromList(cmd));
            DragStartedFrom = new RelayCommand(cmd => DragStartedFromControl(cmd));
            GenerateReport = new RelayCommand(cmd => SelectFolder(cmd));
            QuickView = new RelayCommand(cmd => QuickViewReport(cmd));
            AdvancedJoin = new RelayCommand(cmd => SelectAdvancedJoins(cmd));
            EditDataItem = new RelayCommand(cmd => EditSelectedDataItem(cmd), can => CanEditDataItem(can));
            AddChart = new RelayCommand(cmd => AddChartToList(cmd));
            PopulatingCommand = new RelayCommand(cmd => PopulateAutoCompleteBox(cmd));//,exe => CanPopulateAutoCompleteBox(exe));

            ItemDroppedOnChartCategoryCommand = new RelayCommand(cmd => UpdateChartCategory(cmd));
            ItemDroppedOnChartSeriesCommand = new RelayCommand(cmd => UpdateChartSeries(cmd));
            this.messageBoxService = SimpleServiceLocator.Instance.Get<IMessageBoxService>();
            NewReportCommand = new RelayCommand(cmd =>
            {
                var result = this.messageBoxService.Show("This will clear the current report selections, are you sure?",
                   "Please confirm", GenericMessageBoxButton.OkCancel);
                if (result == GenericMessageBoxResult.Ok)
                {
                    NewReport(cmd);
                }
            });

            //Guid currentReportCode = (Guid)(App.Session[SessionKey.CurrentReportCode] == null ? Guid.Empty:(Guid)App.Session[SessionKey.CurrentReportCode];
            
        }

        void LoadReport(Guid reportCode)
        {
            isLoadingReport = true; 
            myContext.Load<Report>(myContext.GetReportByCodeQuery(reportCode), LoadReportCallback, null);
        }
        
        void LoadReportCallback(LoadOperation<Report> lo)
        {
            loadedReport = lo.Entities.FirstOrDefault<Report>();
            EditingReport = loadedReport.Name;
            EditingReportGroup = loadedReport.GroupCode.ToString();
                        
            SelectedDataModel = (from x in DataModels where x.Code == loadedReport.DataModelCode select x).FirstOrDefault();
        }

        void Filters_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RecalculateSelectable();
        }

        void SelectedDataItems_CollectionChanged(object sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
        {
            RecalculateSelectable();
        }

        void ReportWizardViewModel_PropertyChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            switch (e.PropertyName)
            {
                case "SelectedDataModel":
                    IsLoadingModel = true;
                    SelectedDataItems.Clear();
                    Filters.Clear();
                    CurrentDataItem = null;
                    CurrentSelectedDataItem = null;
                    selectedReportGroup = null;
                    selectedReportName = null;
                    if (!isLoadingReport)
                    {
                        loadedReport = null;
                    }
                    isCategoryComplete = false;
                    isTableComplete = false;
                    isRelationshipComplete = false;
                    myContext.Load<DataCategory>(myContext.GetDataCategoriesWithDataItemsByDataModelQuery(selectedDataModel.Code), CallbackFromCategory, null);
                    myContext.Load<DataTableRelationship>(myContext.GetDataTableRelationshipsByDataModelQuery(selectedDataModel.Code), CallbackFromRelationship, null);
                    myContext.Load<DataTable>(myContext.GetDataTablesByDataModelQuery(selectedDataModel.Code), CallbackFromTables, null);
                    break;
                case "DataModels":
                    Guid currentReportCode = (Guid)(App.Session[SessionKey.CurrentReportCode] ?? Guid.Empty);
                    if (currentReportCode != null && currentReportCode != Guid.Empty)
                    {
                        LoadReport(currentReportCode);
                        App.Session[SessionKey.CurrentReportCode] = Guid.Empty;
                    }
                    else
                    {
                        NewReport(null);
                    }
                    break;
                case "CurrentDataItemForCategory":
                    EditDataItem.UpdateCanExecuteCommand();
                    break;

            }
        }
        void RecalculateSelectable()
        {
            var selectedTables = GetSelectedTableCodes();
            var tablesList = from x in allTables select x.Code;
            var availableTableList = allTables.Select(t => t.Code).ToList();
            
            foreach (Guid selectedTable in selectedTables)
            {
                var relatedFromTables = DataTableRelationships.Where(dtr => dtr.DataTableFromCode == selectedTable).Select(dtr => dtr.DataTableToCode).Distinct();
                var relatedToTables = DataTableRelationships.Where(dtr => dtr.DataTableToCode == selectedTable).Select(dtr => dtr.DataTableFromCode).Distinct();
                var allRelatedTables = relatedFromTables.Union(relatedToTables).Union(selectedTables);
                availableTableList = availableTableList.Intersect(allRelatedTables).ToList();
            }
            foreach (DataCategory currentCategory in DataCategories)
            {
                foreach (DataItem currentDataItem in currentCategory.DataItems)
                {
                    currentDataItem.IsSelectable = (availableTableList.Contains(currentDataItem.DataTableCode)); 
                }
            }
            
        }
        void CheckSelectable()
        {
            if (isCategoryComplete && isTableComplete && isRelationshipComplete)
            { 
                RecalculateSelectable();
                IsLoadingModel = false;
                //if the model was changed via loading the report then continue setting the report fields
                if (isLoadingReport && !isPopulatingReport)
                {
                    isPopulatingReport = true;
                    selectedReportGroup = new ReportGroupFolder();//
                    IsDrillDown = loadedReport.IsAllowDrilldown;
                    IsDataMap = loadedReport.IsDataMapDisplayed.HasValue ? loadedReport.IsDataMapDisplayed.Value : false;
                    //reportDefinition.IsInitialExpanded = false;
                    //reportDefinition.IsOuterJoin = false;
                    //reportDefinition.IsStandard = false;
                    IsSummary = loadedReport.IsSummaryReport.HasValue ? loadedReport.IsSummaryReport.Value : false;
                    //reportDefinition.ModifiedDate = DateTime.Now;
                    //reportDefinition.ModifiedUserID = "Tester";
                    selectedReportName = loadedReport.Name;
                    //reportDefinition.RowIdentifier = new byte[] { 0 };
                    

                    foreach (ReportItem CurrentReportItem in loadedReport.ReportItems.OrderBy(x=>x.SortField))
                    {
   
                        DataItem itemToAdd = (from x in DataCategories 
                                              select x.DataItems).SelectMany(y => y).Where<DataItem>(x => x.Code == CurrentReportItem.DataItemCode).First<DataItem>();
                        
                        itemToAdd.IsColumnGrouping = CurrentReportItem.IsColumnTotal;
                        itemToAdd.IsParameter = CurrentReportItem.IsParameter;
                        itemToAdd.IsRowGrouping = CurrentReportItem.IsRowTotal;
                        
                        if (CurrentReportItem.IsField)
                        {
                            SelectedDataItems.Add(itemToAdd);
                        }
                        if (CurrentReportItem.IsFilter)
                        {
                            foreach (Server.DataAccess.Filter currentFilter in CurrentReportItem.Filters)
                            {
                                Models.Filter filterToAdd = new Models.Filter();
                                filterToAdd.DataItemToFilter = itemToAdd;
                                filterToAdd.DropDownVisibility = ((!itemToAdd.IsValueType || itemToAdd.DataType == "bit") ? true : false);
                                filterToAdd.TextBoxVisibility = ((itemToAdd.IsValueType && (itemToAdd.DataType != "bit") && itemToAdd.DataType.ToUpper() != "DATETIME") ? true : false);
                                filterToAdd.CalendarVisibility = ((itemToAdd.IsValueType && itemToAdd.DataType.ToUpper() == "DATETIME") ? true : false);
                                filterToAdd.PossibleOperands = GetOperandsByDataType(itemToAdd.DataType, itemToAdd.IsValueType);
                                filterToAdd.OperandChosen = (from x in filterToAdd.PossibleOperands where x.Value == currentFilter.Operand select x).FirstOrDefault();
                                if (itemToAdd.IsValueType && itemToAdd.DataType != "bit")
                                {
                                    filterToAdd.Value = currentFilter.FilterValue;
                                    Filters.Add(filterToAdd);
                                }
                                else
                                {
                                    if (itemToAdd.DataType == "bit")
                                    {
                                        filterToAdd.ExistingValues = new ObservableCollection<string>(new List<string> { "Yes", "No" });
                                        filterToAdd.Value = currentFilter.FilterValue;
                                        Filters.Add(filterToAdd);
                                    }
                                    else
                                    {
                                        filterToAdd.Value = currentFilter.FilterValue;
                                        Filters.Add(filterToAdd);
                                        GetReferenceData(itemToAdd, filterToAdd, currentFilter.FilterValue);
                                    }
                                }
                                //Filters.Add(filterToAdd);
                                //GetReferenceData(itemToAdd,filterToAdd,currentFilter.FilterValue);
                            }
                        }
                   
                    
                    }
                    
                    foreach (ReportChart CurrentChart in loadedReport.ReportCharts)
                    {
                        ConsoleChart newChart = new ConsoleChart();
                        newChart.ChartStyle = CurrentChart.ChartStyle;
                        newChart.ChartType = CurrentChart.ChartType;
                        newChart.Description = CurrentChart.Description;
                        newChart.Title = CurrentChart.Title;

                        newChart.SeriesDataItems = new ObservableCollection<DataItem>();
                        newChart.CategoryDataItems = new ObservableCollection<DataItem>();
                        ChartList.Add(newChart);
                        newChart.CategoryDataItems.Add((from x in DataCategories 
                                              select x.DataItems).SelectMany(y => y).Where<DataItem>(x => x.Code == CurrentChart.ReportItem.DataItemCode).First<DataItem>());
                        List<Guid> seeriesDataItemCodes = new List<Guid>(from x in CurrentChart.ReportChartSeries
                                                                         select x.ReportItem.DataItemCode);
                        foreach (Guid dataitemCode in seeriesDataItemCodes)
                        {
                            newChart.SeriesDataItems.Add((from x in DataCategories 
                                              select x.DataItems).SelectMany(y => y).Where<DataItem>(x => x.Code == dataitemCode).First<DataItem>());
                        }
                    }
                    RefreshJoins("LoadingReport");
                    
                }
            }
        }
        void CallbackFromCategory(LoadOperation<DataCategory> lo)
        {
            DataCategories = new ObservableCollection<DataCategory>(lo.Entities);
            isCategoryComplete = true;
            CheckSelectable();
        }
        void CallbackFromTables(LoadOperation<DataTable> lo)
        {
            allTables = new ObservableCollection<DataTable>(lo.Entities);
            isTableComplete = true;
            CheckSelectable();
            
            
        }
        void CallbackFromModel(LoadOperation<DataModel> lo)
        {
            DataModels = new ObservableCollection<DataModel>(lo.Entities);
            
        }
        void CallbackFromRelationship(LoadOperation<DataTableRelationship> lo)
        {
            DataTableRelationships = new ObservableCollection<DataTableRelationship>(lo.Entities);
            isRelationshipComplete = true;
            CheckSelectable();
        }
        void QuickViewReport(object item)
        {
            selectedReportGroup = null;
            selectedReportName = WebContext.Current.User.Name + "Temp";
            RefreshJoins("CreateReport");
        }
        void SelectFolder(object item)
        {
            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>("SelectReportGroupDialog");
            this.modalDialogService = SimpleServiceLocator.Instance.Get<IModalDialogService>();
            this.modalDialogService.ShowDialog(dialog, new SelectReportGroupViewModel(!string.IsNullOrEmpty(EditingReportGroup) ? Guid.Parse(EditingReportGroup) : Guid.Empty, EditingReport != null ? EditingReport : null), 
            returnedViewModelInstance => 
            { 
                if (dialog.DialogResult.HasValue && dialog.DialogResult.Value) 
                {
                    selectedReportGroup = returnedViewModelInstance.SelectedReportGroup;
                    selectedReportName = returnedViewModelInstance.ReportName;
                    EditingReport = selectedReportName;
                    EditingReportGroup = selectedReportGroup.Code.ToString();
                    RefreshJoins("CreateReport");
            
                }
                
            }); 
        }
        void NewReport(object item)
        {
            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>("SelectReportModelDialog");
            this.modalDialogService = SimpleServiceLocator.Instance.Get<IModalDialogService>();
            this.modalDialogService.ShowDialog(dialog, new SelectReportModelDialogViewModel(dataModels),
                returnedViewModelInstance =>
                {
                    if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                    {
                        SelectedDataModel = returnedViewModelInstance.SelectedDataModel;
                        
                    }
                });
        }
        void AddChartToList(object item)
        {
            ConsoleChart chartToAdd = new ConsoleChart() { ChartType="Column" };
            
            chartList.Add(chartToAdd);
        }
        void SelectAdvancedJoins(object item)
        {
            RefreshJoins("AdvancedJoins");
            
        }
        void SelectAdvancedJoins()
        {
            var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>("AdvancedJoinDialog");
            this.modalDialogService = SimpleServiceLocator.Instance.Get<IModalDialogService>();
            this.modalDialogService.ShowDialog(dialog, new AdvancedJoinViewModel(JoinList),
            returnedViewModelInstance =>
            {
                if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                {
                    JoinList = returnedViewModelInstance.JoinList;
                }

            });
        }

        void EditSelectedDataItem(object item)
        {
            if (CurrentDataItemForCategory != null)
            {
                var dialog = SimpleServiceLocator.Instance.Get<IModalWindow>("EditDataItemDialog");

                this.modalDialogService = SimpleServiceLocator.Instance.Get<IModalDialogService>();

                //myContextForPopUp = new ReportWizardContext();
                //Display the EditItemDialog, passing in the guid of current DataItem that is selected
                this.modalDialogService.ShowDialog(dialog, new EditDataItemViewModel(CurrentDataItemForCategory.Code),//myContextForPopUp),
                returnedViewModelInstance =>
                {
                    if (dialog.DialogResult.HasValue && dialog.DialogResult.Value)
                    {
                        //Re-load the amended data item and merge it into the current context
                        myContext.Load<DataItem>(myContext.GetDataItemByCodeQuery(CurrentDataItemForCategory.Code),LoadBehavior.MergeIntoCurrent, LoadDataItemCallback, null);
                                                                                
                    }                 
                });    
            }
        }
        void PopulateAutoCompleteBox(object item)
        {
            if (!isLoadingReport)
            {
                var filterToPopulate = item as Models.Filter;
                if (filterToPopulate != null)
                {
                    if (filterToPopulate.DataItemToFilter.DataType == "bit")
                    {
                        filterToPopulate.ExistingValues = new ObservableCollection<string>(new List<string> { "Yes", "No" });
                    }
                    else
                    {
                        GetReferenceData(filterToPopulate.DataItemToFilter, filterToPopulate, filterToPopulate.Value, filterToPopulate.Value);
                    }
                }
            }
        }

        void LoadDataItemCallback(LoadOperation<DataItem> lo)
        {                                                       
            //Once the new item has been loaded, re-calculate its isSelectable property
            RecalculateSelectable();

        }

        bool CanEditDataItem(object item)
        {
            return (CurrentDataItemForCategory != null);
        }

        void CreateReport(object item)
        {

            IsGeneratingReport = true;
            if (loadedReport != null && selectedReportGroup !=null && loadedReport.GroupCode == selectedReportGroup.Code && loadedReport.Name==selectedReportName)
            {
                reportDefinition = loadedReport;
            }
            else
            {
                reportDefinition = new Report();
                reportDefinition.Code = Guid.NewGuid();
                reportDefinition.CreationDate = DateTime.Now;
                reportDefinition.CreationUserID = WebContext.Current.User.Name;
                reportDefinition.Description = "New Report";
                myContext.Reports.Add(reportDefinition);
                loadedReport = reportDefinition;
            }
            reportDefinition.DataModelCode = selectedDataModel.Code;
            reportDefinition.GroupCode = (selectedReportGroup != null)?selectedReportGroup.Code:Guid.Parse("b6c8dd8a-469c-4fa4-94ca-44913fead0b2");
            reportDefinition.IsAllowDrilldown = IsDrillDown;
            reportDefinition.IsDataMapDisplayed = IsDataMap;
            reportDefinition.IsInitialExpanded = false;
            reportDefinition.IsMatrix = (from x in SelectedDataItems 
                                        where x.IsColumnGrouping
                                        select x).Count<DataItem>() > 0;
            reportDefinition.IsOuterJoin = false;
            reportDefinition.IsPageOnFirstItem = IsPageOnFirstGroup;
            reportDefinition.IsStandard = false;
            reportDefinition.IsSummaryReport = IsSummary;
            reportDefinition.ModifiedDate = DateTime.Now;
            reportDefinition.ModifiedUserID = WebContext.Current.User.Name;
            reportDefinition.Name = selectedReportName;
            if (reportDefinition.RowIdentifier == null)
            {
                reportDefinition.RowIdentifier = new byte[] { 0 };
            }

            int l = reportDefinition.ReportCharts.Count;
            for (int p = l; l > 0; l--)
            {
                ReportChart currentItem = reportDefinition.ReportCharts.First();
                int q = currentItem.ReportChartSeries.Count;
                for (int r = q; r > 0; r--)
                {
                    myContext.ReportChartSeries.Remove(currentItem.ReportChartSeries.First());

                }

                myContext.ReportCharts.Remove(currentItem);
               
            }

            int j = reportDefinition.ReportItems.Count;
            for( int i=j;i>0;i--)
            {
                ReportItem currentItem = reportDefinition.ReportItems.First();
                int m = currentItem.Filters.Count;
                for (int n = m; n > 0; n--)
                {
                    myContext.Filters.Remove(currentItem.Filters.First());   
                }
                myContext.ReportItems.Remove(currentItem);
                
                //    reportDefinition.ReportItems.Remove(reportDefinition.ReportItems.First());
            }
            int reportOrder = 0;
            bool hasGroups = false;
            foreach (DataItem CurrentDataItem in SelectedDataItems)
            {
                ReportItem NewReportItem = new ReportItem();
                
                NewReportItem.Code = Guid.NewGuid();
                NewReportItem.DataItem = (from x in DataCategories select x.DataItems)
                    .SelectMany(y => y).Where<DataItem>(x => x.Code==CurrentDataItem.Code).First<DataItem>();
                NewReportItem.DataItemCode = CurrentDataItem.Code;
                NewReportItem.IsColumnTotal = CurrentDataItem.IsColumnGrouping;
                NewReportItem.IsField = true;
                NewReportItem.IsParameter = CurrentDataItem.IsParameter;
                NewReportItem.IsRowTotal = CurrentDataItem.IsRowGrouping;
                NewReportItem.Report = reportDefinition;
                NewReportItem.ReportCode = reportDefinition.Code;
                NewReportItem.RowIdentifier = new byte[] { 0 };
                NewReportItem.SortField = reportOrder;
                reportDefinition.ReportItems.Add(NewReportItem);
                reportOrder++;
                if (NewReportItem.IsRowTotal)
                {
                    hasGroups = true;
                }
                
            }
            if (!hasGroups)
            {
                reportDefinition.IsAllowDrilldown = false;
                IsDrillDown = false;
            }
            foreach (Models.Filter CurrentFilter in Filters)
            {
                var ReportItemFoundList = (from x in reportDefinition.ReportItems
                                       where x.DataItem == CurrentFilter.DataItemToFilter
                                       select x);
                if (ReportItemFoundList.Count<ReportItem>() != 0)
                {
                    var ReportItemFound = ReportItemFoundList.First<ReportItem>();
                    ReportItemFound.IsFilter = true;
                    Server.DataAccess.Filter newFilter = new Server.DataAccess.Filter();
                    newFilter.Code = Guid.NewGuid();
                    if (CurrentFilter.DataItemToFilter.DataType == "bit")
                    {
                        newFilter.FilterValue = CurrentFilter.Value == "Yes" ? "1" : "0";
                    }
                    else
                    {
                        newFilter.FilterValue = CurrentFilter.OperandChosen.Value != "ISNULL" ? CurrentFilter.Value : "NULL";
                    }
                    newFilter.Operand = CurrentFilter.OperandChosen.Value;
                    newFilter.ReportItem = ReportItemFound;
                    newFilter.ReportItemCode = ReportItemFound.Code;
                    newFilter.RowIdentifier= new Byte[] {0};
                    ReportItemFound.Filters.Add(newFilter);
                }
                else
                {
                    ReportItem NewFilterReportItem = new ReportItem();
                    NewFilterReportItem.Code = Guid.NewGuid();
                    NewFilterReportItem.DataItem = (from x in DataCategories select x.DataItems)
                    .SelectMany(y => y).Where<DataItem>(x => x.Code == CurrentFilter.DataItemToFilter.Code).First<DataItem>(); 
                    NewFilterReportItem.DataItemCode = CurrentFilter.DataItemToFilter.Code;
                    NewFilterReportItem.IsColumnTotal = false;
                    NewFilterReportItem.IsField = false;
                    NewFilterReportItem.IsParameter = false;
                    NewFilterReportItem.IsRowTotal = false;
                    NewFilterReportItem.IsFilter = true;
                    NewFilterReportItem.Report = reportDefinition;
                    NewFilterReportItem.ReportCode = reportDefinition.Code;
                    NewFilterReportItem.RowIdentifier = new Byte[] { 0 };
                    
                    reportDefinition.ReportItems.Add(NewFilterReportItem);
                    Server.DataAccess.Filter newFilter = new Server.DataAccess.Filter();
                    newFilter.Code = Guid.NewGuid();
                    if (CurrentFilter.DataItemToFilter.DataType == "bit")
                    {
                        newFilter.FilterValue = CurrentFilter.Value == "Yes" ? "1" : "0";
                    }
                    else
                    {
                        newFilter.FilterValue = CurrentFilter.OperandChosen.Value != "ISNULL" ? CurrentFilter.Value : "NULL";
                    }
                    newFilter.Operand = CurrentFilter.OperandChosen.Value;
                    newFilter.ReportItem = NewFilterReportItem;
                    newFilter.ReportItemCode = NewFilterReportItem.Code;
                    newFilter.RowIdentifier = new Byte[] { 0 };
                    NewFilterReportItem.Filters.Add(newFilter);
                }
            }
            int chartCount=0;
            foreach (ConsoleChart currentChart in ChartList)
            {
                ReportChart NewChartToAdd = new ReportChart() { Code=Guid.NewGuid()};
                NewChartToAdd.ChartStyle = currentChart.ChartStyle;
                NewChartToAdd.ChartType = currentChart.ChartType;
                NewChartToAdd.Description = currentChart.Description;
                NewChartToAdd.Title = currentChart.Title;

                foreach (DataItem currentDataItem in currentChart.CategoryDataItems.Union(currentChart.SeriesDataItems))
                {
                    var ReportItemFoundList = (from x in reportDefinition.ReportItems
                                               where x.DataItem == currentDataItem
                                               select x);
                    if (ReportItemFoundList.Count<ReportItem>() != 0)
                    {
                        var ReportItemFound = ReportItemFoundList.First<ReportItem>();
                        ReportItemFound.IsChartField = true;
                        
                    }
                    else
                    {
                        ReportItem NewChartReportItem = new ReportItem();
                        NewChartReportItem.Code = Guid.NewGuid();
                        NewChartReportItem.DataItem = (from x in DataCategories select x.DataItems)
                        .SelectMany(y => y).Where<DataItem>(x => x.Code == currentDataItem.Code).First<DataItem>();
                        NewChartReportItem.DataItemCode = currentDataItem.Code;
                        NewChartReportItem.IsColumnTotal = false;
                        NewChartReportItem.IsField = false;
                        NewChartReportItem.IsParameter = false;
                        NewChartReportItem.IsRowTotal = false;
                        NewChartReportItem.IsFilter = false;
                        NewChartReportItem.IsChartField = true;
                        NewChartReportItem.Report = reportDefinition;
                        NewChartReportItem.ReportCode = reportDefinition.Code;
                        NewChartReportItem.RowIdentifier = new Byte[] { 0 };

                        reportDefinition.ReportItems.Add(NewChartReportItem);

                    }
                }
                ReportItem currentCategoryReportItem = (from x in reportDefinition.ReportItems where x.DataItem == currentChart.CategoryDataItems[0] select x).First();
                NewChartToAdd.CategoryReportItemCode = currentCategoryReportItem.Code;
                NewChartToAdd.ReportItem = currentCategoryReportItem;
                int seriesCount=0;
                foreach(DataItem seriesItem in currentChart.SeriesDataItems)
                {
                    ReportItem currentSeriesReportItem = (from x in reportDefinition.ReportItems where x.DataItem == seriesItem select x).First();
                    ReportChartSery newSeries = new ReportChartSery();
                    newSeries.Code = Guid.NewGuid();
                    newSeries.ReportChart = NewChartToAdd;
                    newSeries.ReportChartCode = NewChartToAdd.Code;
                    newSeries.ReportItem = currentSeriesReportItem;
                    newSeries.SeriesReportItemCode = currentSeriesReportItem.Code;
                    newSeries.RowIdentifier = new Byte[] { 0 };
                    newSeries.SortOrder=seriesCount;
                    NewChartToAdd.ReportChartSeries.Add(newSeries);
                    seriesCount ++;
                }
                NewChartToAdd.SortOrder = chartCount;
                NewChartToAdd.Title = currentChart.Description;
                NewChartToAdd.RowIdentifier = new Byte[] { 0 };
                reportDefinition.ReportCharts.Add(NewChartToAdd);
                chartCount++;
            }
            int k = reportDefinition.ReportDataTableJoins.Count;
            for (int i = k; i > 0; i--)
            {
                myContext.ReportDataTableJoins.Remove(reportDefinition.ReportDataTableJoins.First());
                //reportDefinition.ReportDataTableJoins.Remove(reportDefinition.ReportDataTableJoins.First());
            }
            foreach (DataTableJoin currentJoin in JoinList)
            {
                ReportDataTableJoin NewReportJoin = new ReportDataTableJoin();
                NewReportJoin.Code = Guid.NewGuid();
                NewReportJoin.DataTableJoinCode = currentJoin.Code;
                NewReportJoin.ReportCode = reportDefinition.Code;
                NewReportJoin.Report = reportDefinition;
                NewReportJoin.JoinType = currentJoin.JoinType;
                NewReportJoin.RowIdentifier = new Byte[] { 0 };
            }
            //myContext.GenerateReport(reportDefinition, CallbackFromGenerateReport, null);
            //myContext.GenerateReport(reportDefinition);//, CallbackFromGenerateReport, null);
            //myContext.GenerateReport(reportDefinition);
            
            myContext.SubmitChanges(CallbackFromGenerateReport,null);
        }

        private void CallbackFromGenerateReport(SubmitOperation lo)
        {
            //Open a window with the report URL.
            if (lo.HasError == false)
            {
                bool ReportGenerated = true;
                IsGeneratingReport = false;
                //HtmlWindow window = HtmlPage.Window.Navigate(new Uri(@"ReportViewer2010.aspx?report=" + (selectedReportGroup != null ? selectedReportGroup.FullPath:"Temp/") + selectedReportName, UriKind.Relative),
                //    "_blank", "toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes");

                Uri reportUri = new Uri(App.Current.Host.Source, @"../ReportViewer2010.aspx");
                HtmlPage.Window.Eval("var win = window.open('" + reportUri.AbsoluteUri + "', '_blank', 'toolbar=no,location=no,status=no,menubar=no,scrollbars=yes,resizable=yes');");
            }
            else
            {
                List<Entity> ErrorEntities = lo.EntitiesInError.ToList<Entity>();
                Exception ErrorForOp = lo.Error;
                string errormessage = ErrorForOp.Message;
            }
            

        }

        void DragStartedFromControl(object item)
        {
            dragFrom = (string)item;
        }
        void UpdateSelected(object item)
        {
            //Retreive the event args
            EventInformation<EventArgs> PassedInfo = item as EventInformation<EventArgs>;
            //Microsoft.Windows.DragEventArgs args = (Microsoft.Windows.DragEventArgs)PassedInfo.EventArgs;
            
            //Extract the ItemDragEventArgs from the DragEventArgs 
            //ItemDragEventArgs data = (ItemDragEventArgs)(args.Data.GetData(args.Data.GetFormats()[0]));
            ItemDragEventArgs data = (ItemDragEventArgs)PassedInfo.EventArgs;
            /*if (data.DragSource is ListBox)
            {
                data.Effects = DragDropEffects.Move;
                args.Effects = DragDropEffects.Move;
                data.Handled = false;
                    args.Handled = false;
                    return;
                
            }*/

            //Extract the Selected data
            SelectionCollection data1 = data.Data as SelectionCollection;
            
            //If the dragged data is a dataitem add it to the selected data items collection
            if (data1.Count > 0 && data1[0].Item is DataItem)
            {
                DataItem ItemDragged = (DataItem)data1[0].Item;

                if (!SelectedDataItems.Contains(ItemDragged))
                {
                    SelectedDataItems.Add(ItemDragged);
                    CurrentSelectedDataItem = ItemDragged;
                    
                }
            }
        }
        void UpdateSelectedSource(object item)
        {
            //Retreive the event args
            EventInformation<EventArgs> PassedInfo = item as EventInformation<EventArgs>;
            Microsoft.Windows.DragEventArgs args = (Microsoft.Windows.DragEventArgs)PassedInfo.EventArgs;

            //Extract the ItemDragEventArgs from the DragEventArgs 
           // ItemDragEventArgs data = (ItemDragEventArgs)(args.Data.GetData(args.Data.GetFormats()[0]));

                args.Handled = false;
                return;

        }
        void RefreshJoins(string caller)
        {
            var allSelectedTables = GetSelectedTableCodes();

            myContext.Load<DataTableJoin>(myContext.RetreiveJoinListQuery(allSelectedTables), RetreiveJoinsCallBack, caller); ;
            
        }

        private IEnumerable<Guid> GetSelectedTableCodes()
        {
            var selectedTableCodes = from x in selectedDataItems select x.DataTableCode;
            var filterTableCodes = from y in filters select y.DataItemToFilter.DataTableCode;
            var allSelectedTables = selectedTableCodes.Union(filterTableCodes);
            return allSelectedTables;
        }

        void RetreiveJoinsCallBack(LoadOperation<DataTableJoin> lo)
        {
            string caller = (string)lo.UserState;
            //remove the joins not in the returned entity list.
            
            //var NotInList = JoinList.Except(lo.Entities, new DataTableJoinEqualityComparer());
            
            var NotInList = new List<DataTableJoin>(JoinList.Except(lo.Entities, new DataTableJoinEqualityComparer()));

            foreach (DataTableJoin currentItem in NotInList)
            {
                JoinList.Remove(currentItem);
            }
            

            //Add any joins in the entity list that are in the JoinList
            foreach (DataTableJoin currentJoin in lo.Entities)
            {

                var join = (from x in JoinList where x.Code == currentJoin.Code select x).FirstOrDefault();
                if (join == null)
                {
                    currentJoin.JoinType=currentJoin.DefaultJoinType;
                    currentJoin.AvailableJoinTypes = new ObservableCollection<JoinType>();
                    string fromTablePlural = currentJoin.FromTable.Substring(currentJoin.FromTable.Length - 1).ToUpper() == "Y" ? currentJoin.FromTable.Substring(0, currentJoin.FromTable.Length - 1) + "ies" : currentJoin.FromTable + "s";
                    string toTablePlural = currentJoin.ToTable.Substring(currentJoin.ToTable.Length - 1).ToUpper() == "Y" ? currentJoin.ToTable.Substring(0, currentJoin.ToTable.Length - 1) + "ies" : currentJoin.ToTable + "s";
 
                    currentJoin.AvailableJoinTypes.Add(new JoinType()
                    {
                        ShortText = "INNER",
                        LongText = "INNER: Select only " + fromTablePlural + " that have related " + toTablePlural + " and " +
                    "select only " + toTablePlural + " that have related " + fromTablePlural
                    });
                    currentJoin.AvailableJoinTypes.Add(new JoinType()
                    {
                        ShortText = "LEFT",
                        LongText = "LEFT: Select all " + fromTablePlural + " and only related " +
                    toTablePlural
                    });

                    currentJoin.AvailableJoinTypes.Add(new JoinType()
                    {
                        ShortText = "RIGHT",
                        LongText = "RIGHT: Select all " + toTablePlural + " and only related " +
                            fromTablePlural
                    });
                    currentJoin.AvailableJoinTypes.Add(new JoinType()
                    {
                        ShortText = "FULL",
                        LongText = "FULL: Select all " + fromTablePlural + " and " +
                    toTablePlural + " with or without relations"
                    });

                    JoinList.Add(currentJoin);
                }
            }
            if (caller == "CreateReport")
            {
                CreateReport(null);
            }
            else if (caller == "AdvancedJoins")
            {
                SelectAdvancedJoins();
            }
            else if (caller=="LoadingReport")
            {
                foreach (ReportDataTableJoin currentJoin in loadedReport.ReportDataTableJoins)
                    {
                        DataTableJoin joinToAdd = (from x in JoinList where x.Code == currentJoin.DataTableJoinCode select x).FirstOrDefault();
                        joinToAdd.JoinType = currentJoin.JoinType;
                        
                    }
                    isLoadingReport = false;
                    isPopulatingReport = false;
            }
        }
        void UpdateChartCategory(object item)
        {   
            //Retreive the event args
            EventInformation<EventArgs> PassedInfo = item as EventInformation<EventArgs>;
            Microsoft.Windows.DragEventArgs args = (Microsoft.Windows.DragEventArgs)PassedInfo.EventArgs;
            if ((args.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                //Extract the ItemDragEventArgs from the DragEventArgs 
                ItemDragEventArgs data = (ItemDragEventArgs)(args.Data.GetData(args.Data.GetFormats()[0]));
                data.Handled = true;
                //Extract the Selected data
                SelectionCollection data1 = data.Data as SelectionCollection;

                //If the dragged data is a dataitem 
                if (data1.Count > 0 && data1[0].Item is DataItem)
                {
                    DataItem ItemDragged = (DataItem)data1[0].Item;

                    //Check if a report item exists with 

                    SelectedReportChart.CategoryDataItems.Clear();
                    SelectedReportChart.CategoryDataItems.Add(ItemDragged); 

                }
            }
            args.Effects = DragDropEffects.Copy;
            args.Handled = true;
        }
        void UpdateChartSeries(object item)
        {
            //Retreive the event args
            EventInformation<EventArgs> PassedInfo = item as EventInformation<EventArgs>;
            Microsoft.Windows.DragEventArgs args = (Microsoft.Windows.DragEventArgs)PassedInfo.EventArgs;
            if ((args.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                //Extract the ItemDragEventArgs from the DragEventArgs 
                ItemDragEventArgs data = (ItemDragEventArgs)(args.Data.GetData(args.Data.GetFormats()[0]));
                data.Handled = true;
                //Extract the Selected data
                SelectionCollection data1 = data.Data as SelectionCollection;

                //If the dragged data is a dataitem 
                if (data1.Count > 0 && data1[0].Item is DataItem)
                {
                    DataItem ItemDragged = (DataItem)data1[0].Item;

                    //Check if a report item exists with 
                    if (!SelectedReportChart.SeriesDataItems.Contains(ItemDragged))
                    {
                        SelectedReportChart.SeriesDataItems.Add(ItemDragged);
                    }
                }
            }
            args.Effects = DragDropEffects.Copy;
            args.Handled = true;
        }

        void UpdateFilter(object item)
        {
            
            //Retreive the event args
            EventInformation<EventArgs> PassedInfo = item as EventInformation<EventArgs>;
            Microsoft.Windows.DragEventArgs args = (Microsoft.Windows.DragEventArgs)PassedInfo.EventArgs;
            if ((args.Effects & DragDropEffects.Copy) == DragDropEffects.Copy)
            {
                //Extract the ItemDragEventArgs from the DragEventArgs 
                ItemDragEventArgs data = (ItemDragEventArgs)(args.Data.GetData(args.Data.GetFormats()[0]));
                data.Handled = true;
                //Extract the Selected data
                SelectionCollection data1 = data.Data as SelectionCollection;
                
                //If the dragged data is a dataitem add it to the selected data items collection
                if (data1.Count > 0 && data1[0].Item is DataItem)
                {
                    DataItem ItemDragged = (DataItem)data1[0].Item;

                    Models.Filter newFilter = new UcbManagementInformation.Models.Filter()
                    {
                        DataItemToFilter = ItemDragged,
                        PossibleOperands = GetOperandsByDataType(ItemDragged.DataType, ItemDragged.IsValueType),
                        ExistingValues = ItemDragged.IsValueType ? new ObservableCollection<string>() : null,
                        DropDownVisibility = ((!ItemDragged.IsValueType || (ItemDragged.DataType == "bit")) ? true : false),
                        TextBoxVisibility = ((ItemDragged.IsValueType && ItemDragged.DataType != "bit" && ItemDragged.DataType.ToUpper() != "DATETIME") ? true : false),
                        CalendarVisibility = ((ItemDragged.IsValueType && ItemDragged.DataType.ToUpper() == "DATETIME") ? true : false),
                                 OperandChosen = new List<Operand>(((from x in allOperands
                                                            where x.Name == "Equal To"
                                                            select x)))[0]


                    };
                    if (!ItemDragged.IsValueType && !(ItemDragged.DataType == "bit"))
                    {
                        GetReferenceData(ItemDragged, newFilter,"");
                    }
                    else if (ItemDragged.DataType == "bit")
                    {
                        newFilter.ExistingValues = new ObservableCollection<string>(new List<string> { "Yes", "No" });
                    }
                    Filters.Add(newFilter);
                }
            }
            args.Effects = DragDropEffects.Copy;
            args.Handled = true;
        }
        struct FilterData
        {
            public Models.Filter FilterItem;
            public string ExistingValue;
        }
        void GetReferenceData(DataItem itemDragged, Models.Filter newFilter,string existingValue,string startsWith)
        {
            ObservableCollection<string> filterValues = new ObservableCollection<string>();
            ReportWizardContext myContext = new ReportWizardContext();
            FilterData filterState = new FilterData();
            filterState.ExistingValue = existingValue;
            filterState.FilterItem = newFilter;
            myContext.GetReferenceData(itemDragged.DataTableCode, itemDragged.Name,startsWith, CallbackFromGetReferenceData, filterState);
        }
        void GetReferenceData(DataItem itemDragged, Models.Filter newFilter, string startsWith)
        {
            GetReferenceData(itemDragged, newFilter,startsWith, null); 
        }
        
        private void CallbackFromGetReferenceData(InvokeOperation<IEnumerable<string>> lo)
        {
            FilterData filterState = (FilterData)lo.UserState;
            ((Models.Filter)filterState.FilterItem).ExistingValues = new ObservableCollection<string>(lo.Value);
            //((Models.Filter)filterState.FilterItem).Value = filterState.ExistingValue;

        }

        ObservableCollection<Operand> GetOperandsByDataType(string dataType, bool isValueType)
        {
            ObservableCollection<Operand> possibleOperands = new ObservableCollection<Operand>();
            if (isValueType)
            {
                if (dataType == "varchar" || dataType == "char")
                {
                    if (dataType == "char")
                    {
                        possibleOperands = new ObservableCollection<Operand>((from x in allOperands
                                                                              where x.Name == "Like" || x.Name == "Equal To" || x.Name == "Not Equal To"
                                                                              select x));
                    }
                    else
                    {
                        possibleOperands = allOperands;
                    }
                }
                else
                {
                    possibleOperands = new ObservableCollection<Operand>((from x in allOperands
                                                                          where x.Name != "Like"
                                                                          select x));
                }
            }



            else //reference value
            {

                possibleOperands = new ObservableCollection<Operand>((from x in allOperands
                                                                      where x.Name != "Like"
                                                                      select x));

            }


            return possibleOperands;
            
        }

        void RemoveDataItemFromSelected(object item)
        {
            SelectedDataItems.Remove((DataItem)item);
        }
        void RemoveFilterFromList(object item)
        {
            Filters.Remove((UcbManagementInformation.Models.Filter)item);
        }
        public DataItem CurrentDataItem
        {
            get;
            set;
        }
        public ObservableCollection<DataItem> SelectedDataItems
        {
            get
            {
                return selectedDataItems;
            }
            set
            {
                selectedDataItems = value; OnPropertyChanged("SelectedDataItems");
            }
        }
        public ObservableCollection<DataTableJoin> JoinList
        {
            get
            {
                return joinList;
            }
            set
            {
                joinList = value; OnPropertyChanged("JoinList");
            }
        }
        public ObservableCollection<Guid> AvailableTables
        {
            get {return availableTables;}
            set { availableTables = value; OnPropertyChanged("AvailableTables"); }
        }
        private DataItem currentDataItemForCategory;
        
        public DataItem CurrentDataItemForCategory
        {
            get { return currentDataItemForCategory; }
            set { currentDataItemForCategory = value; OnPropertyChanged("CurrentDataItemForCategory"); }
        }
        public DataItem CurrentSelectedDataItem
        {
            get { return currentSelectedDataItem; }
            set { currentSelectedDataItem = value; OnPropertyChanged("CurrentSelectedDataItem"); }
        }
        public System.Collections.ObjectModel.ObservableCollection<DataCategory> DataCategories
        {
            get
            {
               return dataCategories;
            }

            set { dataCategories = value; OnPropertyChanged("DataCategories"); }
            
        }
        public System.Collections.ObjectModel.ObservableCollection<DataTableRelationship> DataTableRelationships
        {
            get
            {
                return dataTableRelatiopnships;
            }

            set { dataTableRelatiopnships = value; OnPropertyChanged("DataTableRelationships"); }

        }
        private RelayCommand dataItemDroppedOnSelected;
        public RelayCommand DataItemDroppedOnSelected
        {
            get{ return dataItemDroppedOnSelected;}
            set { dataItemDroppedOnSelected = value; OnPropertyChanged("DataItemDroppedOnSelected"); }
        }
        private RelayCommand dataItemDroppedOnSelectedSource;
        public RelayCommand DataItemDroppedOnSelectedSource
        {
            get { return dataItemDroppedOnSelectedSource; }
            set { dataItemDroppedOnSelectedSource = value; OnPropertyChanged("DataItemDroppedOnSelectedSource"); }
        }
        private RelayCommand addChart;
        public RelayCommand AddChart
        {
            get { return addChart; }
            set { addChart = value; OnPropertyChanged("AddChart"); }
        }
        private RelayCommand dataItemDroppedOnFilter;
        public RelayCommand DataItemDroppedOnFilter
        {
            get{ return dataItemDroppedOnFilter;}
            set{dataItemDroppedOnFilter=value;OnPropertyChanged("DataItemDroppedOnFilter");}
        }
        public RelayCommand DragStartedFrom
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
        public RelayCommand PopulatingCommand
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
        public RelayCommand AdvancedJoin
        {
            get;
            set;
        }
        private RelayCommand editDataItem;
        public RelayCommand EditDataItem
        {
            get { return editDataItem; }
            set { editDataItem = value; OnPropertyChanged("EditDataItem"); }
        }

        public RelayCommand NewReportCommand
        {
            get;
            set;
        }
        private RelayCommand itemDroppedOnChartSeriesCommand;
        public RelayCommand ItemDroppedOnChartSeriesCommand
        {
            get { return itemDroppedOnChartSeriesCommand; }
            set { itemDroppedOnChartSeriesCommand = value; OnPropertyChanged("ItemDroppedOnChartSeriesCommand"); }
        }
        private RelayCommand itemDroppedOnChartCategoryCommand;
        public RelayCommand ItemDroppedOnChartCategoryCommand
        {
            get { return itemDroppedOnChartCategoryCommand; }
            set { itemDroppedOnChartCategoryCommand = value; OnPropertyChanged("ItemDroppedOnChartCategoryCommand"); }
        }
        public ObservableCollection<UcbManagementInformation.Models.Filter> Filters
        {
            get { return filters; }
            set { filters = value; OnPropertyChanged("Filters"); }
        }
        
        public ObservableCollection<Operand> AllOperands { get { return allOperands; } set { allOperands = value; } }
        public ObservableCollection<DataModel> DataModels { get { return dataModels; } set { dataModels = value; OnPropertyChanged("DataModels"); } }
        public DataModel SelectedDataModel { get { return selectedDataModel; } set { selectedDataModel = value; OnPropertyChanged("SelectedDataModel"); } }
        private bool isPageOnFirstGroup;
        private bool isDrillDown;
        private bool isSummary;
        private bool isDataMap;
        private bool isGeneratingReport;
        private bool isLoadingModel;
        public bool IsPageOnFirstGroup 
        { get{return isPageOnFirstGroup;}
            set { isPageOnFirstGroup = value; OnPropertyChanged("IsPageOnFirstGroup"); }
        }
        public bool IsDrillDown
        {
            get { return isDrillDown; }
            set { isDrillDown = value; OnPropertyChanged("IsDrillDown"); }
        }
        public bool IsSummary
        {
            get { return isSummary; }
            set { isSummary = value; OnPropertyChanged("IsSummary"); }
        }
        public bool IsDataMap
        {
            get { return isDataMap; }
            set { isDataMap = value; OnPropertyChanged("IsDataMap"); }
        }
        public bool IsGeneratingReport
        {
            get { return isGeneratingReport; }
            set { isGeneratingReport = value; OnPropertyChanged("IsGeneratingReport"); }
        }

        public bool IsLoadingModel
        {
            get { return isLoadingModel; }
            set { isLoadingModel = value; OnPropertyChanged("IsLoadingModel"); }
        }
        public ObservableCollection<ConsoleChart> ChartList
        {
            get { return chartList; }
            set { chartList = value; OnPropertyChanged("ChartList"); }
        }
        public ObservableCollection<string> AvailableCharts
        {
            get { return availableCharts; }
            set { availableCharts = value; OnPropertyChanged("AvailableCharts"); }
        }
        public ConsoleChart SelectedReportChart
        {
            get { return selectedReportChart; }
            set { selectedReportChart = value; OnPropertyChanged("SelectedReportChart"); }
        }
       
    }
}
