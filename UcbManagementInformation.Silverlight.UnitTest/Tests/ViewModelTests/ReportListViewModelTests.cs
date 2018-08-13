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
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UcbManagementInformation.Silverlight.UnitTest.Helpers;
using System.ServiceModel.DomainServices.Client;
using UcbManagementInformation.Web.Server;
using UcbManagementInformation.ViewModels;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Web.Models;
using System.Linq;
using System.Security.Principal;
using UcbManagementInformation.Server.DataAccess;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;
using System.Threading;
using UcbManagementInformation.Helpers;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Web.Services;

namespace UcbManagementInformation.Silverlight.UnitTest.Tests.ViewModelTests
{
    [TestClass]
    public class ReportListViewModelTests : SilverlightTest
    {
        #region example
        /*
        //Example below shows how a specific user could be loaded for these tests. May be useful if we ever
        //need to test users with specific roles.
        [TestInitialize]
        public void TestSetUp()
        {
            MCUser testUser = new MCUser 
            { 
                Name = "Test2" ,
                Roles= new List<string>()
                {
                    "Administrator","CAUser"
                } 
            };
            FakeAuthentication.FakeUser = testUser;
            WebContext.Current.Authentication.LoadUser();
        }
        */
        #endregion

        #region class fields
        
        ReportListViewModel _constructedEmptyViewModel=null;
        
        #endregion

        #region Constructors
        public ReportListViewModelTests()    
        {
            _constructedEmptyViewModel = CreateEmptyViewModel();
        }
        #endregion

        #region utility methods

        private ReportListViewModel CreateEmptyViewModel()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            //UcbPublishContext publishContext = new UcbPublishContext(mockDomainClient2.Object);
            ReportListViewModel constructedEmptyViewModel = new ReportListViewModel(context,new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
           
            return constructedEmptyViewModel;
        }

        #endregion

        #region Constructor Tests 

        #endregion

        #region Initialize Tests 
        [TestMethod]
        [Asynchronous]
        public void Initialize_IsRetrievingReportGroupsIsInitiallySetToTrue()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            //UcbPublishContext publishContext = new UcbPublishContext(mockDomainClient2.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
           
            ////Assert
            viewModel.InitializeCommand.Execute(null);
            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.IsRetreivingReportGroups == true, "IsRetrievingReportReportGroups was not set to true");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void Initialize_RootGroupsGetsPopulatedWithTheReportGroups()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                //.Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {
                        AccessLevel = ReportGroupAccessLevelType.None,
                        Name="ESF2007MIReports", 
                        PathName="ESF2007MIReports/"},

                    new ReportGroup {
                        AccessLevel = ReportGroupAccessLevelType.None,
                        Name="Sub Committees", 
                        ParentPath="ESF2007MIReports/",
                        PathName="ESF2007MIReports/Sub Committee/"}
                }.AsQueryable());

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(Report))))
                //.Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new Report {}
 
                }.AsQueryable());


            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            viewModel.InitializeCommand.Execute(null);
            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.RootGroups.Count == 2, "RootGroups was not populated with the correct number of ReportGroupFolders"); 
            });
            EnqueueTestComplete();
        }

        

        [TestMethod]
        [Asynchronous]
        public void Constructor_IsRetrievingReportGroupsIsSetToFalseWhenTheReportGroupsAreLoaded()
        {

            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
           
            viewModel.InitializeCommand.Execute(null);
            
            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.IsRetreivingReportGroups == false, "IsRetrievingReportReportGroups was not set to false");
            });
            EnqueueTestComplete();
        }
        #endregion

        #region PropertyChanged Tests

        [TestMethod]
        public void Reports_ShouldRaisePropertyChangeWhenAltered()
        {
            _constructedEmptyViewModel.AssertRaisesPropertyChangedFor("Reports");
        }

        [TestMethod]
        public void SelectedReport_ShouldRaisePropertyChangeWhenAltered()
        {
            _constructedEmptyViewModel.AssertRaisesPropertyChangedFor("SelectedReport");
        }

        [TestMethod]
        public void SelectedReportGroup_ShouldRaisePropertyChangeWhenAltered()
        {
            _constructedEmptyViewModel.AssertRaisesPropertyChangedFor("SelectedReportGroup");
        }

        [TestMethod]
        [Asynchronous]
        public void SelectedReportGroup_ShouldLoadReportsWhenAlteredIfUserAccessIsGreaterThanReadOnly()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(Report))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new Report {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);


            viewModel.SelectedReportGroup = new ReportGroupFolder
            {
                Name = "TestGroup",
                AccessLevel = ReportGroupAccessLevelType.Update
            };

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(loadExecuted == true, "GetAllReportsForReportGroupQuery was not called");
            });
            EnqueueTestComplete();

        }
        [TestMethod]
        [Asynchronous]
        public void SelectedReportGroup_ShouldNotLoadReportsWhenAlteredIfUserAccessIsLessThanReadOnly()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(Report))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new Report {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
           

            //Even though this test should not do a load, the test is left as asyncronous 
            //as we want to give it the opportunity to do the load if code is incorrect.
            viewModel.SelectedReportGroup = new ReportGroupFolder
            {
                Name = "TestGroup",
                AccessLevel = ReportGroupAccessLevelType.None
            };

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(loadExecuted == false, "GetAllReportsForReportGroupQuery was called");
            });
            EnqueueTestComplete();

        }
        [TestMethod]
        public void RootGroups_ShouldRaisePropertyChangeWhenAltered()
        {
            _constructedEmptyViewModel.AssertRaisesPropertyChangedFor("RootGroups");
        }

        [TestMethod]
        public void NavigationService_ShouldRaisePropertyChangeWhenAltered()
        {
           _constructedEmptyViewModel.AssertRaisesPropertyChangedFor("NavigationService");
        }

        [TestMethod]
        public void IsRetreivingReports_ShouldRaisePropertyChangeWhenAltered()
        {
            _constructedEmptyViewModel.AssertRaisesPropertyChangedFor("IsRetreivingReports");
           
        }

        [TestMethod]
        public void IsRetreivingReportGroups_ShouldRaisePropertyChangeWhenAltered()
        {
            _constructedEmptyViewModel.AssertRaisesPropertyChangedFor("IsRetreivingReportGroups");
        }
        #endregion

        #region Can View Report Tests

        [TestMethod]
        public void CanViewReport_ShouldBeFalseIfSelectedReportIsNull()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            
            viewModel.SelectedReport = null; //report name left blank
            viewModel.SelectedReportGroup = new ReportGroupFolder //do  have access to this folder
            {
                AccessLevel = ReportGroupAccessLevelType.Admin
            };
            Assert.IsTrue(viewModel.ViewReportCommand.CanExecute(null) == false, "ViewReportCommand should be disabled if no report has been selected");
        }

        [TestMethod]
        public void CanViewReport_ShouldBeFalseIfSelectedReportGroupIsNull()
        {

            ReportListViewModel viewModel = CreateEmptyViewModel();
            
            viewModel.SelectedReport = new Report { Name = "TestReport" };
 
           
            Assert.IsTrue(viewModel.ViewReportCommand.CanExecute(null) == false, "ViewReportCommand should be disabled if no report group has been selected");
        }

        [TestMethod]
        public void CanViewReport_ShouldBeFalseIfAccessLevelIsNone()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            
            viewModel.SelectedReport = new Report { Name = "TestReport" };
            viewModel.SelectedReportGroup = new ReportGroupFolder //do not have access to this folder
            {
                AccessLevel = ReportGroupAccessLevelType.None
            };
           
        }

        [TestMethod]
        public void CanViewReport_ShouldBeTrueIfAccessLevelIsGreaterThanNoneAndReportandReportGroupArePopulated()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            
            viewModel.SelectedReport = new Report { Name = "TestReport" };
            viewModel.SelectedReportGroup = new ReportGroupFolder
            {
                AccessLevel = ReportGroupAccessLevelType.ReadOnly
            };

            Assert.IsTrue(viewModel.ViewReportCommand.CanExecute(null) == true, "ViewReportCommand should be enabled");
           
        }
        #endregion

        #region Can Edit Report Tests
        [TestMethod]
        public void CanEditReport_ShouldBeFalseIfSelectedReportIsNull()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            

            viewModel.SelectedReport = null;
                
            viewModel.SelectedReportGroup = new ReportGroupFolder 
            {
                AccessLevel = ReportGroupAccessLevelType.Admin
            };

            Assert.IsTrue(viewModel.EditReportCommand.CanExecute(null) == false, "EditReportCommand should be disabled");
            
        }

        [TestMethod]
        public void CanEditReport_ShouldBeFalseIfSelectedReportGroupIsNull()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            viewModel.SelectedReport = new Report { Name = "Test", IsStandard = false };
            Assert.IsTrue(viewModel.EditReportCommand.CanExecute(null) == false, "EditReportCommand should be disabled");
          
        }

        [TestMethod]
        public void CanEditReport_ShouldBeFalseIfStandardReport()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            
            viewModel.SelectedReport = new Report { Name = "Test", IsStandard = true };
            viewModel.SelectedReportGroup = new ReportGroupFolder 
            {
                AccessLevel = ReportGroupAccessLevelType.ReadOnly
            };

            Assert.IsTrue(viewModel.EditReportCommand.CanExecute(null) == false, "EditReportCommand should be disabled");
        }

        [TestMethod]
        public void CanEditReport_ShouldBeFalseIfNoAccess()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            viewModel.SelectedReport = new Report { Name = "Test", IsStandard = false };
            viewModel.SelectedReportGroup = new ReportGroupFolder 
            {
                AccessLevel = ReportGroupAccessLevelType.ReadOnly
            };

            Assert.IsTrue(viewModel.EditReportCommand.CanExecute(null) == false, "EditReportCommand should be disabled");
        }

        [TestMethod]
        public void CanEditReport_ShouldBeTrueIfAllOk()
        {
            //ReportListViewModel viewModel = CreateEmptyViewModel();
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            Guid reportGroupGuid = Guid.NewGuid();
            Guid reportGuid = Guid.NewGuid();
            ReportGroup rgForTest = new ReportGroup { Code = reportGroupGuid };
            rgForTest.AccessLevel = ReportGroupAccessLevelType.Update;
            
            var reportForTest = new Report { Code = reportGuid, Name = "Test", IsStandard = false, ReportGroup = rgForTest };
            //need too add to context
            context.Reports.Add(reportForTest);
            
            viewModel.SelectedReport = reportForTest;
            
            Assert.IsTrue(viewModel.EditReportCommand.CanExecute(null) == true, "EditReportCommand should be enabled");
           
        }
        #endregion

        #region Edit Report Tests
        [TestMethod]
        public void EditReport_ShouldSetReportCodeSessionKey()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            Guid ReportGuid = Guid.NewGuid();

            viewModel.SelectedReport = new Report() {Name="Name", Code=ReportGuid};
            Mock NavigationMock = new Mock<INavigationService>();
            viewModel.NavigationService = (INavigationService)NavigationMock.Object;
            viewModel.EditReportCommand.Execute(null);
            Assert.IsTrue((Guid)UcbManagementInformation.App.Session[SessionKey.CurrentReportCode] == ReportGuid, "The report key has not been stored in Session");

        }

        [TestMethod]
        public void EditReport_ShouldNavigateToReportWizard()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            Guid ReportGuid = Guid.NewGuid();

            viewModel.SelectedReport = new Report() { Name = "Name", Code = ReportGuid };
            bool isNavigated=false;
            var NavigationMock = new Mock<INavigationService>();
            NavigationMock.Setup(nav => nav.Navigate("/Reporting/ReportWizard/ReportWizard")).Callback(() => isNavigated = true);
            viewModel.NavigationService = (INavigationService)NavigationMock.Object;
            viewModel.EditReportCommand.Execute(null);
            Assert.IsTrue(isNavigated==true, "Navigation to report wizard did not occur");
        }

        #endregion

        #region Can Delete Reports Tests
        [TestMethod]
        public void CanDeleteReport_ShouldBeFalseIfSelectedReportIsNull()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();


            viewModel.SelectedReport = null;

            viewModel.SelectedReportGroup = new ReportGroupFolder
            {
                AccessLevel = ReportGroupAccessLevelType.Admin
            };

            Assert.IsTrue(viewModel.DeleteReportCommand.CanExecute(null) == false, "DeleteReportCommand should be disabled");

        }

        [TestMethod]
        public void CanDeleteReport_ShouldBeFalseIfSelectedReportGroupIsNull()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            viewModel.SelectedReport = new Report { Name = "Test", IsStandard = false };
            Assert.IsTrue(viewModel.DeleteReportCommand.CanExecute(null) == false, "DeleteReportCommand should be disabled");

        }

        [TestMethod]
        public void CanDeleteReport_ShouldBeFalseIfStandardReport()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();

            viewModel.SelectedReport = new Report { Name = "Test", IsStandard = true };
            viewModel.SelectedReportGroup = new ReportGroupFolder
            {
                AccessLevel = ReportGroupAccessLevelType.ReadOnly
            };

            Assert.IsTrue(viewModel.DeleteReportCommand.CanExecute(null) == false, "DeleteReportCommand should be disabled");
        }

        [TestMethod]
        public void CanDeleteReport_ShouldBeFalseIfNoAccess()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();
            viewModel.SelectedReport = new Report { Name = "Test", IsStandard = false };
            viewModel.SelectedReportGroup = new ReportGroupFolder
            {
                AccessLevel = ReportGroupAccessLevelType.ReadOnly
            };

            Assert.IsTrue(viewModel.DeleteReportCommand.CanExecute(null) == false, "DeleteReportCommand should be disabled");
        }

        [TestMethod]
        public void CanDeleteReport_ShouldBeTrueIfAllOk()
        {
            //ReportListViewModel viewModel = CreateEmptyViewModel();
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            Guid reportGroupGuid = Guid.NewGuid();
            Guid reportGuid = Guid.NewGuid();
            ReportGroup rgForTest = new ReportGroup { Code = reportGroupGuid };
            rgForTest.AccessLevel = ReportGroupAccessLevelType.Delete;

            var reportForTest = new Report { Code = reportGuid, Name = "Test", IsStandard = false, ReportGroup = rgForTest };
            //need too add to context
            context.Reports.Add(reportForTest);

            viewModel.SelectedReport = reportForTest;

            Assert.IsTrue(viewModel.DeleteReportCommand.CanExecute(null) == true, "DeleteReportCommand should be enabled");

        }
        #endregion

        #region Delete Report Tests
        [TestMethod]
        [Asynchronous]
        public void DeleteReport_ShouldSetIsSubmittingToFalseAtEnd()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            Report ReportToDelete = new Report { Code = Guid.NewGuid(), Name = "DeleteReport" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            
            bool submitExecuted = false;
            viewModel.Reports = new ObservableCollection<Report>() {ReportToDelete};
            context.Reports.Attach(ReportToDelete);
            viewModel.SelectedReport = ReportToDelete;
            
            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());
            
            //Act
            viewModel.DeleteReportCommand.Execute(null);
            
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);
            
            EnqueueCallback(() =>
            {
                Assert.IsFalse(viewModel.IsSubmittingContext, "IsSubmitting context not set to false");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void DeleteReport_ShouldCallSubmitWithADeleteChangeSet()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            Report ReportToDelete = new Report { Code = Guid.NewGuid(), Name = "DeleteReport" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;
            viewModel.Reports = new ObservableCollection<Report>() { ReportToDelete };
            context.Reports.Attach(ReportToDelete);
            viewModel.SelectedReport = ReportToDelete;

            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(() => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.DeleteReportCommand.Execute(null);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(context.Reports.Count == 0, "Report was not deleted from context");
                Assert.IsTrue(submitExecuted, "Submit was not called");
                mockDomainClient.Verify(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)),"Submit was not called with 1 report to remove");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void DeleteReport_ShouldSetIsSubmittingToTrueAtStart()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            Report ReportToDelete = new Report { Code = Guid.NewGuid(), Name = "DeleteReport" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;
            viewModel.Reports = new ObservableCollection<Report>() { ReportToDelete };
            context.Reports.Attach(ReportToDelete);
            viewModel.SelectedReport = ReportToDelete;

            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(() => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.DeleteReportCommand.Execute(null);

            ////Assert

            // NO ENQUEUECONDITIONAL BECAUSE WANT TO SEE IssubmittingContext IS SET
            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.IsSubmittingContext, "IsSubmitting context not set to true");
            });
           
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void DeleteReport_ShouldRemoveTheReportFromTheViewModelReportsCollection()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            Report ReportToDelete = new Report { Code = Guid.NewGuid(), Name = "DeleteReport" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;
            viewModel.Reports = new ObservableCollection<Report>() { ReportToDelete };
            context.Reports.Attach(ReportToDelete);
            viewModel.SelectedReport = ReportToDelete;

            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(() => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.DeleteReportCommand.Execute(null);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.Reports.Count == 0, "Report was not deleted from the view model collection");
            });
            EnqueueTestComplete();
        }
        #endregion

        #region Can Insert Folder Tests
        [TestMethod]
        public void CanAddChildFolder_ShouldBeTrueIfAccessLevelIsGreaterThanUpdate()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();

            ReportGroupFolder ReportGroupToAddChildTo = new ReportGroupFolder
            {
                AccessLevel = ReportGroupAccessLevelType.Update
            };

            Assert.IsTrue(viewModel.NewFolderCommand.CanExecute(ReportGroupToAddChildTo) == true, "NewFolderCommand should be enabled");

        }
        [TestMethod]
        public void CanAddChildFolder_ShouldBeFalseIfAccessLevelIsNotGreaterThanUpdate()
        {
            ReportListViewModel viewModel = CreateEmptyViewModel();

            ReportGroupFolder ReportGroupToAddChildTo = new ReportGroupFolder
            {
                AccessLevel = ReportGroupAccessLevelType.ReadOnly
            };

            Assert.IsTrue(viewModel.NewFolderCommand.CanExecute(ReportGroupToAddChildTo) == false, "NewFolderCommand should be disabled");
        }
        #endregion

        #region Add Folder Tests
        [TestMethod]
        [Asynchronous]
        public void AddChildFolder_ShouldCallSubmitWithOneFolderInChangeSet()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder ReportFolderToAdd = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "AddChildToThis", FullPath = "Root/AddChildToThis/" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;
            
            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.NewFolderCommand.Execute(ReportFolderToAdd);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(submitExecuted, "Submit was not called");
                mockDomainClient.Verify(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)),"Submit not called with single insert");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void AddChildFolder_ShouldSetParentFolderToExpanded()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder ReportFolderToAdd = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "AddChildToThis", FullPath = "Root/AddChildToThis/" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;

            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.NewFolderCommand.Execute(ReportFolderToAdd);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(ReportFolderToAdd.IsExpanded == true, "Didn't expand the parent");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void AddChildFolder_ShouldAddTheReportGroupToTheContext()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder ReportFolderToAdd = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "AddChildToThis", FullPath = "Root/AddChildToThis/" };
            ReportGroup ReportGroupToAdd = new ReportGroup() { Code = ReportFolderToAdd.Code, Name = ReportFolderToAdd.Name, PathName = ReportFolderToAdd.FullPath };
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            context.ReportGroups.Attach(ReportGroupToAdd);
            bool submitExecuted = false;

            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.NewFolderCommand.Execute(ReportFolderToAdd);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                 Assert.IsTrue(context.ReportGroups.Count == 2, "Didn't add report group to context");
                 var x = (from y in context.ReportGroups
                         where y.Name == "New Folder"
                         select y).Single();
                 //Assert.IsTrue(x.AccessLevel == ReportGroupAccessLevelType.Admin, "New Report Group is not Admin Access");
                 Assert.IsTrue(x.ParentPath == ReportFolderToAdd.FullPath, "New Report Group parent path not set");
                 Assert.IsTrue(x.ParentCode == ReportFolderToAdd.Code, "New Report Group parent code not set");
                 Assert.IsTrue(x.PathName == ReportFolderToAdd.FullPath + x.Name + "/", "New Report Group Pathname is not set");
                 Assert.IsTrue(x.ReportType == "A", "New Report Group report Type should be A");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void AddChildFolder_ShouldAddTheFolderToTheCurrentGroupsChildren()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder ReportFolderToAdd = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "AddChildToThis", FullPath = "Root/AddChildToThis/" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;

            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.NewFolderCommand.Execute(ReportFolderToAdd);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(ReportFolderToAdd.Children.Count == 1, "Child folder not added to parent");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void AddChildFolder_ShouldSetIsUpdatingToFalseAtEnd()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder ReportFolderToAdd = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "AddChildToThis", FullPath = "Root/AddChildToThis/" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;

            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.NewFolderCommand.Execute(ReportFolderToAdd);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsFalse(viewModel.IsUpdatingReportGroups, "IsUpdatingReportGroups not set to false");
                Assert.IsFalse(viewModel.IsSubmittingContext, "IsSubmitting context not set to false");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void AddChildFolder_ShouldSetIsUpdatingToTrueAtStart()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder ReportFolderToAdd = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "AddChildToThis", FullPath = "Root/AddChildToThis/" };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;

            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.NewFolderCommand.Execute(ReportFolderToAdd);

            //No conditional as we are testing before the callback
            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.IsUpdatingReportGroups, "IsUpdatingReportGroups not set to true");
                Assert.IsTrue(viewModel.IsSubmittingContext, "IsSubmitting context not set to true");
            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void AddChildFolder_ShouldAdjustNameIfAlreadyExistsInTheParent()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder ReportFolderToAdd = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "AddChildToThis", FullPath = "AddChildToThis/" };
            ReportGroupFolder ReportFolderToAdd1 = new ReportGroupFolder { Code = Guid.NewGuid(), Name = "New Folder", FullPath = "AddChildToThis/New Folder/", Parent = ReportFolderToAdd, };
            ReportFolderToAdd.Children = new ObservableCollection<ReportGroupFolder>() { ReportFolderToAdd1 };
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            bool submitExecuted = false;

            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.NewFolderCommand.Execute(ReportFolderToAdd);

            //No conditional as we are testing before the callback
            EnqueueCallback(() =>
            {
                var x = (from y in context.ReportGroups
                         where y.Name == "New Folder(1)"
                         select y);
                Assert.IsTrue(x.Count() == 1, "Child Folder not renamed to avoid existing folder"); 
            });
            EnqueueTestComplete();
        }
        #endregion

        #region Can Delete Folder Tests
        [TestMethod]
        public void CanDeleteFolder_ShouldBeFalseIfAccessLevelLessThanDelete()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(),Children=new ObservableCollection<ReportGroupFolder>(), };
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code,AccessLevel = ReportGroupAccessLevelType.ReadOnly };
            viewModel.IsRetreivingReports = false;
            context.ReportGroups.Attach(AssociatedReportGroup);
            
            Assert.IsFalse(viewModel.DeleteFolderCommand.CanExecute(FolderToDelete),"Can delete when access level is less than delete");

        }
        [TestMethod]
        public void CanDeleteFolder_ShouldBeFalseIfFolderHasChildFolders()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>() };
            ReportGroupFolder ChildFolder = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>() };
            FolderToDelete.Children.Add(ChildFolder);
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Delete };
            context.ReportGroups.Attach(AssociatedReportGroup);
            viewModel.IsRetreivingReports = false;
            
            Assert.IsFalse(viewModel.DeleteFolderCommand.CanExecute(FolderToDelete), "Can delete when child folders exist");
        }
        [TestMethod]
        public void CanDeleteFolder_ShouldBeFalseIfFolderContainsReports()
        {//Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>() };
            
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Delete };
            context.ReportGroups.Attach(AssociatedReportGroup);
            Report AssociatedReport = new Report { Code = Guid.NewGuid(), ReportGroup = AssociatedReportGroup };
            context.Reports.Attach(AssociatedReport);
            viewModel.IsRetreivingReports = false;
            Assert.IsFalse(viewModel.DeleteFolderCommand.CanExecute(FolderToDelete), "Can delete when reports exist");
      
        }
        [TestMethod]
        public void CanDeleteFolder_ShouldBeTrueIfAllOK()
        {
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>() };

            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Delete };
            context.ReportGroups.Attach(AssociatedReportGroup);
            viewModel.IsRetreivingReports = false;
            
            Assert.IsTrue(viewModel.DeleteFolderCommand.CanExecute(FolderToDelete), "Cant delete when OK");
        }

        #endregion

        #region DeleteFolder Tests
        [TestMethod]
        [Asynchronous]
        public void DeleteFolder_ShouldDeleteFromTheContext()
        { //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(), };
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Delete };
            
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            viewModel.RootGroups = new ObservableCollection<ReportGroupFolder>() { FolderToDelete };
            bool submitExecuted = false;
            context.ReportGroups.Attach(AssociatedReportGroup);
            viewModel.IsRetreivingReports = false;
            
            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.DeleteFolderCommand.Execute(FolderToDelete);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(context.ReportGroups.Count == 0,"Folder not removed from context");
                Assert.IsTrue(submitExecuted, "Submit not executed");
                mockDomainClient.Verify(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)));
            });
            EnqueueTestComplete();
        }
        
        [TestMethod]
        [Asynchronous]
        public void DeleteFolder_ShouldDeleteFromTheViewModelFolderStructure()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            var mockDomainClient2 = new Mock<FakeDomainClient>();
            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(), };
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Delete };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            viewModel.RootGroups = new ObservableCollection<ReportGroupFolder>() { FolderToDelete };
            bool submitExecuted = false;
            context.ReportGroups.Attach(AssociatedReportGroup);
            viewModel.IsRetreivingReports = false;
            
            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.DeleteFolderCommand.Execute(FolderToDelete);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.RootGroups.Count == 0, "Folder not removed from view model structure");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void DeleteFolder_ShouldInitiallySetTheSubmittingFlagsToTrue()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(), };
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Delete };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            viewModel.RootGroups = new ObservableCollection<ReportGroupFolder>() { FolderToDelete };
            bool submitExecuted = false;
            context.ReportGroups.Attach(AssociatedReportGroup);
            viewModel.IsRetreivingReports = false;
            
            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.DeleteFolderCommand.Execute(FolderToDelete);

            
            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.IsUpdatingReportGroups, "is updating report groups not set");
                Assert.IsTrue(viewModel.IsSubmittingContext, "is updating context not set");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void DeleteFolder_ShouldFinallySetTheSubmittingFlagsToFalse()
        { //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportGroupFolder FolderToDelete = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(), };
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToDelete.Code, AccessLevel = ReportGroupAccessLevelType.Delete };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            viewModel.RootGroups = new ObservableCollection<ReportGroupFolder>() { FolderToDelete };
            bool submitExecuted = false;
            context.ReportGroups.Attach(AssociatedReportGroup);
            viewModel.IsRetreivingReports = false;
            
            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.RemovedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.DeleteFolderCommand.Execute(FolderToDelete);
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);


            EnqueueCallback(() =>
            {
                Assert.IsFalse(viewModel.IsUpdatingReportGroups, "is updating report groups not set to false at end");
                Assert.IsFalse(viewModel.IsSubmittingContext, "is updating context not set to false at end");
            });
            EnqueueTestComplete();
        }

        #endregion

        #region Can rename folder tests
        [TestMethod]
        public void CanRenameFolder_ShouldBeFalseIfAccessLevelLessThanUpdate()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToRename = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(), AccessLevel = ReportGroupAccessLevelType.ReadOnly };
            
            Assert.IsFalse(viewModel.RenameFolderCommand.CanExecute(FolderToRename),"Can rename when access level is less than update");
        }

        [TestMethod]
        public void CanRenameFolder_ShouldBeTrueIfAccessLevelUpdate()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToRename = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(),AccessLevel=ReportGroupAccessLevelType.Update };

            Assert.IsTrue(viewModel.RenameFolderCommand.CanExecute(FolderToRename), "Cannot rename when access level is update");
        }
        #endregion

        #region RenameFolder Tests
        [TestMethod]
        public void RenameFolder_ShouldBeChangeTheIsRenamingFlag()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            
            ReportGroupFolder FolderToRename = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(),AccessLevel=ReportGroupAccessLevelType.Update };
            FolderToRename.IsRenaming = false;
            viewModel.RenameFolderCommand.Execute(FolderToRename);
            Assert.IsTrue(FolderToRename.IsRenaming, "Renaming flag not set");
        }
        #endregion

        #region CanRenamingCompleted Tests
        [TestMethod]
        public void CanRenamingCompleted_ShouldBeFalseIfAccessLevelLessThanUpdate()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToRename = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(), AccessLevel = ReportGroupAccessLevelType.ReadOnly };

            Assert.IsFalse(viewModel.RenamingCompleteCommand.CanExecute(FolderToRename), "Can rename completed when access level is less than update");
        }

        [TestMethod]
        public void CanRenamingCompleted_ShouldBeTrueIfAccessLevelUpdate()
        {
            //Fake out the domain client in the context so that no calls to the service actually occur occur
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);

            ReportGroupFolder FolderToRename = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(), AccessLevel = ReportGroupAccessLevelType.Update };

            Assert.IsTrue(viewModel.RenamingCompleteCommand.CanExecute(FolderToRename), "Cannot rename completed when access level is update");
        }
        #endregion

        #region RenamingCompleted Tests
        [TestMethod]
        [Asynchronous]
        public void RenamingCompleted_ShouldSubmitTheChanges()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportGroupFolder FolderToRename = new ReportGroupFolder() { Code = Guid.NewGuid(), Children = new ObservableCollection<ReportGroupFolder>(),Name="Name 1",FullPath="Name 1/" };
            ReportGroup AssociatedReportGroup = new ReportGroup() { Code = FolderToRename.Code, AccessLevel = ReportGroupAccessLevelType.Delete,Name="Name 2",PathName="Name 2/",ParentPath="",ParentCode=null };

            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            ReportListViewModel viewModel = new ReportListViewModel(context, new Mock<IMessageBoxService>().Object, new Mock<IModalDialogService>().Object);
            viewModel.RootGroups = new ObservableCollection<ReportGroupFolder>() { FolderToRename };
            bool submitExecuted = false;
            context.ReportGroups.Attach(AssociatedReportGroup);
            viewModel.IsRetreivingReports = false;
            viewModel.RootGroups = new ObservableCollection<ReportGroupFolder>() { FolderToRename };
            //EntityChangeSet resultSet = context.EntityContainer.GetChanges();
            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.ModifiedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(()
                    => context.EntityContainer.GetChanges().GetChangeSetEntries());

            //Act
            viewModel.RenamingCompleteCommand.Execute(FolderToRename);

            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsSubmitting == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(submitExecuted, "Submit not executed");
                mockDomainClient.Verify(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.ModifiedEntities.Count == 1)));
                var x = (from y in context.ReportGroups
                         where y.Code == FolderToRename.Code
                         select y).Single();
                Assert.IsTrue(x.Name == "Name 1", "Name Not changed");
                Assert.IsTrue(x.PathName == "Name 1/", "Name Not changed");

            });
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void RenamingCompleted_ShouldAdjustTheNameIfAlreadyExists()
        {
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void RenamingCompleted_ShouldChangePathForAllChildFolders()
        {
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void RenamingCompleted_ShouldInitiallySetTheFlags()
        {
            EnqueueTestComplete();
        }
        [TestMethod]
        [Asynchronous]
        public void RenamingCompleted_ShouldFinallyUnsetTheFlags()
        {
            EnqueueTestComplete();
        }

        #endregion
        
        #region Can Change Permisisons Tests
        #endregion

        #region Change Permission Tests
        #endregion



    }
}
