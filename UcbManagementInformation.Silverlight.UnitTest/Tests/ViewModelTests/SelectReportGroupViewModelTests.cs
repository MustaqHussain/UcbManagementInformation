using System;
using System.Linq;
using System.Net;
using System.ServiceModel.DomainServices.Client;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Silverlight.UnitTest.Helpers;
using UcbManagementInformation.ViewModels;
using UcbManagementInformation.Web.Server;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UcbManagementInformation.Commanding;
using System.Collections.Generic;
using UcbManagementInformation.Web.Models;
using System.Collections.ObjectModel;
using UcbManagementInformation.Models;
namespace UcbManagementInformation.Silverlight.UnitTest.Tests.ViewModelTests
{
    [TestClass]
    public class SelectReportGroupViewModelTests : SilverlightTest
    {

        [TestInitialize]
        public void TestSetUp()
        {
            //No Test Setup in this test
        }

       
        [TestMethod]
        [Asynchronous]
        public void Constructor_OKCommandShouldBeARelayCommand()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Returns(() => new Entity[]      
                {              
                    new ReportGroup {}
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until OkCommand has been initialised
                viewModel.OKCommand != null);

            EnqueueCallback(() => //test that the save command is a relay command    
            {
                Assert.IsInstanceOfType(viewModel.OKCommand,
                    typeof(RelayCommand), "OKCommand is not a RelayCommand");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void Constructor_CancelCommandShouldBeARelayCommand()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Returns(() => new Entity[]      
                {              
                    new ReportGroup {}
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until OkCommand has been initialised
                viewModel.CancelCommand != null);

            EnqueueCallback(() => //test that the save command is a relay command    
            {
                Assert.IsInstanceOfType(viewModel.CancelCommand,
                    typeof(RelayCommand), "CancelCommand is not a RelayCommand");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void Constructor_RootGroupsShouldBeAnObservableCollectionOfReportGroupFolders()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Returns(() => new Entity[]      
                {              
                    new ReportGroup {}
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
                context.IsLoading != null);

            EnqueueCallback(() =>     
            {
                Assert.IsInstanceOfType(viewModel.RootGroups,
                    typeof(ObservableCollection<ReportGroupFolder>), "RootGroups is not an observablecollection of report group folders");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void Constructor_ShouldAttemptToLoadReportGroupsForUser()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {              
                    new ReportGroup {}
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>  
            {
                Assert.IsTrue(loadExecuted, "The load was not called"); //Test if the method was executed
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void GetReportGroupsForUserCallback_RootGroupsGetsPopulatedWithTheReportGroups()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
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

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.RootGroups.Count ==2, "RootGroups was not populated with the correct number of ReportGroupFolders"); //Test if the method was executed
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void OkCommandCanExecute_ShouldBeFalseIfAGreenFolderHasNotBeenSelected()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                viewModel.ReportName = "TestReportName"; //report name populated
                viewModel.SelectedReportGroup = new ReportGroupFolder //do not have access to this folder
                {
                    AccessLevel = ReportGroupAccessLevelType.None
                };

                Assert.IsTrue(viewModel.OKCommand.CanExecute(null) == false,"OkCommand should be disabled if access is denied to the report folder");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void OkCommandCanExecute_ShouldBeFalseIfNoFolderHasBeenSelected()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                viewModel.ReportName = "TestReportName"; //report name populated
                viewModel.SelectedReportGroup = null; //no folder selected

                Assert.IsTrue(viewModel.OKCommand.CanExecute(null) == false,"OkCommand should be disabled if no folder has been selected");
            });
            EnqueueTestComplete();
        }


        [TestMethod]
        [Asynchronous]
        public void OkCommandCanExecute_ShouldBeFalseIfReportNameHasNotBeenEntered()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());     

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                viewModel.ReportName = ""; //report name left blank
                viewModel.SelectedReportGroup = new ReportGroupFolder //do  have access to this folder
                {
                    AccessLevel = ReportGroupAccessLevelType.Admin
                };

                Assert.IsTrue(viewModel.OKCommand.CanExecute(null) == false, "OkCommand should be disabled if report name has been left blank");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void OkCommandCanExecute_ShouldBeTrueIfGreenFolderSelecetedAndReportNameHasBeenEntered()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                viewModel.ReportName = "PopulatedReportName"; //report name populated
                viewModel.SelectedReportGroup = new ReportGroupFolder //do  have access to this folder
                {
                    AccessLevel = ReportGroupAccessLevelType.Admin
                };

                Assert.IsTrue(viewModel.OKCommand.CanExecute(null) == true, "OkCommand should be enabled if an accessible folder has been selected and the report name has been filled in");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void OKCommand_ShouldRaisePropertyChangeWhenAltered()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {

                viewModel.AssertRaisesPropertyChangedFor("OKCommand");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void CancelCommand_ShouldRaisePropertyChangeWhenAltered()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {
                viewModel.AssertRaisesPropertyChangedFor("CancelCommand");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void RootGroups_ShouldRaisePropertyChangeWhenAltered()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {

                viewModel.AssertRaisesPropertyChangedFor("RootGroups");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void ReportName_ShouldRaisePropertyChangeWhenAltered()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {

                viewModel.AssertRaisesPropertyChangedFor("ReportName");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void DialogResult_ShouldRaisePropertyChangeWhenAltered()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup{}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);

            EnqueueCallback(() =>
            {

                viewModel.AssertRaisesPropertyChangedFor("DialogResult");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void OKCommand_ShouldSetTheDialogResultToTrue()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);


            EnqueueCallback(() =>
            {
                viewModel.OKCommand.Execute(null);
                Assert.IsTrue(viewModel.DialogResult.Value == true, "OKCommand has not set the dialog result to true");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void CancelCommand_ShouldSetTheDialogResultToFalse()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(ReportGroup))))
                .Callback(() => loadExecuted = true)
                .Returns(() => new Entity[]      
                {      
                    new ReportGroup {}
 
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            SelectReportGroupViewModel viewModel = new SelectReportGroupViewModel(context);

            ////Assert
            EnqueueConditional(() =>  //Wait until the load has finished
             context.IsLoading == false);


            EnqueueCallback(() =>
            {
                viewModel.CancelCommand.Execute(null);
                Assert.IsTrue(viewModel.DialogResult.Value == false, "CancelCommand has not set the dialog result to false");
            });
            EnqueueTestComplete();
        }

        
 
    }
}