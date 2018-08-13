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
namespace UcbManagementInformation.Silverlight.UnitTest.Tests.ViewModelTests
{
    [TestClass]
    public class EditDataItemViewModelTests : SilverlightTest
    {

        [TestInitialize]
        public void TestSetUp()
        {
            //No Test Setup in this test
        }

        [TestMethod]
        [Asynchronous]
        public void Constructor_ShouldAttemptToLoadADataItem()
        {
            //Arrange
            bool loadExecuted = false;

            //I expect that within the constructor, my mock domain client will have its Query method called. 
            //I expect that it will be called with an EntityQuery for DataItem objects. When it gets called, 
            //I want to set a flag (load executed)to verify that it was called. 

            var mockDomainClient = new Mock<FakeDomainClient>();
            
            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {},
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);
            //Assert
            Assert.IsTrue(loadExecuted,"The load was not called"); //Test if the method was executed
          
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_DataItemToEditGetsPopulatedWithTheDataItem()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert
            //Assert.IsTrue(loadExecuted); //Test if the method was executed

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            EnqueueCallback(() => // Did it populate the view model with the data item     
            {
                Assert.AreSame(context.DataItems.ElementAt(0), 
                    viewModel.DataItemToEdit,
                    "The DataItemToEdit in the view model was not pouplated with the data item retrieved from the load");
                //Assert.AreEqual("ProgrammeBalance", viewModel.DataItemToEdit.Name);  

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_LoadsASingleDataItem()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            

            //Assert
            //Assert.IsTrue(loadExecuted); //Test if the method was executed

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);
             

            EnqueueCallback(() => // Did the load return 1 data item  
            {
                Assert.IsTrue(context.DataItems.Count() == 1,
                    "The load did not return 1 data item");
                //Assert.AreEqual("ProgrammeBalance", viewModel.DataItemToEdit.Name);  

            });
            EnqueueTestComplete();
        }

        
        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_IfAllFieldsPopulatedThenTheSaveCommandIsExecutable()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            EnqueueCallback(() => //    
            {
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == true,
                    "SaveCommand should be executable if all the fields are poulated");

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_IfCaptionNotPopulatedThenTheSaveCommandIsNotExecutable()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="",
                        Description="The current programme balance",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert
            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            EnqueueCallback(() =>   
            {             
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == false,
                    "Save Command should not be executable if the caption in empty");  
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_IfDescriptionNotPopulatedThenTheSaveCommandIsNotExecutable()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert
            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == false,
                    "Save Command should not be executable if the description in empty");

            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_IfLocationOnSystemNotPopulatedTheSaveCommandIsNotExecutable()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();
            bool loadExecuted = false;

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem=""}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert
            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == false,
                    "Save Command should not be executable if the location on system in empty");
            });
            EnqueueTestComplete();
        }

        
        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_SaveCommandShouldBeARelayCommand()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))      
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {}
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            ////Assert
            EnqueueConditional(() =>  //Wait until SaveCommand has been initialised
                viewModel.SaveCommand != null);

            EnqueueCallback(() => //test that the save command is a relay command    
            {
                Assert.IsInstanceOfType(viewModel.SaveCommand,
                    typeof(RelayCommand), "Save Command is not a RelayCommand");
            });
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LoadDataItemCallBack_CancelCommandShouldBeARelayCommand()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {}
    
                }.AsQueryable());
            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert
            EnqueueConditional(() =>  //Wait until CancelCommand has been initialised
                viewModel.CancelCommand != null);

            EnqueueCallback(() => //test that the cancel command is a relay command    
            {
                Assert.IsInstanceOfType(viewModel.CancelCommand,
                    typeof(RelayCommand), "Cancel Command is not a RelayCommand");
            });
            EnqueueTestComplete();

        }

        [TestMethod]
        [Asynchronous]
        public void Caption_ShouldRaisePropertyChangedEventWhenAltered()
        {
            //Arrange
            bool wasCalled = false;
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="Location On System"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert
            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            EnqueueCallback(() => //test that amending the caption triggers the property changed event   
            {
                viewModel.DataItemToEdit.PropertyChanged += (s, e) => { wasCalled = true; };
                viewModel.DataItemToEdit.Caption += " Test"; //change the caption
                Assert.IsTrue(wasCalled, "PropertyChanged event not fired when the caption was changed");
            });
           
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void Description_ShouldRaisePropertyChangedEventWhenAltered()
        {
            //Arrange
            bool wasCalled = false;
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="Location On System"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            //Assert
            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            EnqueueCallback(() => //test that amending the caption triggers the property changed event   
            {
                viewModel.DataItemToEdit.PropertyChanged += (s, e) => { wasCalled = true; };
                viewModel.DataItemToEdit.Description += " Test"; //change the description
                Assert.IsTrue(wasCalled,"PropertyChanged event not fired when the description was changed");
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void LocationOnSystem_ShouldRaisePropertyChangedEventWhenAltered()
        {
            //Arrange
            bool wasCalled = false;
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="Location On System"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            //Assert
            EnqueueCallback(() => //test that amending the location on system triggers the property changed event   
            {
                viewModel.DataItemToEdit.PropertyChanged += (s, e) => { wasCalled = true; };
                viewModel.DataItemToEdit.LocationOnSystem += " Test"; //change the location on system
                Assert.IsTrue(wasCalled,"PropertyChanged event not fired when the location on system was changed");
            });

            EnqueueTestComplete();
        }


        [TestMethod]
        [Asynchronous]
        public void SaveCommandCanExecute_ShouldBeFalseIfCaptionIsCleared()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="Location On System"}
    
                }.AsQueryable());

            
            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            //Assert
            EnqueueCallback(() => //test that clearing the caption sets SaveCommand.CanExecute to false   
            {
                viewModel.DataItemToEdit.Caption = ""; //change the caption to be empty
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == false,
                    "Save Command should not be executable if the caption is cleared");  
                });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void SaveCommandCanExecute_ShouldBeFalseIfDescriptionIsCleared()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="Location On System"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            //Assert
            EnqueueCallback(() => //test that clearing the description sets SaveCommand.CanExecute to false   
            {
                viewModel.DataItemToEdit.Description = ""; //change the description to be empty
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == false,
                    "Save Command should not be executable if the description is cleared");
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void SaveCommandCanExecute_ShouldBeFalseIfLocationOnSystemIsCleared()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="Location On System"}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            //Assert
            EnqueueCallback(() => //test that clearing the description sets SaveCommand.CanExecute to false   
            {
                viewModel.DataItemToEdit.LocationOnSystem = ""; //change the caption to be empty
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == false,
                    "Save Command should not be executable if the location on sytem is cleared");
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void SaveCommandCanExecute_ShouldBeTrueIfAllFieldsAreAmendedAndPopulated()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="",
                        Description="",
                        LocationOnSystem=""}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            //Assert
            EnqueueCallback(() => //test that populating the fields  sets SaveCommand.CanExecute to true  
            {
                viewModel.DataItemToEdit.Caption = "Programme Balance";
                viewModel.DataItemToEdit.Description = "The name of the programme";
                viewModel.DataItemToEdit.LocationOnSystem = "Location On System";
                Assert.IsTrue(viewModel.SaveCommand.CanExecute(null) == true,
                    "Save Command should  be executable if all the fields are populated");
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void CancelCommand_ShouldRaisePropertyChangeWhenAltered()
        {

            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            EditDataItemViewModel viewModel = null;

            var firedEvents = new List<string>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                     new DataItem {
                        Name="ProgrammeBalance",
                        Caption="",
                        Description="",
                        LocationOnSystem=""}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            viewModel.PropertyChanged += ((sender, e) => firedEvents.Add(e.PropertyName));

            //Assert
            EnqueueConditional(() =>  //Wait until CancelCommand has been initialised
                viewModel.CancelCommand != null);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(firedEvents.Contains("CancelCommand"), "Cancel Command did not fire OnPropertyChanged");
            });

            //Assert
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void SaveCommand_ShouldRaisePropertyChangeWhenAltered()
        {

            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            EditDataItemViewModel viewModel = null;

            var firedEvents = new List<string>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                     new DataItem {
                        Name="ProgrammeBalance",
                        Caption="",
                        Description="",
                        LocationOnSystem=""}
    
                }.AsQueryable());
            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            viewModel.PropertyChanged += ((sender, e) => firedEvents.Add(e.PropertyName));

            //Assert
            EnqueueConditional(() =>  //Wait until SaveCommand has been initialised
                viewModel.SaveCommand != null);

            EnqueueCallback(() =>
            {
                Assert.IsTrue(firedEvents.Contains("SaveCommand"), "Save Command did not fire OnPropertyChanged");
            });

            //Assert
            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void SaveCommandExecute_ShouldCallSubmitChanges()
        {
            //Arrange

            bool submitExecuted = false;
            bool loadExecuted = false;
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());
           
            
            EntityChangeSet resultSet = context.EntityContainer.GetChanges();

            //resultSet.GetChangeSetEntries


            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.ModifiedEntities.Count == 1)))
                //mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.AddedEntities.Count == 0)))
                .Callback(() => submitExecuted = true)
                .Returns(() => new List<ChangeSetEntry>());
                //.Returns(resultSet.GetChangeSetEntries());

            //Act
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);
           

            EnqueueConditional(() =>  //Wait until the load has finished
                context.IsLoading==false);

            EnqueueCallback(() => //Amend the description of the data item and submit the changes to the database
            {
                context.DataItems.FirstOrDefault(x => x.Name == "ProgrammeBalance").Description = "Amended Description";
                viewModel.SaveCommand.Execute(null);
            });
            

            EnqueueConditional(() =>  //Wait until the submit has finished
                context.IsSubmitting == false);

            //Assert
            EnqueueCallback(() => //test that the method was executed
            {
                Assert.IsTrue(submitExecuted, "The submit was not called"); //Test if the method was executed
                //mockDomainClient.Verify(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.ModifiedEntities.Count == 1))); 
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void SaveCommandExecute_ShouldCallSubmitChangesWithASingleModifiedEntity()
        {
            //Arrange
            bool submitExecuted = false;
            bool loadExecuted = false;
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());
           
            
            EntityChangeSet resultSet = context.EntityContainer.GetChanges();

            //resultSet.GetChangeSetEntries


            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.ModifiedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(() => new List<ChangeSetEntry>());
                //.Returns(resultSet.GetChangeSetEntries());

            //Act
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);
           

            EnqueueConditional(() =>  //Wait until the load has finished
                context.IsLoading==false);

            EnqueueCallback(() => //Amend the description of the data item and submit the changes to the database
            {
                context.DataItems.FirstOrDefault(x => x.Name == "ProgrammeBalance").Description = "Amended Description";
                viewModel.SaveCommand.Execute(null);
            });
            
            EnqueueConditional(() =>  //Wait until the submit has finished
                context.IsSubmitting == false);

            //Assert
            EnqueueCallback(() => //test that the method was executed
            {
                //Assert.IsTrue(submitExecuted, "The submit was not called"); //Test if the method was executed
                mockDomainClient.Verify(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.ModifiedEntities.Count == 1))); 
            });

            EnqueueTestComplete();
        }
 
        [TestMethod]
        [Asynchronous]
        public void CancelCommandExecute_ShouldSetTheDialogResultToFalse()
        {
            //Arrange
            var mockDomainClient = new Mock<FakeDomainClient>();

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {}
    
                }.AsQueryable());

            //Act
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);

            EnqueueConditional(() =>  //Wait until DataItemToEditIsPopulated
                viewModel.DataItemToEdit != null);

            //Assert
            EnqueueCallback(() => //test that executing the save command sets the dialog result to true  
            {
                viewModel.CancelCommand.Execute(null);
                Assert.IsTrue(viewModel.DialogResult.Value==false, "CancelCommand has not set the dialog result to false");
            });

            EnqueueTestComplete();
        }

        [TestMethod]
        [Asynchronous]
        public void SaveCommandExecute_ShouldSetTheDialogResultToTrue()
        {
            //Arrange
            bool submitExecuted = false;
            bool loadExecuted = false;
            var mockDomainClient = new Mock<FakeDomainClient>();
            ReportWizardContext context = new ReportWizardContext(mockDomainClient.Object);

            mockDomainClient.Setup(dc => dc.Query(It.Is<EntityQuery>(query => query.EntityType == typeof(DataItem))))
                .Callback(() => loadExecuted = true) // used to verify it was called         
                .Returns(() => new Entity[] // a data item       
                {              
                    new DataItem {
                        Name="ProgrammeBalance",
                        Caption="Programme Balance",
                        Description="The current programme balance",
                        LocationOnSystem="The location on the system"}
    
                }.AsQueryable());


            EntityChangeSet resultSet = context.EntityContainer.GetChanges();

            mockDomainClient.Setup(dc => dc.Submit(It.Is<EntityChangeSet>(ecs => ecs.ModifiedEntities.Count == 1)))
                .Callback(() => submitExecuted = true)
                .Returns(() => new List<ChangeSetEntry>());
                //.Returns(resultSet.GetChangeSetEntries());

            //Act
            EditDataItemViewModel viewModel = new EditDataItemViewModel(It.IsAny<Guid>(), context);


            EnqueueConditional(() =>  //Wait until the load has finished
                context.IsLoading == false);

            EnqueueCallback(() => //Amend the description of the data item and submit the changes to the database
            {
                context.DataItems.FirstOrDefault(x => x.Name == "ProgrammeBalance").Description = "Amended Description";
                viewModel.SaveCommand.Execute(null);
            });

            EnqueueConditional(() =>  //Wait until the submit has finished
                context.IsSubmitting == false);

            //Assert
            EnqueueCallback(() => 
            {
                Assert.IsTrue(viewModel.DialogResult.Value == true, "SaveCommand has not set the dialog result to true");
            });

            EnqueueTestComplete();
        }

       
    }
}