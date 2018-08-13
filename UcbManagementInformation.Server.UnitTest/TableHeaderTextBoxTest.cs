using UcbManagementInformation.Server.RDL2003Engine;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Xml;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using Moq;
using System.Collections.Generic;
using UcbManagementInformation.Server.UnitTest.Helpers;
using System.Text;

namespace UcbManagementInformation.Server.UnitTest
{

    [TestClass()]
    public class TableHeaderTextBoxTests
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


        #region constructor tests

   
       

        /// <summary>
        ///A test for TableHeaderTextBox Constructor
        ///</summary>
        [TestMethod()]
        public void TableHeaderTextBoxConstructor_ShouldSetTheNameAndCaptionProperties()
        {
            var ServiceLocatorMock = new Mock<ISimpleServiceLocator>();
            SimpleServiceLocator.SetServiceLocatorProvider(ServiceLocatorMock.Object);

            string name = "Table1";
            string caption = "Caption1";
            bool isRightAligned = false;
            TableHeaderTextBox target = new TableHeaderTextBox(name, caption, isRightAligned);
            Assert.AreEqual(target.Name,"Table1");
            Assert.AreEqual(target.Caption, "Caption1");
        }

        /// <summary>
        ///A test for TableHeaderTextBox Constructor
        ///</summary>
        [TestMethod()]
        public void TableHeaderTextBoxConstructor_ShouldSetTheAlignmentToRightIfPassed()
        {
            var ServiceLocatorMock = new Mock<ISimpleServiceLocator>();
            SimpleServiceLocator.SetServiceLocatorProvider(ServiceLocatorMock.Object);

            var reportingStyleMock = new Mock<IReportingServicesStyle>();
            reportingStyleMock.Setup(x => x.TextAlign).Returns("Right");

            ServiceLocatorMock.Setup(x => x.Get<IReportingServicesStyle>
                ("ReportingServicesStyleRight", It.IsAny<Dictionary<string, object>>())).Returns(reportingStyleMock.Object);

            string name = "Table1";
            string caption = "Caption1";
            bool isRightAligned = true;

            TableHeaderTextBox target = new TableHeaderTextBox(name, caption, isRightAligned);
            ServiceLocatorMock.Verify(x => x.Get<IReportingServicesStyle>("ReportingServicesStyleRight", It.IsAny<Dictionary<string, object>>()), Times.Once());
            ServiceLocatorMock.Verify(x => x.Get<IReportingServicesStyle>("ReportingServicesStyle", It.IsAny<Dictionary<string, object>>()), Times.Never());
            Assert.AreEqual(target.TextboxStyle.TextAlign, "Right");

            
           
        }

        /// <summary>
        ///A test for TableHeaderTextBox Constructor
        ///</summary>
        [TestMethod()]
        public void TableHeaderTextBoxConstructor_ShouldNotSetTheAlignmentToRightIfNotPassed()
        {
            var ServiceLocatorMock = new Mock<ISimpleServiceLocator>();
            SimpleServiceLocator.SetServiceLocatorProvider(ServiceLocatorMock.Object);

            var reportingStyleLeftMock = new Mock<IReportingServicesStyle>();
            reportingStyleLeftMock.Setup(x => x.TextAlign).Returns("");

            ServiceLocatorMock.Setup(x => x.Get<IReportingServicesStyle>
                ("ReportingServicesStyle", It.IsAny<Dictionary<string, object>>())).Returns(reportingStyleLeftMock.Object);

            string name = "Table1";
            string caption = "Caption1";
            bool isRightAligned = false;

            TableHeaderTextBox target = new TableHeaderTextBox(name, caption, isRightAligned);
            ServiceLocatorMock.Verify(x => x.Get<IReportingServicesStyle>("ReportingServicesStyleRight", It.IsAny<Dictionary<string, object>>()), Times.Never());
            ServiceLocatorMock.Verify(x => x.Get<IReportingServicesStyle>("ReportingServicesStyle", It.IsAny<Dictionary<string, object>>()), Times.Once());
            Assert.AreNotEqual(target.TextboxStyle.TextAlign, "Right");


        }

        #endregion

        #region Property Tests
        /// <summary>
        ///A test for TableHeaderTextBox Constructor
        ///</summary>
        [TestMethod()]
        public void TableHeaderTextBox_ShouldSetTheNameAndCaptionProperties()
        {
            var ServiceLocatorMock = new Mock<ISimpleServiceLocator>();
            SimpleServiceLocator.SetServiceLocatorProvider(ServiceLocatorMock.Object);

            string name = "Table1";
            string caption = "Caption1";
            bool isRightAligned = false; // TODO: Initialize to an appropriate value
            TableHeaderTextBox target = new TableHeaderTextBox(name, caption, isRightAligned);
            Assert.AreEqual(target.Name, "Table1");
            Assert.AreEqual(target.Caption, "Caption1");
        }

        

       
    
#endregion


        #region Render Test
        /// <summary>
        ///A test for Render2010
        ///</summary>
        [TestMethod()]
        public void TableHeaderTextBoxRender2010_ShouldRenderTextBoxRdlFragment()
        {
            var ServiceLocatorMock = new Mock<ISimpleServiceLocator>();
            SimpleServiceLocator.SetServiceLocatorProvider(ServiceLocatorMock.Object);
            
            var reportingStyleLeftMock = new Mock<IReportingServicesStyle>();
            reportingStyleLeftMock.Setup(x => x.TextAlign).Returns("");

            ServiceLocatorMock.Setup(x => x.Get<IReportingServicesStyle>
                ("ReportingServicesStyle", It.IsAny<Dictionary<string, object>>())).Returns(reportingStyleLeftMock.Object);

            string name = "Field1"; 
            string caption = "Caption1"; 
            TableHeaderTextBox target = new TableHeaderTextBox(name, caption);
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
                XmlAssert.AreEqual(outputRdl, "<TablixCell><CellContents><Textbox Name=\"HeaderField1\"><Paragraphs><Paragraph><TextRuns><TextRun><Value>Caption1</Value></TextRun></TextRuns></Paragraph></Paragraphs><CanGrow>true</CanGrow><Top>0in</Top><Left>0in</Left><Width>.5in</Width></Textbox></CellContents></TablixCell>");
            }
        }
        #endregion
       
    }
    
}
