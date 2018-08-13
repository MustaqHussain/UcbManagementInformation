using UcbManagementInformation.Server.RDL2003Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using UcbManagementInformation.Server.DataAccess;
using System.Collections.Generic;
using System.Xml;
using Moq;
using System.Text;
using System.Xml.Linq;
using UcbManagementInformation.Server.UnitTest.Helpers;
using UcbManagementInformation.Server.IoC.ServiceLocation;
namespace UcbManagementInformation.Server.UnitTest
{

    #region Constructor tests
    /// <summary>
    ///This is a test class for TableHeaderTest and is intended
    ///to contain all TableHeaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TableHeaderTests
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Render2010
        ///</summary>
        //[TestMethod()]
        //public void Render2010Test()
        //{
        //    List<DataItem> fieldList = null; // TODO: Initialize to an appropriate value
        //    TableHeader target = new TableHeader(fieldList); // TODO: Initialize to an appropriate value
        //    XmlWriter xmlWriter = null; // TODO: Initialize to an appropriate value
        //    target.Render2010(xmlWriter);
        //    Assert.Inconclusive("A method that does not return a value cannot be verified.");
        //}

        /// <summary>
        ///A test for TableHeader Constructor
        ///</summary>
        [TestMethod()]
        public void Constructor_ShouldCreateATextboxForEachField()
        {
            var ServiceLocatorMock = new Mock<ISimpleServiceLocator>();
            SimpleServiceLocator.SetServiceLocatorProvider(ServiceLocatorMock.Object);

            var headerTextBoxMock = new Mock<ITableHeaderTextBox>();
            headerTextBoxMock.Setup(x => x.TextboxStyle.TextAlign).Returns("Right");

            var headerTextBoxMockLeft = new Mock<ITableHeaderTextBox>();
            headerTextBoxMockLeft.Setup(x => x.TextboxStyle.TextAlign).Returns("");

            ServiceLocatorMock.Setup(x => x.Get<ITableHeaderTextBox>
                ("TableHeaderTextBoxRight", It.IsAny<Dictionary<string, object>>())).Returns(headerTextBoxMock.Object);
            ServiceLocatorMock.Setup(x => x.Get<ITableHeaderTextBox>
                ("TableHeaderTextBox", It.IsAny<Dictionary<string, object>>())).Returns(headerTextBoxMockLeft.Object);


            List<DataItem> fieldList = new List<DataItem> 
            { 
                new DataItem { Name="Item1",Caption="Cap1",DataType="char" },
                new DataItem { Name="Item2",Caption="Cap2",DataType="int" }
            };

            var mockRepository = new Moq.Mock<IDataTableRepository>();

            mockRepository.Setup(x => x.GetByCode(It.IsAny<Guid>())).Returns(new DataTable { Name = "TestTable" });

            //method under test
            TableHeader target = new TableHeader(fieldList, mockRepository.Object,new DataModel());

            ServiceLocatorMock.Verify(x => x.Get<ITableHeaderTextBox>
                ("TableHeaderTextBox", It.IsAny<Dictionary<string, object>>()), Times.Once());
            ServiceLocatorMock.Verify(x => x.Get<ITableHeaderTextBox>
                ("TableHeaderTextBoxRight", It.IsAny<Dictionary<string, object>>()), Times.Once());

            Assert.IsTrue(target.TableHeaderTextBoxList.Count == 2);

        }

        [TestMethod()]
        public void Constructor_ShouldCallTextboxConstructorWithNameEqualTablenamePlusFieldName()
        {
            //mock the service locator
            var ServiceLocatorMock = new Mock<ISimpleServiceLocator>();
            SimpleServiceLocator.SetServiceLocatorProvider(ServiceLocatorMock.Object);
           

            //var headerTextBoxMockLeft = new Mock<ITableHeaderTextBox>();
            //headerTextBoxMockLeft.Setup(x => x.TextboxStyle.TextAlign).Returns("");

            //ServiceLocatorMock.Setup(x => x.Get<ITableHeaderTextBox>
            //    ("TableHeaderTextBox", It.IsAny<Dictionary<string, object>>())).Returns(headerTextBoxMockLeft.Object);

            List<DataItem> fieldList = new List<DataItem> 
            {
                //char datatype selects left aligned
                new DataItem { Name="Item1",Caption="Cap1",DataType="char" },
            };

            var mockRepository = new Moq.Mock<IDataTableRepository>();

            mockRepository.Setup(x => x.GetByCode(It.IsAny<Guid>())).Returns(new DataTable { Name = "TestTable" });

            TableHeader target = new TableHeader(fieldList, mockRepository.Object, new DataModel());
            ServiceLocatorMock.Verify(x => x.Get<ITableHeaderTextBox>
                ("TableHeaderTextBox", It.Is<Dictionary<string,object>>(y => y["name"] == "TestTableCap1" && y["caption"] == "Cap1")), Times.Once());
            
            
        }

        [TestMethod()]
        public void CreateATextboxRightAlingedForNumericDataTypes()
        {
            List<DataItem> fieldList = new List<DataItem> 
            { 
                new DataItem { Name="Item1",Caption="Cap1",DataType="decimal" },
                new DataItem { Name="Item2",Caption="Cap1",DataType="money" },
                new DataItem { Name="Item5",Caption="Cap2",DataType="int" }
            };

            var mockRepository = new Moq.Mock<IDataTableRepository>();

            mockRepository.Setup(x => x.GetByCode(It.IsAny<Guid>())).Returns(new DataTable { Name = "TestTable" });

            TableHeader target = new TableHeader(fieldList, mockRepository.Object, new DataModel());

            Assert.IsTrue(target.TableHeaderTextBoxList[0].TextboxStyle.TextAlign == "Right");
            Assert.IsTrue(target.TableHeaderTextBoxList[1].TextboxStyle.TextAlign == "Right");
            Assert.IsTrue(target.TableHeaderTextBoxList[2].TextboxStyle.TextAlign == "Right");

            
        }

        [TestMethod()]
        public void CreateATextboxLeftAlingedForNonNumericDataTypes()
        {
            List<DataItem> fieldList = new List<DataItem> 
            { 
                new DataItem { Name="Item1",Caption="Cap1",DataType="char" },
                new DataItem { Name="Item2",Caption="Cap2",DataType="varchar" }
            };

            var mockRepository = new Moq.Mock<IDataTableRepository>();

            mockRepository.Setup(x => x.GetByCode(It.IsAny<Guid>())).Returns(new DataTable { Name = "TestTable" });

            TableHeader target = new TableHeader(fieldList, mockRepository.Object, new DataModel());

            Assert.IsTrue(target.TableHeaderTextBoxList[0].TextboxStyle.TextAlign != "Right");
            Assert.IsTrue(target.TableHeaderTextBoxList[1].TextboxStyle.TextAlign != "Right");

        }
    }
    #endregion


    #region Render tests
    /// <summary>
    ///This is a test class for TableHeaderTest and is intended
    ///to contain all TableHeaderTest Unit Tests
    ///</summary>
    [TestClass()]
    public class TableHeaderRenderShould
    {


        private TestContext testContextInstance;

        /// <summary>
        ///Gets or sets the test context which provides
        ///information about and functionality for the current test run.
        ///</summary>
        public TestContext TestContext
        {
            get
            {
                return testContextInstance;
            }
            set
            {
                testContextInstance = value;
            }
        }

        #region Additional test attributes
        // 
        //You can use the following additional attributes as you write your tests:
        //
        //Use ClassInitialize to run code before running the first test in the class
        //[ClassInitialize()]
        //public static void MyClassInitialize(TestContext testContext)
        //{
        //}
        //
        //Use ClassCleanup to run code after all tests in a class have run
        //[ClassCleanup()]
        //public static void MyClassCleanup()
        //{
        //}
        //
        //Use TestInitialize to run code before running each test
        //[TestInitialize()]
        //public void MyTestInitialize()
        //{
        //}
        //
        //Use TestCleanup to run code after each test has run
        //[TestCleanup()]
        //public void MyTestCleanup()
        //{
        //}
        //
        #endregion


        /// <summary>
        ///A test for Render2010
        ///</summary>
        [TestMethod()]
        public void CallTheRenderOfTextBoxOncePerTextBox()
        {
            //Fake list of data items
            List<DataItem> fieldList = new List<DataItem> 
            { 
                new DataItem { Name="Item1",Caption="Cap1",DataType="char" },
                new DataItem { Name="Item2",Caption="Cap2",DataType="int" }
            };

            //Mock the repository for dependency injection
            var mockRepository = new Moq.Mock<IDataTableRepository>();
            mockRepository.Setup(x => x.GetByCode(It.IsAny<Guid>())).Returns(new DataTable { Name = "TestTable" });

            TableHeader target = new TableHeader(fieldList, mockRepository.Object,new DataModel());
            StringBuilder sb = new StringBuilder();
            
            //create an xmlwriter
            using (XmlWriter xmlWriter = XmlWriter.Create(sb)) 
            {
                //mock the textbox list of the header so that verification of calls to render2010 can be tested
                var mockTextBox1 = new Mock<ITableHeaderTextBox>();
                var mockTextBox2 = new Mock<ITableHeaderTextBox>();
                List<ITableHeaderTextBox> textboxList = new List<ITableHeaderTextBox>{mockTextBox1.Object,mockTextBox2.Object};
                target.TableHeaderTextBoxList = textboxList;

                //method under test
                target.Render2010(xmlWriter);

                //verify render 2010 was called on the textboxes.
                mockTextBox1.Verify(x => x.Render2010(xmlWriter), Times.Once());
                mockTextBox2.Verify(x => x.Render2010(xmlWriter), Times.Once());
        
            }
         }

        /// <summary>
        ///A test for Render2010
        ///</summary>
        [TestMethod()]
        public void OutputTablixRowElement()
        {
            //Mock repository
            var mockRepository = new Moq.Mock<IDataTableRepository>();
            
            //construct the tableheader object
            TableHeader target = new TableHeader(new List<DataItem>(), mockRepository.Object, new DataModel());
            
            //Create stringbuilder for the xmlwriter
            StringBuilder sb = new StringBuilder();
            using (XmlWriter xmlWriter = XmlWriter.Create(sb))
            {
                //method under test
                target.Render2010(xmlWriter);
                
                //flush the output and grab the xml chunk
                xmlWriter.Flush();
                string outputRdl = sb.ToString();

                //compare output with expected xml
                XmlAssert.AreEqual(outputRdl, "<TablixRow><Height>.25in</Height><TablixCells /></TablixRow>");
            }
        }
    }
    #endregion
}
