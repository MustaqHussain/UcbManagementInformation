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
using UcbManagementInformation.ViewModels;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.ObjectModel;
using System.Linq;
using System.Collections.Generic;
using UcbManagementInformation.Silverlight.UnitTest.Helpers;
using Moq;
using UcbManagementInformation.Commanding;

namespace UcbManagementInformation.Silverlight.UnitTest.Tests.ViewModelTests
{
    [TestClass]
    public class SelectReportModelDialogViewModelTests
    {
        private ObservableCollection<DataModel> _modelList;
        private SelectReportModelDialogViewModel _viewModel;

        [TestInitialize]
        public void TestSetUp()
        {
            _modelList = new ObservableCollection<DataModel>
            {
                new DataModel {Name="Managing Authority",Description="A Data Model For managing Authority" },
                 new DataModel {Name="Finance",Description="A Data Model Forfinance" },
                new DataModel {Name="Model 3",Description="A Data Model For managing 3" }
            };
        }
        [TestMethod]
        public void Constructor_ShouldSetTheDataModels()
        {
           
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            //TestHelpers.AreCollectionsItemsEqual<DataModel>(_viewModel.DataModels, _modelList);
            CollectionAssert.AreEquivalent(_viewModel.DataModels, _modelList);
        }
        [TestMethod]
        public void Constructor_ShouldLeaveSelectedDataModelUnset()
        {

            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            Assert.IsTrue(_viewModel.SelectedDataModel == null);
        }
        [TestMethod]
        public void Constructor_ShouldInitialiseTheOKCommandAsNotExecutable()
        {
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            Assert.IsTrue(_viewModel.OKCommand.CanExecute(null) == false);
                    }
        [TestMethod]
        public void Constructor_ShouldInitialiseTheCancelCommandAsExecutable()
        {

            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            Assert.IsTrue(_viewModel.CancelCommand.CanExecute(null) == true);
        }

        [TestMethod]
        public void SelectedDataModel_ShouldRaisePropertyChangedEventWhenAltered()
        {
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            _viewModel.AssertRaisesPropertyChangedFor("SelectedDataModel");
            // Exercise system    
            _viewModel.SelectedDataModel = new DataModel {Name="NewModel" };
        }
        [TestMethod]
        public void OKCommand_ShouldRaisePropertyChangedEventWhenAltered()
        {
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            _viewModel.AssertRaisesPropertyChangedFor("OKCommand");
            // Exercise system    
            _viewModel.OKCommand = new RelayCommand(x=>{});
        }
        [TestMethod]
        public void CancelCommand_ShouldRaisePropertyChangedEventWhenAltered()
        {
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            _viewModel.AssertRaisesPropertyChangedFor("CancelCommand");
            // Exercise system    
            _viewModel.CancelCommand = new RelayCommand(x => { });
        }
        [TestMethod]
        public void DialogResult_ShouldRaisePropertyChangedEventWhenAltered()
        {
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            _viewModel.AssertRaisesPropertyChangedFor("DialogResult");
            // Exercise system    
            _viewModel.DialogResult = false;
        }

        [TestMethod]
        public void OKCommand_ShouldBeTrueAfterDataModelSelected()
        {
            // Exercise system    
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            _viewModel.SelectedDataModel = new DataModel { Name = "NewModel" };
            // Verify outcome 
            Assert.IsTrue(_viewModel.OKCommand.CanExecute(null), "Command Can not be executed");
            // Teardown}

        }
        [TestMethod]
        public void OKCommandExecute_ShouldSetTheDialogResultTrue()
        {
            // Exercise system    
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            _viewModel.SelectedDataModel = new DataModel { Name = "NewModel" };
            // Verify outcome
            _viewModel.OKCommand.Execute(null);
            Assert.IsTrue(_viewModel.DialogResult.Value, "OK Command has not set the dialog result");
            // Teardown}

        }
        [TestMethod]
        public void CancelCommandExecute_ShouldSetTheDialogResultToFalse()
        {
            _viewModel = new SelectReportModelDialogViewModel(_modelList);
            // Exercise system    
            _viewModel.SelectedDataModel = new DataModel { Name = "NewModel" };
            // Verify outcome
            _viewModel.CancelCommand.Execute(null);
            Assert.IsTrue(_viewModel.DialogResult.Value==false, "Cancel Command has not set the dialog result");
            // Teardown}

        }
    }
}