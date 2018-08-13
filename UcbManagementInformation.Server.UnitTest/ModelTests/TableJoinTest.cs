using System;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using UcbManagementInformation.Server.RDL2003Engine;

namespace UcbManagementInformation.Server.UnitTest.ModelTests
{
    [TestClass]
    public class TableJoinTest
    {
        [TestMethod]
        public void ConstructorTest()
        {
            string fromSchema = "dbo";
            string toSchema = "dmacr";
            string fromTable = "dboTable";
            string toTable = "dmacrTable";
            string fromField = "dboColumn";
            string toField = "dmacrColumn";
            int index = 0;
            string joinMethod = "INNER";
            bool isOneToOne = false;

            TableJoin tj = new TableJoin(fromSchema, toSchema, fromTable, toTable, fromField, toField, index, joinMethod, isOneToOne);

            Assert.AreEqual(fromSchema, tj.FromSchema);
            Assert.AreEqual(toSchema, tj.ToSchema);
            Assert.AreEqual(fromTable, tj.FromTable);
            Assert.AreEqual(toTable, tj.ToTable);
            Assert.AreEqual(fromField, tj.FromField);
            Assert.AreEqual(toField, tj.ToField);
            Assert.AreEqual(index, tj.Index);
            Assert.AreEqual(joinMethod + " JOIN", tj.JoinMethod);
            Assert.AreEqual(isOneToOne, tj.IsOneToOne);
        }

        [TestMethod]
        public void RenderTest_WithDboSchema_AndFirstJoin()
        {
            string fromSchema = "dbo";
            string toSchema = "dbo";
            string fromTable = "dboTable";
            string toTable = "dboTable2";
            string fromField = "dboColumn";
            string toField = "dboColumn2";
            int index = 0;
            string joinMethod = "INNER";
            bool isOneToOne = false;

            TableJoin tj = new TableJoin(fromSchema, toSchema, fromTable, toTable, fromField, toField, index, joinMethod, isOneToOne);

            string expected = String.Format("[{0}].[{1}] {2} JOIN [{3}].[{4}] ON [{0}].[{1}].[{5}] = [{3}].[{4}].[{6}]", fromSchema, fromTable, joinMethod, toSchema, toTable, fromField, toField);
            string actual = tj.Render();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RenderTest_WithDmacrSchema_AndFirstJoin()
        {
            string fromSchema = "dmacr";
            string toSchema = "dmacr";
            string fromTable = "dmacrTable";
            string toTable = "dmacrTable2";
            string fromField = "dmacrColumn";
            string toField = "dmacrColumn2";
            int index = 0;
            string joinMethod = "INNER";
            bool isOneToOne = false;

            TableJoin tj = new TableJoin(fromSchema, toSchema, fromTable, toTable, fromField, toField, index, joinMethod, isOneToOne);

            string expected = String.Format("[{0}].[{1}] {2} JOIN [{3}].[{4}] ON [{0}].[{1}].[{5}] = [{3}].[{4}].[{6}]", fromSchema, fromTable, joinMethod, toSchema, toTable, fromField, toField);
            string actual = tj.Render();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RenderTest_WithDboAndDmacrSchema_AndFirstJoin()
        {
            string fromSchema = "dbo";
            string toSchema = "dmacr";
            string fromTable = "dboTable";
            string toTable = "dmacrTable";
            string fromField = "dboColumn";
            string toField = "dmacrColumn";
            int index = 0;
            string joinMethod = "INNER";
            bool isOneToOne = false;

            TableJoin tj = new TableJoin(fromSchema, toSchema, fromTable, toTable, fromField, toField, index, joinMethod, isOneToOne);

            string expected = String.Format("[{0}].[{1}] {2} JOIN [{3}].[{4}] ON [{0}].[{1}].[{5}] = [{3}].[{4}].[{6}]", fromSchema, fromTable, joinMethod, toSchema, toTable, fromField, toField);
            string actual = tj.Render();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RenderTest_WithDboSchema_AndSecondJoin()
        {
            string fromSchema = "dbo";
            string toSchema = "dbo";
            string fromTable = "dboTable";
            string toTable = "dboTable2";
            string fromField = "dboColumn";
            string toField = "dboColumn2";
            int index = 1;
            string joinMethod = "INNER";
            bool isOneToOne = false;

            TableJoin tj = new TableJoin(fromSchema, toSchema, fromTable, toTable, fromField, toField, index, joinMethod, isOneToOne);

            string expected = String.Format(" {2} JOIN [{3}].[{4}] ON [{3}].[{4}].[{6}] = [{0}].[{1}].[{5}]", fromSchema, fromTable, joinMethod, toSchema, toTable, fromField, toField);
            string actual = tj.Render();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RenderTest_WithDmacrSchema_AndSecondJoin()
        {
            string fromSchema = "dmacr";
            string toSchema = "dmacr";
            string fromTable = "dmacrTable";
            string toTable = "dmacrTable2";
            string fromField = "dmacrColumn";
            string toField = "dmacrColumn2";
            int index = 1;
            string joinMethod = "INNER";
            bool isOneToOne = false;

            TableJoin tj = new TableJoin(fromSchema, toSchema, fromTable, toTable, fromField, toField, index, joinMethod, isOneToOne);

            string expected = String.Format(" {2} JOIN [{3}].[{4}] ON [{3}].[{4}].[{6}] = [{0}].[{1}].[{5}]", fromSchema, fromTable, joinMethod, toSchema, toTable, fromField, toField);
            string actual = tj.Render();
            Assert.AreEqual(expected, actual);
        }

        [TestMethod]
        public void RenderTest_WithDboAndDmacrSchema_AndSecondJoin()
        {
            string fromSchema = "dbo";
            string toSchema = "dmacr";
            string fromTable = "dboTable";
            string toTable = "dmacrTable";
            string fromField = "dboColumn";
            string toField = "dmacrColumn";
            int index = 1;
            string joinMethod = "INNER";
            bool isOneToOne = false;

            TableJoin tj = new TableJoin(fromSchema, toSchema, fromTable, toTable, fromField, toField, index, joinMethod, isOneToOne);

            string expected = String.Format(" {2} JOIN [{3}].[{4}] ON [{3}].[{4}].[{6}] = [{0}].[{1}].[{5}]", fromSchema, fromTable, joinMethod, toSchema, toTable, fromField, toField);
            string actual = tj.Render();
            Assert.AreEqual(expected, actual);
        }
    }
}
