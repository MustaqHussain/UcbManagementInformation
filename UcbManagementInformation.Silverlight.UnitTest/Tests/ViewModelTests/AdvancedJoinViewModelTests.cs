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
using System.Collections.Generic;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.Silverlight.UnitTest.Helpers;

namespace UcbManagementInformation.Silverlight.UnitTest.Tests.ViewModelTests
{
    [TestClass]
    public class AdvancedJoinViewModelTests
    {
        [TestMethod]
        public void Constructor_ShouldSetTheJoinList()
        {
            List<DataTableJoin> JoinList = new List<DataTableJoin>() { new DataTableJoin() { JoinType = "INNER" }, new DataTableJoin() { JoinType = "OUTER" } };
            AdvancedJoinViewModel ViewModel = new AdvancedJoinViewModel(JoinList);
            CollectionAssert.AreEqual(JoinList, ViewModel.JoinList);
        }

        [TestMethod]
        public void Constructor_ShouldSetTheJoinTypes()
        {
            List<DataTableJoin> JoinList = new List<DataTableJoin>(){};
            AdvancedJoinViewModel ViewModel = new AdvancedJoinViewModel(JoinList);
            Assert.IsTrue(ViewModel.JoinTypes.Contains("INNER"));
            Assert.IsTrue(ViewModel.JoinTypes.Contains("FULL"));
            Assert.IsTrue(ViewModel.JoinTypes.Contains("LEFT"));
            Assert.IsTrue(ViewModel.JoinTypes.Contains("RIGHT"));
        }
        [TestMethod]
        public void JoinList_ShouldRaisePropertyChanged()
        {
            List<DataTableJoin> JoinList = new List<DataTableJoin>() { };
            AdvancedJoinViewModel ViewModel = new AdvancedJoinViewModel(JoinList);
            ViewModel.AssertRaisesPropertyChangedFor("JoinList");
        }
        [TestMethod]
        public void SelectedJoin_ShouldRaisePropertyChanged()
        {
            List<DataTableJoin> JoinList = new List<DataTableJoin>() { };
            AdvancedJoinViewModel ViewModel = new AdvancedJoinViewModel(JoinList);
            ViewModel.AssertRaisesPropertyChangedFor("SelectedJoin",new DataTableJoin());
                
          
        }

        [TestMethod]
        public void JoinTypes_ShouldRaisePropertyChanged()
        {
            List<DataTableJoin> JoinList = new List<DataTableJoin>() { };
            AdvancedJoinViewModel ViewModel = new AdvancedJoinViewModel(JoinList);
            ViewModel.AssertRaisesPropertyChangedFor("JoinTypes");
        }
        
    }
}