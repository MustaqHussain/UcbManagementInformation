
namespace UcbManagementInformation.Web.Server
{
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.ComponentModel.DataAnnotations;
    using System.Data;
    using System.Linq;
    using System.ServiceModel.DomainServices.Hosting;
    using System.ServiceModel.DomainServices.Server;
    using UcbManagementInformation.Server.DataAccess;
    using UcbManagementInformation.Server.DataAccess.BusinessObjects;
    using UcbManagementInformation.Server.RDL2003Engine;
    using UcbManagementInformation.Web.Reporting;
    using System.Data.Objects;
    using System.Collections;
    using UcbManagementInformation.Web.Models;
    using UcbManagementInformation.Server.IoC.ServiceLocation;
    using System.Text;
    using System.Web;


    // Implements application logic using the UcbManagementInformationEntities context.
    // TODO: Add your application logic to these methods or in additional methods.
    // TODO: Wire up authentication (Windows/ASP.NET Forms) and uncomment the following to disable anonymous access
    // Also consider adding roles to restrict access as appropriate.
    // [RequiresAuthentication]
    [EnableClientAccess()]
    [RequiresAuthentication]
    public class ReportWizardService : DomainService
    {
        //External services
        private IReportingServices ReportService;

        //Unit of work
        private IUnitOfWork uow;
        private IUnitOfWork UcbUow;

        //Repositories
        private IUcbManagementInformationRepository<DataCategory> dataCategoryRepository;
        private IDataTableRelationshipRepository dataTableRelationshipRepository;
        private IDataTableRepository dataTableRepository;
        private IUcbManagementInformationRepository<Report> reportRepository;
        private IUcbManagementInformationRepository<MCUser> mcUserRepository;
        private IReportGroupRepository reportGroupRepository;
        private IReportGroupRepository reportGroupRepositoryRead;
        private IDataItemRepository dataItemRepository;
        private IUcbManagementInformationRepository<UserReportGroup> userReportGroupRepository;
        private IDataModelRepository dataModelRepository;
        private IDataTableJoinRepository dataTableJoinRepository;
        private IDataTableRelationshipJoinRepository dataTableRelationshipJoinRepository;
        private IUcbManagementInformationRepository<Filter> filterRepository;
        private IUcbManagementInformationRepository<ReportChart> reportChartRepository;
        private IUcbManagementInformationRepository<ReportChartSery> reportChartSeriesRepository;
        private IUcbManagementInformationRepository<ReportDataTableJoin> reportDataTableJoinRepository;
        private IUcbManagementInformationRepository<ReportItem> reportItemRepository;
        private IUcbRepository<ReportCategory> reportCategoryRepository;
        private IUcbRepository<StandardReport> standardReportRepository;
        private ReportGroup reportGroupUpdate = new ReportGroup();
        private ReportGroup originalGroup = new ReportGroup();
        private Report reportForDeleting = new Report();
        private string reportingServicesAction = "";

        private StandardReport standardReportUpdate;
        //GET REFERENCE DATA NEEDS SORTING!!!!!

        public ReportWizardService(IReportingServices reportService,
            IUnitOfWork uowItem,
            IUnitOfWork UcbUowItem,
            IDataTableRelationshipRepository dataTableRelationshipRepositoryItem,
            IUcbManagementInformationRepository<DataCategory> dataCategoryRepositoryItem,
            IDataTableRepository dataTableRepositoryItem,
            IUcbManagementInformationRepository<Report> reportRepositoryItem,
            IUcbManagementInformationRepository<MCUser> mcUserRepositoryItem,
            IReportGroupRepository reportGroupRepositoryItem,
            IReportGroupRepository reportGroupRepositoryReadItem,
            IDataItemRepository dataItemRepositoryItem,
            IUcbManagementInformationRepository<UserReportGroup> userReportGroupRepositoryItem,
            IDataModelRepository dataModelRepositoryItem,
            IDataTableJoinRepository dataTableJoinRepositoryItem,
            IDataTableRelationshipJoinRepository dataTableRelationshipJoinRepositoryItem,
            IUcbManagementInformationRepository<Filter> filterRepositoryItem,
            IUcbManagementInformationRepository<ReportChart> reportChartRepositoryItem,
            IUcbManagementInformationRepository<ReportChartSery> reportChartSeriesRepositoryItem,
            IUcbManagementInformationRepository<ReportDataTableJoin> reportDataTableJoinRepositoryItem,
            IUcbManagementInformationRepository<ReportItem> reportItemRepositoryItem,
            IUcbRepository<ReportCategory> reportCategoryRepositoryItem,
            IUcbRepository<StandardReport> standardReportRepositoryItem

            )
        {
            ReportService = reportService;
            uow = uowItem;
            dataCategoryRepository = dataCategoryRepositoryItem;
            dataTableRelationshipRepository = dataTableRelationshipRepositoryItem;
            dataTableRepository = dataTableRepositoryItem;
            reportRepository = reportRepositoryItem;
            mcUserRepository = mcUserRepositoryItem;
            reportGroupRepository = reportGroupRepositoryItem;
            reportGroupRepositoryRead = reportGroupRepositoryReadItem;
            dataItemRepository = dataItemRepositoryItem;
            userReportGroupRepository = userReportGroupRepositoryItem;
            dataModelRepository = dataModelRepositoryItem;
            dataTableJoinRepository = dataTableJoinRepositoryItem;
            dataTableRelationshipJoinRepository = dataTableRelationshipJoinRepositoryItem;
            filterRepository = filterRepositoryItem;
            reportChartRepository = reportChartRepositoryItem;
            reportChartSeriesRepository = reportChartSeriesRepositoryItem;
            reportDataTableJoinRepository = reportDataTableJoinRepositoryItem;
            reportItemRepository = reportItemRepositoryItem;
            reportCategoryRepository = reportCategoryRepositoryItem;
            standardReportRepository = standardReportRepositoryItem;
            UcbUow = UcbUowItem;
        }

        public IQueryable<DataCategory> GetDataCategoriesWithDataItemsByDataModel(Guid dataModelCode)
        {
            //InsertRelationships();
            IEnumerable<DataCategory> dcList = dataCategoryRepository.Find(x => x.DataModelCode == dataModelCode).OrderBy(x => x.SortField);//.AsQueryable<DataCategory>();

            //To provide the child items sorted we must not do eager loading, and manually retreive the child items.
            foreach (DataCategory category in dcList)
            {
                var itemsQuery = category.DataItems.CreateSourceQuery().OrderByDescending(
                                                 o => o.IsCommonTableGrouping
                                             ).ThenByDescending(x => x.IsGroup).ThenBy(x => x.Name);
                                             //.ThenBy(x => x.IsSummable);
                category.DataItems.Attach(itemsQuery);

            }

            return dcList.AsQueryable();

        }

        public IQueryable<DataTableRelationship> GetDataTableRelationshipsByDataModel(Guid dataModelCode)
        {
            IEnumerable<DataTableRelationship> dcList = dataTableRelationshipRepository.Find(x => x.DataTable.DataModelCode == dataModelCode);//.AsQueryable<DataCategory>();

            return dcList.AsQueryable();

        }

        public IQueryable<UcbManagementInformation.Server.DataAccess.DataTable> GetDataTablesByDataModel(Guid dataModelCode)
        {
            IEnumerable<UcbManagementInformation.Server.DataAccess.DataTable> dtList = dataTableRepository.Find(x => x.DataModelCode == dataModelCode);//.AsQueryable<DataCategory>();

            return dtList.AsQueryable();

        }

        public IQueryable<Report> GetAllReportsForReportGroup(Guid reportGroupCode)
        {
            return reportRepository.Find(x => x.ReportGroup.Code == reportGroupCode).OrderBy(report=>report.Name).AsQueryable();
        }
                
        /// <summary>
        /// GetAllReportGroupsForUser obtains list of report groups for a user.
        /// </summary>
        /// <param name="userID">User ID passed as parameter</param>
        /// <returns>List of all report groups for user</returns>
        public IQueryable<ReportGroup> GetAllReportGroupsForUser()
        {
            string UserID = this.ServiceContext.User.Identity.Name;
            UserID = UserID.Substring(UserID.LastIndexOf('\\') + 1);
            
            //Declare arraylists to get data from data access level, and to pass on to UI 
            //Get UserID
            MCUser CurrentUser = new MCUser();
            
            CurrentUser = mcUserRepository.Find(x => x.Name == UserID).First();
            var groups =  reportGroupRepository.GetAllReportGroupsForUser(CurrentUser).AsQueryable();
            return groups;
        }
        public ReportGroup GetReportGroupByCode(Guid code)
        {
            return reportGroupRepository.Single(x => x.Code == code, "UserReportGroups");
        }
        public IQueryable<MCUser> GetAllMCUser()
        {
            return mcUserRepository.GetAll("UserReportGroups").AsQueryable();
        }
        public Report GetReportByCode(Guid reportCode)
        {
            return reportRepository.Find(x => x.Code == reportCode, new string[] { "ReportItems", "ReportItems.Filters", "ReportDataTableJoins", "ReportCharts", "ReportCharts.ReportChartSeries" }).FirstOrDefault();
        }

        public DataItem RetrieveDataItem(Guid dataitemcode)
        {
            return dataItemRepository.Find(x => x.Code == dataitemcode).FirstOrDefault();
        }

        public DataItem GetDataItemByCode(Guid dataitemCode)
        {
            return dataItemRepository.GetByCode(dataitemCode);
        }

        private ReportGroupAccessLevelType GetAccessLevel(
            string userID, string reportGroupPathName)
        {
            ReportGroupAccessLevelType MyAccessLevel = ReportGroupAccessLevelType.None;

            // To obtain the UserReportGroup, we need the UserCode and ReportGroupCode
            ReportGroup CurrentReportGroup = reportGroupRepository.Find(x => x.PathName == reportGroupPathName).First();
            MCUser ThisUser = mcUserRepository.Find(x => x.Name == userID).First();

            UserReportGroup CurrentUserReportGroup =
                userReportGroupRepository.Find(x => x.ReportGroupCode == CurrentReportGroup.Code && x.UserCode == ThisUser.Code).FirstOrDefault();

            /* In getting the access level, we try firstly to obtain it from the
            * specified report group. If it has none, we must then check the parent
            * of this report group for its access level. If the parent has an access
            * level, we return that, otherwise we check this folder's parent. We
            * continue like this, until a level is found or the top of the tree
            * is reached without finding one.
            */

            // This denotes whether the top of the tree has been reached in event of searching.
            bool IsTreeTop = false;

            // This denotes if a UserReportGroup for the ReportGroup has been found.
            bool IsFound = false;

            while (!IsTreeTop && !IsFound)
            {
                // If a UserReportGroup is found, we can stop searching.
                if (CurrentUserReportGroup != null)
                {
                    MyAccessLevel = (ReportGroupAccessLevelType)CurrentUserReportGroup.AccessLevel;
                    IsFound = true;
                }
                else
                {
                    // We must look to its parent for an access level.
                    if (CurrentReportGroup.ParentPath != null)
                    {
                        string ParentPath = CurrentReportGroup.ParentPath;
                        CurrentReportGroup = reportGroupRepository.Find(x => x.PathName == ParentPath).First();
                        CurrentUserReportGroup =
                            userReportGroupRepository.Find(x => x.ReportGroupCode == CurrentReportGroup.Code && x.UserCode == ThisUser.Code).FirstOrDefault();
                    }
                    else // The child is a root folder, so has no parent.
                    {
                        // Hence, we must stop searching.
                        IsTreeTop = true;
                    }
                }
            }

            return MyAccessLevel;
        }

       
        /*
        public ReportDefinitionBusinessObject ExposeReportDefinitionBusinessObject()
        {
            throw new NotSupportedException();
        } 
        */
        /// <summary>
        /// Call RDL Generator to create RDL formatted report
        /// </summary>
        /// <param name="reportDefinitionBO">report definition required to generate report 
        /// related table objects</param>
        /// <param name="isReportExisting">Indicate if a new or existing report to be generated</param>
        /// <returns>true: successful; false: failed</returns>
        public void GenerateReport(Report
            reportDefinitionSO)
        {
            /*ExceptionLoggingtry
            {*/
            //get list of row totals, column totals, parameters and filters

            //translate from reporDefinitionSO to BO
            ReportDefinitionBusinessObject reportDefinitionBO = new ReportDefinitionBusinessObject();
            foreach (ReportItem currentReportItem in reportDefinitionSO.ReportItems)
            {
                if (currentReportItem.DataItem == null)
                {
                    currentReportItem.DataItem = dataItemRepository.GetByCode(currentReportItem.DataItemCode);
                }

            }
            DataModel currentModel = dataModelRepository.Find(x => x.Code == reportDefinitionSO.DataModelCode).FirstOrDefault();
            List<ReportItem> ReportItems = new List<ReportItem>(reportDefinitionSO.ReportItems);
            ReportItems.ForEach(x => x.DataItem.IsField = x.IsField);

            reportDefinitionBO.SelectedDataItems = new List<DataItem>(from x in ReportItems
                                                                      select x.DataItem);
            reportDefinitionBO.FieldDataItems = new List<DataItem>(from x in ReportItems
                                                                   where x.IsField
                                                                   select x.DataItem);
            reportDefinitionBO.RowTotalDataItems = new List<DataItem>(from x in ReportItems
                                                                      where x.IsRowTotal
                                                                      select x.DataItem);
            reportDefinitionBO.ColumnTotalDataItems = new List<DataItem>(from x in ReportItems
                                                                         where x.IsColumnTotal
                                                                         select x.DataItem);
            reportDefinitionBO.ParameterDataItems = new List<DataItem>(from x in ReportItems
                                                                       where x.IsParameter
                                                                       select x.DataItem);
            var FilterList = (from x in ReportItems
                              where x.IsFilter
                              select x.Filters).SelectMany(y => y);
            List<ReportFilterBusinessObject> filterListBO = new List<ReportFilterBusinessObject>();
            foreach (Filter currentFilter in FilterList)
            {
                ReportFilterBusinessObject newFilter = new ReportFilterBusinessObject();
                newFilter.Code = currentFilter.Code.ToString();
                newFilter.DataItemCaption = currentFilter.ReportItem.DataItem.Caption;
                newFilter.DataItemCode = currentFilter.ReportItem.DataItemCode.ToString();
                newFilter.IsValueType = currentFilter.ReportItem.DataItem.IsValueType;
                newFilter.FilterDefinition = currentFilter.Code.ToString();
                newFilter.FilterValue = currentFilter.FilterValue;
                newFilter.Operand = currentFilter.Operand;
                newFilter.ReportItemCode = currentFilter.ReportItemCode.ToString();
                newFilter.SortField = currentFilter.SortField;
                filterListBO.Add(newFilter);
            }

            reportDefinitionBO.FilterList = filterListBO;

            reportDefinitionBO.JoinList = reportDefinitionSO.ReportDataTableJoins.ToList();
            reportDefinitionBO.IsColumnTotal = reportDefinitionSO.IsMatrix;
            reportDefinitionBO.IsDataMapDisplayed = (bool)reportDefinitionSO.IsDataMapDisplayed;
            reportDefinitionBO.IsDrillDown = reportDefinitionSO.IsAllowDrilldown;
            reportDefinitionBO.IsOuterJoin = (bool)reportDefinitionSO.IsOuterJoin;
            reportDefinitionBO.IsPageBreak = reportDefinitionSO.IsPageOnFirstItem;
            reportDefinitionBO.IsSummaryReport = (bool)reportDefinitionSO.IsSummaryReport;
            reportDefinitionBO.ReportCode = reportDefinitionSO.Code.ToString();
            reportDefinitionBO.ReportDescription = reportDefinitionSO.Description;
            reportDefinitionBO.ReportName = reportDefinitionSO.Name;
            reportDefinitionBO.SelectedReportGroupCode = reportDefinitionSO.GroupCode.ToString();
            if (reportDefinitionSO.ReportGroup == null)
            {
                reportDefinitionBO.ReportPath = (reportGroupRepository.FirstOrDefault(x => x.Code == reportDefinitionSO.GroupCode)).PathName;
            }
            else
            {
                reportDefinitionBO.ReportPath = reportDefinitionSO.ReportGroup.PathName;
            }
            reportDefinitionBO.ChartList = new List<ReportChartBusinessObject>();
            foreach (ReportChart currentChart in reportDefinitionSO.ReportCharts)
            {
                ReportChartBusinessObject newChart = new ReportChartBusinessObject();
                newChart.CategoryReportItemCode = currentChart.CategoryReportItemCode.ToString();
                newChart.CategoryDataItemCode = currentChart.ReportItem.DataItemCode.ToString();
                newChart.Style = currentChart.ChartStyle;
                newChart.Type = currentChart.ChartType;
                newChart.Code = currentChart.Code.ToString();
                newChart.Description = currentChart.Description;
                newChart.ChartSeriesList = new List<ReportChartSeriesBusinessObject>();//
                newChart.SortOrder = currentChart.SortOrder;

                foreach (ReportChartSery currentSeries in currentChart.ReportChartSeries)
                {
                    ReportChartSeriesBusinessObject newSeries = new ReportChartSeriesBusinessObject();
                    newSeries.Code = currentSeries.Code.ToString();
                    newSeries.SeriesReportItemCode = currentSeries.SeriesReportItemCode.ToString();
                    newSeries.SortOrder = currentSeries.SortOrder;
                    newSeries.SeriesDataItemCode = currentSeries.ReportItem.DataItemCode.ToString();
                    newChart.ChartSeriesList.Add(newSeries);
                }

                reportDefinitionBO.ChartList.Add(newChart);
            }
            Byte[] RdlByteArray;
            if (reportDefinitionBO.IsColumnTotal == true)
            {
                //create matrix report
                MatrixReport NewMatrixReport = new MatrixReport(reportDefinitionBO, currentModel.DatasourceName);
                //Generate tabular report
                RdlByteArray = NewMatrixReport.Render();
            }
            else
            {
                //Create tabular report

                TabularReport NewTabularReport = new TabularReport(reportDefinitionBO, currentModel.DatasourceName);
                //Generate tabular report
                // RdlByteArray = NewTabularReport.Render();
                RdlByteArray = NewTabularReport.Render2010();

            }

            ReportService.PublishReport(RdlByteArray, reportDefinitionBO.ReportName, reportDefinitionBO.ReportPath.Replace(@"\", @"/"), currentModel.DatasourceName);
            
            string reportPath = (reportDefinitionBO.ReportPath != null ? reportDefinitionBO.ReportPath : "Temp/") + reportDefinitionBO.ReportName;
            PutReportInSession(reportPath);
            /*ExceptionLogging}
            catch (Exception e)
            {
                ExceptionLogging System.IO.File.WriteAllText("C:\\Releases\\Ucb MI\\MI_Error_Log.txt", String.Format("[{0}] Exception: {1}\nInner Exception: {2}", DateTime.Now, e.ToString(), e.InnerException == null ? "N/A" : e.InnerException.ToString()));
            }*/
        }

        public void PutReportInSession(string fullReportPath)
        {
            HttpContext.Current.Session["report"] = fullReportPath;
        }

        public static string GetUserGuid()
        {
            IUnitOfWork UcbUow = SimpleServiceLocator.Instance.Get<IUnitOfWork>("Ucb");
            IRepository<Staff> staffRepository = DataAccessUtilities.RepositoryLocator<IUcbRepository<Staff>>(UcbUow.ObjectContext);
            string userNameWithoutDomain = System.Web.HttpContext.Current.User.Identity.Name.Substring(System.Web.HttpContext.Current.User.Identity.Name.LastIndexOf('\\') + 1);
            Staff staffrecord = staffRepository.Find(x => x.StaffNumber == userNameWithoutDomain).SingleOrDefault();
            string staffGuid = "";
            if (staffrecord != null)
            {
                staffGuid = staffrecord.Code.ToString();
            }
            return staffGuid;
        }

        [Invoke]
        public IEnumerable<string> GetReferenceData(Guid dataTableCode, string dataFieldName,string startsWith)
        {

            UcbManagementInformation.Server.DataAccess.DataTable CurrentDataTable = dataTableRepository.GetByCode(dataTableCode);
            string DataTableName = CurrentDataTable.Name;
            string DataTableSchemaName = CurrentDataTable.SchemaName;

            UcbManagementInformation.Server.DataAccess.DataModel CurrentDataModel = dataModelRepository.GetByCode(CurrentDataTable.DataModelCode);
           
            string DatasourceName = CurrentDataTable.DataModel.DatasourceName;
            string UserGuid = GetUserGuid();
            return new UcbManagementInformationEntities().GetReferenceData1(DataTableName, DataTableSchemaName, dataFieldName, DatasourceName, startsWith,UserGuid).AsEnumerable<string>();
        }
        // THIS CODE IS USED TO POPULATE THE DATATABLERELATIONSHIP AND DATATABLERELATIONSHIPJOINS FROM THE DATATABLEJOIN TABLE.
        public bool InsertRelationships()
        {
            Guid DataModelCodeToUse = Guid.Parse("3A1DF010-7F7B-4619-B937-6781AC8F9089");
            List<DataTableJoin> dtjrep = new List<DataTableJoin>(dataTableJoinRepository.Find(x => x.DataModelCode == DataModelCodeToUse));//INES Model
            List<UcbManagementInformation.Server.DataAccess.DataTable> dtrep = new List<UcbManagementInformation.Server.DataAccess.DataTable>(dataTableRepository.Find(x => x.DataModelCode == DataModelCodeToUse));

            List<DataTableRelationship> allRelationships = new List<DataTableRelationship>();
            //List<DataTableJoin> joinPath = new List<DataTableJoin>();
            using (UcbManagementInformationEntities context = new UcbManagementInformationEntities())
            {
                foreach (UcbManagementInformation.Server.DataAccess.DataTable currentStartTable in dtrep)
                {

                    FindRelationships(new List<DataTableJoin>(), allRelationships, currentStartTable, currentStartTable, dtjrep, dtrep, context);

                }
                List<DataTableRelationship> originalRels = new List<DataTableRelationship>();
                originalRels.AddRange(allRelationships);
                foreach (UcbManagementInformation.Server.DataAccess.DataTable currentStartTable in dtrep)
                {

                    AddParentRelationships(new List<DataTableJoin>(), allRelationships, originalRels, currentStartTable, currentStartTable, dtjrep, dtrep, context);

                }

                AddRelationshipsForManyToMany("StaffOrganisation", "Staff", allRelationships, context);
                //AddRelationshipsForManyToMany("vwTeam", "Staff", allRelationships, context);


                //AddRelationshipsForManyToMany("ControlMeasure", "Customer", allRelationships,context);
                //AddRelationshipsForManyToMany("ContingencyArrangement", "Customer", allRelationships, context);
                //AddRelationshipsForManyToMany("InterestedParty", "Incident", allRelationships, context);
                //AddRelationshipsForManyToMany("SystemMarked", "Incident", allRelationships, context);
                //AddRelationshipsForManyToMany("Staff", "Site", allRelationships, context);

                using (UnitOfWork UoW = new UnitOfWork((IObjectContext)context))
                {
                    UoW.Commit();
                }
            }
            return true;
        }

        public IQueryable<DataTableJoin> RetreiveJoinList(List<Guid> TableCodes)
        {
            return QueryBuilder.RetreiveTableJoins(TableCodes).AsQueryable();
        }

        void FindRelationships(List<DataTableJoin> joinPath, List<DataTableRelationship> allRelationships, UcbManagementInformation.Server.DataAccess.DataTable startTable,
            UcbManagementInformation.Server.DataAccess.DataTable currentTable, List<DataTableJoin> allJoins, List<UcbManagementInformation.Server.DataAccess.DataTable> allTables,
            UcbManagementInformationEntities context)
        {
            var JoinsForCurrentTable = from x in allJoins
                                       where x.ToTable == currentTable.Name
                                       select x;
            foreach (DataTableJoin currentJoin in JoinsForCurrentTable)
            {
                UcbManagementInformation.Server.DataAccess.DataTable FromTable = (from y in allTables where y.Name == currentJoin.FromTable select y).First<UcbManagementInformation.Server.DataAccess.DataTable>();
                //joinPath.Add(currentJoin);
                List<DataTableJoin> joinPathToPass = new List<DataTableJoin>(joinPath);
                joinPathToPass.Add(currentJoin);
                DataTableRelationship newRelationShip = new DataTableRelationship();
                newRelationShip.Code = Guid.NewGuid();
                newRelationShip.DataTable = context.DataTables.Where(c => c.Code == startTable.Code).First();
                newRelationShip.DataTable1 = context.DataTables.Where(c => c.Code == FromTable.Code).First();
                newRelationShip.JoinInfo = " ";
                newRelationShip.DataTableFromCode = startTable.Code;
                newRelationShip.DataTableToCode = FromTable.Code;

                var RelationShipsExisting = from y in allRelationships
                                            where y.DataTable.Code == newRelationShip.DataTableFromCode && y.DataTable1.Code == newRelationShip.DataTableToCode
                                            || y.DataTable.Code == newRelationShip.DataTableToCode && y.DataTable1.Code == newRelationShip.DataTableFromCode
                                            select y;
                if (RelationShipsExisting.Count<DataTableRelationship>() == 0)
                {
                    if (newRelationShip.DataTableFromCode != newRelationShip.DataTableToCode && joinPathToPass.Count > 0)
                    {
                        context.DataTableRelationships.AddObject(newRelationShip);

                        allRelationships.Add(newRelationShip);
                        foreach (DataTableJoin joininPath in joinPathToPass)
                        {
                            DataTableRelationshipJoin dtrj = new DataTableRelationshipJoin();
                            dtrj.Code = Guid.NewGuid();
                            dtrj.DataTableJoin = context.DataTableJoins.Where(c => c.Code == joininPath.Code).First();
                            //dtrj.DataTableRelationship = context.DataTableRelationships.First();// .Where(c => c.Code == newRelationShip.Code).First(); 
                            dtrj.DataTableJoinCode = joininPath.Code;
                            dtrj.DataTableRelationshipCode = newRelationShip.Code;
                            //context.Attach(dtrj);
                            context.DataTableRelationshipJoins.AddObject(dtrj);
                            newRelationShip.DataTableRelationshipJoins.Add(dtrj);
                        }
                    }
                }
                else
                {
                    var joins = (from x in joinPathToPass where x.FromTable == currentJoin.FromTable && x.ToTable == currentJoin.ToTable select x).ToList<DataTableJoin>();
                    //foreach (DataTableJoin dtj in joins)
                    //{
                    //    joinPathToPass.Remove(dtj);
                    //}
                    //joinPathToPass.Remove(currentJoin);

                }
                FindRelationships(joinPathToPass, allRelationships, startTable, FromTable, allJoins, allTables, context);

            }
        }

        void AddParentRelationships(List<DataTableJoin> joinPath, List<DataTableRelationship> allRelationships, List<DataTableRelationship> originalRelationships,
            UcbManagementInformation.Server.DataAccess.DataTable startTable,
            UcbManagementInformation.Server.DataAccess.DataTable currentTable, List<DataTableJoin> allJoins, List<UcbManagementInformation.Server.DataAccess.DataTable> allTables,
            UcbManagementInformationEntities context)
        {

            var allRelationShipsForStartTable = from y in originalRelationships
                                                where y.DataTable.Code == startTable.Code || y.DataTable1.Code == startTable.Code
                                                select y;
            foreach (DataTableRelationship currentRelationship in allRelationShipsForStartTable)
            {
                joinPath = (from z in currentRelationship.DataTableRelationshipJoins
                            select z.DataTableJoin).ToList<DataTableJoin>();

                FindRelationships(joinPath, allRelationships, startTable, (currentRelationship.DataTableFromCode == startTable.Code ? currentRelationship.DataTable1 : currentRelationship.DataTable),
                allJoins, allTables, context);
            }

        }

        void AddRelationshipsForManyToMany(string TableRequiringRelationships, string TableToCopy,List<DataTableRelationship> allRelationships, UcbManagementInformationEntities context)
        {
            var a = allRelationships.Select(x => new {First = x.DataTable.Name, Second = x.DataTable1.Name });
            DataTableRelationship RelForManyToMany = (from x in allRelationships
                                                      where (x.DataTable.Name == TableRequiringRelationships && x.DataTable1.Name == TableToCopy)
                                                            || (x.DataTable1.Name == TableRequiringRelationships && x.DataTable.Name == TableToCopy) && x.DataTableRelationshipJoins.Count()>0
                                                            select x).First();

            var JoinsForManyToMany = RelForManyToMany.DataTableRelationshipJoins.Select(z => z.DataTableJoin);
            List<DataTableJoin> JoinsToRelationShips = JoinsForManyToMany.ToList();
            var allRelationshipsForTableToCopy = (from y in allRelationships
                                                 where y.DataTable.Name == TableToCopy || y.DataTable1.Name == TableToCopy
                                                 select y).ToList();
            var allRelationshipsForTableRequiringRelationships = from z in allRelationships
                                                                 where z.DataTable.Name == TableRequiringRelationships || z.DataTable1.Name == TableRequiringRelationships
                                                                 select z;
            foreach (var relationshipTocopy in allRelationshipsForTableToCopy)
            {
                bool RelAlreadyExists = false;
                bool isFromTableTheOneToCopy = (relationshipTocopy.DataTable.Name == TableToCopy);
                string othertablename = isFromTableTheOneToCopy ? relationshipTocopy.DataTable1.Name : relationshipTocopy.DataTable.Name;
                var foundrel = from b in allRelationshipsForTableRequiringRelationships
                               where
                                (b.DataTable.Name == othertablename && b.DataTable1.Name == TableRequiringRelationships) ||
                                (b.DataTable1.Name == othertablename && b.DataTable.Name == TableRequiringRelationships)
                               select b;
                if (foundrel != null && foundrel.Count() > 0)
                {
                    RelAlreadyExists = true;
                }
                if (!RelAlreadyExists && relationshipTocopy.DataTableRelationshipJoins.Count()>0)
                {
                    DataTableRelationship newRelationShip = new DataTableRelationship();
                    newRelationShip.Code = Guid.NewGuid();
                    if (isFromTableTheOneToCopy)
                    {
                        newRelationShip.DataTable = context.DataTables.Where(c => c.Name == TableRequiringRelationships).First();
                        newRelationShip.DataTable1 = context.DataTables.Where(c => c.Name == othertablename).First();
                        newRelationShip.DataTableFromCode = newRelationShip.DataTable.Code;
                        newRelationShip.DataTableToCode = newRelationShip.DataTable1.Code;
                    
                    }
                    else
                    {
                        newRelationShip.DataTable1 = context.DataTables.Where(c => c.Name == TableRequiringRelationships).First();
                        newRelationShip.DataTable = context.DataTables.Where(c => c.Name == othertablename).First();
                        newRelationShip.DataTableFromCode = newRelationShip.DataTable.Code;
                        newRelationShip.DataTableToCode = newRelationShip.DataTable1.Code;
                    }
                    newRelationShip.JoinInfo = " ";
                    context.DataTableRelationships.AddObject(newRelationShip);
                    foreach (DataTableRelationshipJoin joininRel in relationshipTocopy.DataTableRelationshipJoins)
                    {
                        DataTableRelationshipJoin dtrj = new DataTableRelationshipJoin();
                        dtrj.Code = Guid.NewGuid();
                        dtrj.DataTableJoin = context.DataTableJoins.Where(c => c.Code == joininRel.DataTableJoinCode).First();
                        //dtrj.DataTableRelationship = context.DataTableRelationships.First();// .Where(c => c.Code == newRelationShip.Code).First(); 
                        dtrj.DataTableJoinCode = joininRel.DataTableJoinCode;
                        dtrj.DataTableRelationshipCode = newRelationShip.Code;
                        //context.Attach(dtrj);
                        context.DataTableRelationshipJoins.AddObject(dtrj);
                        newRelationShip.DataTableRelationshipJoins.Add(dtrj);
                    }
                    foreach (DataTableJoin joinsToAddRel in JoinsToRelationShips)
                    {
                        DataTableRelationshipJoin dtrj = new DataTableRelationshipJoin();
                        dtrj.Code = Guid.NewGuid();
                        dtrj.DataTableJoin = context.DataTableJoins.Where(c => c.Code == joinsToAddRel.Code).First();
                        //dtrj.DataTableRelationship = context.DataTableRelationships.First();// .Where(c => c.Code == newRelationShip.Code).First(); 
                        dtrj.DataTableJoinCode = joinsToAddRel.Code;
                        dtrj.DataTableRelationshipCode = newRelationShip.Code;
                        //context.Attach(dtrj);
                        context.DataTableRelationshipJoins.AddObject(dtrj);
                        newRelationShip.DataTableRelationshipJoins.Add(dtrj);
                    }
                    allRelationships.Add(newRelationShip);
                }
            }
        }
      


        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DataCategories' query.
        public IQueryable<DataCategory> GetDataCategories()
        {
            return dataCategoryRepository.GetAll().AsQueryable();
        }

        public void InsertDataCategory(DataCategory dataCategory)
        {
            dataCategoryRepository.Add(dataCategory);
        }

        public void UpdateDataCategory(DataCategory currentDataCategory)
        {
            dataCategoryRepository.Update(currentDataCategory);
       }

        public void DeleteDataCategory(DataCategory dataCategory)
        {
            dataCategoryRepository.Delete(dataCategory);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DataItems' query.
        public IQueryable<DataItem> GetDataItems()
        {
            return dataItemRepository.GetAll().AsQueryable();
        }

        public void InsertDataItem(DataItem dataItem)
        {
            dataItemRepository.Add(dataItem);
        }

        public void UpdateDataItem(DataItem currentDataItem)
        {
            dataItemRepository.Update(currentDataItem);
        }

        public void DeleteDataItem(DataItem dataItem)
        {
            dataItemRepository.Delete(dataItem);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DataModels' query.
        public IQueryable<DataModel> GetDataModels()
        {
            return dataModelRepository.GetAll().AsQueryable();
        }

        public void InsertDataModel(DataModel dataModel)
        {
            dataModelRepository.Add(dataModel);
        }

        public void UpdateDataModel(DataModel currentDataModel)
        {
            dataModelRepository.Update(currentDataModel);
        }

        public void DeleteDataModel(DataModel dataModel)
        {
            dataModelRepository.Delete(dataModel);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DataTables' query.
        public IQueryable<UcbManagementInformation.Server.DataAccess.DataTable> GetDataTables()
        {
            return dataTableRepository.GetAll().AsQueryable();
        }

        public void InsertDataTable(UcbManagementInformation.Server.DataAccess.DataTable dataTable)
        {
            dataTableRepository.Add(dataTable);
        }

        public void UpdateDataTable(UcbManagementInformation.Server.DataAccess.DataTable currentDataTable)
        {
            dataTableRepository.Update(currentDataTable);
        }

        public void DeleteDataTable(UcbManagementInformation.Server.DataAccess.DataTable dataTable)
        {
            dataTableRepository.Delete(dataTable);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DataTableJoins' query.
        public IQueryable<DataTableJoin> GetDataTableJoins()
        {
            return dataTableJoinRepository.GetAll().AsQueryable();
        }

        public void InsertDataTableJoin(DataTableJoin dataTableJoin)
        {
            dataTableJoinRepository.Add(dataTableJoin);
        }

        public void UpdateDataTableJoin(DataTableJoin currentDataTableJoin)
        {
            dataTableJoinRepository.Update(currentDataTableJoin);
        }

        public void DeleteDataTableJoin(DataTableJoin dataTableJoin)
        {
            dataTableJoinRepository.Delete(dataTableJoin);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DataTableRelationships' query.
        public IQueryable<DataTableRelationship> GetDataTableRelationships()
        {
            return dataTableRelationshipRepository.GetAll().AsQueryable();
        }

        public void InsertDataTableRelationship(DataTableRelationship dataTableRelationship)
        {
            dataTableRelationshipRepository.Add(dataTableRelationship);
        }

        public void UpdateDataTableRelationship(DataTableRelationship currentDataTableRelationship)
        {
            dataTableRelationshipRepository.Update(currentDataTableRelationship);
        }

        public void DeleteDataTableRelationship(DataTableRelationship dataTableRelationship)
        {
            dataTableRelationshipRepository.Delete(dataTableRelationship);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'DataTableRelationshipJoins' query.
        public IQueryable<DataTableRelationshipJoin> GetDataTableRelationshipJoins()
        {
            return dataTableRelationshipJoinRepository.GetAll().AsQueryable();
        }

        public void InsertDataTableRelationshipJoin(DataTableRelationshipJoin dataTableRelationshipJoin)
        {
            dataTableRelationshipJoinRepository.Add(dataTableRelationshipJoin);
        }

        public void UpdateDataTableRelationshipJoin(DataTableRelationshipJoin currentDataTableRelationshipJoin)
        {
            dataTableRelationshipJoinRepository.Update(currentDataTableRelationshipJoin);
        }

        public void DeleteDataTableRelationshipJoin(DataTableRelationshipJoin dataTableRelationshipJoin)
        {
            dataTableRelationshipJoinRepository.Delete(dataTableRelationshipJoin);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Filters' query.
        public IQueryable<Filter> GetFilters()
        {
            return filterRepository.GetAll().AsQueryable();
        }

        public void InsertFilter(Filter filter)
        {
            filterRepository.Add(filter);
        }

        public void UpdateFilter(Filter currentFilter)
        {
            filterRepository.Update(currentFilter);
        }

        public void DeleteFilter(Filter filter)
        {
            filterRepository.Delete(filter);
        }




        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ReportCharts' query.
        public IQueryable<ReportChart> GetReportCharts()
        {
            return reportChartRepository.GetAll().AsQueryable();
        }

        public void InsertReportChart(ReportChart reportChart)
        {
            reportChartRepository.Add(reportChart);
        }

        public void UpdateReportChart(ReportChart currentReportChart)
        {
            reportChartRepository.Update(currentReportChart);
        }

        public void DeleteReportChart(ReportChart reportChart)
        {
            reportChartRepository.Delete(reportChart);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ReportChartSeries' query.
        public IQueryable<ReportChartSery> GetReportChartSeries()
        {
            return reportChartSeriesRepository.GetAll().AsQueryable();
        }

        public void InsertReportChartSery(ReportChartSery reportChartSery)
        {
            reportChartSeriesRepository.Add(reportChartSery);
        }

        public void UpdateReportChartSery(ReportChartSery currentReportChartSery)
        {
            reportChartSeriesRepository.Update(currentReportChartSery);
        }

        public void DeleteReportChartSery(ReportChartSery reportChartSery)
        {
            reportChartSeriesRepository.Delete(reportChartSery);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ReportDataTableJoins' query.
        public IQueryable<ReportDataTableJoin> GetReportDataTableJoins()
        {
            return reportDataTableJoinRepository.GetAll().AsQueryable();
        }

        public void InsertReportDataTableJoin(ReportDataTableJoin reportDataTableJoin)
        {
            reportDataTableJoinRepository.Add(reportDataTableJoin);
        }

        public void UpdateReportDataTableJoin(ReportDataTableJoin currentReportDataTableJoin)
        {
            reportDataTableJoinRepository.Update(currentReportDataTableJoin);
        }

        public void DeleteReportDataTableJoin(ReportDataTableJoin reportDataTableJoin)
        {
            reportDataTableJoinRepository.Delete(reportDataTableJoin);
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ReportGroups' query.
        public IQueryable<ReportGroup> GetReportGroups()
        {
            return reportGroupRepository.GetAll().AsQueryable();
        }
        public IQueryable<ReportGroup> GetReportGroupsForUser()
        {
            return reportGroupRepository.GetQuery().AsQueryable();
        }
        public void InsertReportGroup(ReportGroup reportGroup)
        {
            //Store away ations for reorting services.
            reportingServicesAction = "InsertGroup";
            reportGroupUpdate = reportGroup;
            reportGroupRepository.Add(reportGroup);
            
        }

        public void UpdateReportGroup(ReportGroup currentReportGroup)
        {
            
            //ReportGroup reportGroupOld = this.ChangeSet.GetOriginal(currentReportGroup);
            //Use second repository with different context.
            ReportGroup OriginalGroup = reportGroupRepositoryRead.Find(x => x.Code == currentReportGroup.Code).First();
            reportGroupRepository.Update(currentReportGroup);
            if (currentReportGroup.Name != OriginalGroup.Name)
            {
                originalGroup = OriginalGroup;
                //Store away ations for reorting services.
                reportingServicesAction = "UpdateGroup";
                reportGroupUpdate = currentReportGroup;

                
            }
        }

        public void DeleteReportGroup(ReportGroup reportGroup)
        {
            //retreive the full reportGroup graph for delete
            ReportGroup reportGroupToDelete = GetReportGroupByCode(reportGroup.Code);
            //Need to set Row id of report to ensure optimistic concurrency is maintained.
            reportGroupToDelete.RowIdentifier = reportGroup.RowIdentifier;
            
            //Store away ations for reorting services.
            reportingServicesAction = "DeleteGroup";
            reportGroupUpdate = reportGroupToDelete;

            reportGroupRepository.Delete(reportGroupToDelete);
            
        }

        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'ReportItems' query.
        public IQueryable<ReportItem> GetReportItems()
        {
            return reportItemRepository.GetAll().AsQueryable();
        }

        public void InsertReportItem(ReportItem reportItem)
        {
            reportItemRepository.Add(reportItem);
        }

        public void UpdateReportItem(ReportItem currentReportItem)
        {
            reportItemRepository.Update(currentReportItem);
        }

        public void DeleteReportItem(ReportItem reportItem)
        {
            reportItemRepository.Delete(reportItem);
        }



        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'UserReportGroups' query.
        public IQueryable<UserReportGroup> GetUserReportGroups()
        {
            return userReportGroupRepository.GetAll().AsQueryable();
        }

        public void InsertUserReportGroup(UserReportGroup userReportGroup)
        {
            userReportGroupRepository.Add(userReportGroup);
        }

        public void UpdateUserReportGroup(UserReportGroup currentUserReportGroup)
        {
            userReportGroupRepository.Update(currentUserReportGroup);
        }

        public void DeleteUserReportGroup(UserReportGroup userReportGroup)
        {
            userReportGroupRepository.Delete(userReportGroup);
        }
        // TODO:
        // Consider constraining the results of your query method.  If you need additional input you can
        // add parameters to this method or create additional query methods with different names.
        // To support paging you will need to add ordering to the 'Reports' query.
        public IQueryable<Report> GetReports()
        {
            return reportRepository.GetAll().AsQueryable();
        }

        public void InsertReport(Report report)
        {
            //if report already exists then update.
            var reportFound = reportRepository.Find(x => x.Name == report.Name && x.ReportGroup.Code == report.GroupCode);
            if (reportFound != null && reportFound.Count() == 1)
            {
                DeleteReport(reportFound.First());
            }
            //Override delete with insert so we don't delete the report we are inserting
            reportingServicesAction = "InsertReport";
            reportRepository.Add(report);
            GenerateReport(report); //create in reporting services
            //this.ObjectContext.SaveChanges();

        }

        public void UpdateReport(Report currentReport)
        {
            reportRepository.Update(currentReport);
            GenerateReport(currentReport);
            //this.ObjectContext.SaveChanges();
        }

        public void DeleteReport(Report report)
        {
            //retreive the full report graph for delete
            Report reportToDelete= GetReportByCode(report.Code);
            //Need to set Row id of report to ensure optimistic concurrency is maintained.
            reportToDelete.RowIdentifier = report.RowIdentifier;
            reportGroupUpdate = reportGroupRepository.Find(x => x.Code == report.GroupCode).First();
           
            //Delete the actual report
            reportRepository.Delete(reportToDelete);
            reportingServicesAction = "DeleteReport";
            reportForDeleting = report;
            //this.ObjectContext.SaveChanges();
        }


        //private EntityState GetEntityState(object entity)
        //{
        //    System.Data.Objects.ObjectStateEntry ose;
        //    if (this.ObjectContext.ObjectStateManager.TryGetObjectStateEntry(entity, out ose))
        //        return ose.State;
        //    else
        //        return EntityState.Detached;
        //}
        public IQueryable<ReportCategory> GetReportCategorys()
        {
            return reportCategoryRepository.GetAll().AsQueryable();
        }

        public IQueryable<StandardReport> GetStandardReports()
        {
            return standardReportRepository.GetAll().AsQueryable();
        }

        /// <summary>
        ///Publish to Benenefit Cap app
        /// </summary>
        /// <param name="standardReport"></param>
        [Insert]
        public void InsertStandardReport(StandardReport standardReport)
        {
            var foundReportList = standardReportRepository.Find(x => x.ReportName == standardReport.ReportName).FirstOrDefault();
            if (foundReportList != null)
            {
                foundReportList.IsExportable = standardReport.IsExportable;
                foundReportList.IsPrintable = standardReport.IsPrintable;
                foundReportList.ReportCategoryCode = standardReport.ReportCategoryCode;
                foundReportList.ReportDescription = standardReport.ReportDescription;
                foundReportList.ReportName = standardReport.ReportName;
                foundReportList.ReportToPublishCode = standardReport.ReportToPublishCode;
                reportingServicesAction = "UpdateStandardReport";
                standardReportUpdate = foundReportList;
            }
            else
            {
                reportingServicesAction = "InsertStandardReport";
                //Store away actions for reorting services.
                standardReport.Code = Guid.NewGuid();
                standardReport.ReportUrl = "";
                standardReportUpdate = standardReport;
                standardReportRepository.Add(standardReport);
            }
        }

        protected override bool PersistChangeSet()
        {
            switch (reportingServicesAction)
            {
                case "InsertGroup":
                    if (!ReportService.InsertFolder(reportGroupUpdate))
                    {
                        throw new Exception("Failed to add report group to reporting services");
                    }
                    break;
                case "UpdateGroup":
                    //only update reporting services if name has changed.
                    if (!ReportService.RenameFolder(reportGroupUpdate, originalGroup.PathName))
                    {
                        throw new Exception("Failed to add report group to reporting services");
                    }
                    break;
                case "DeleteGroup":
                    if (!ReportService.DeleteFolder(reportGroupUpdate))
                    {
                        throw new Exception("Failed to remove report group from reporting services");
                    }
                    break;
                case "InsertStandardReport":
                case "UpdateStandardReport":
                    IUcbManagementInformationRepository<Report> repository = DataAccessUtilities.RepositoryLocator<IUcbManagementInformationRepository<Report>>();
                    var report = repository.Single(x => x.Code == standardReportUpdate.ReportToPublishCode, "DataModel_1", "ReportGroup");

                    if (!ReportService.CopyReport(report, standardReportUpdate))
                    {
                        throw new Exception("Failed to add report group to reporting services");
                    }
                    break;
            }
            
            bool isWorking = base.PersistChangeSet();
            
            //Update reporting services based on action above
            //if commit fails, undo reporting services changes.
            try
            {
                uow.Commit();
                UcbUow.Commit();
                switch (reportingServicesAction)
                {
                    case "DeleteReport":
                        try
                        {
                            if (!ReportService.DeleteReport(reportGroupUpdate, reportForDeleting))
                            {
                                //NOT IMPORTANT IF DELETE FAILS IN RS but only do it if db has succeeded
                            }
                        }
                        catch { }
                        break;
                }
            }
            catch
            {
                switch (reportingServicesAction)
                {
                    case "InsertGroup":
                        if (!ReportService.DeleteFolder(reportGroupUpdate))
                        {
                            throw new Exception("Failed to delete report group from Reporting services afdter EF failure");
                        }
                        break;
                    case "UpdateGroup":
                        //only update reporting services if name has changed.
                        if (!ReportService.RenameFolder(originalGroup, reportGroupUpdate.PathName))
                        {
                            throw new Exception("Failed to rename report group in reporting services following EF failure");
                        }
                        break;
                    case "DeleteGroup":
                        if (!ReportService.InsertFolder(reportGroupUpdate))
                        {
                            throw new Exception("Failed to insert report group to reporting services following EF failure");
                        }
                        break;

                }
                throw;
            }
            return isWorking;
        }
    }
}



