using System.Linq;
using System.ServiceModel.DomainServices.Client;
using UcbManagementInformation.Server.DataAccess;
using UcbManagementInformation.ViewModels;
using UcbManagementInformation.Helpers;
using Microsoft.Silverlight.Testing;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Moq;
using UcbManagementInformation.Web.Services;
using System;

namespace UcbManagementInformation.Silverlight.UnitTest.Tests.ViewModelTests
{
    [TestClass]
    public class WktParserTests : SilverlightTest
    {
        string geoPolygon;
        string geoPoint;
        string geoLine;
        string geoGeomCollection;

        [TestInitialize]
        public void TestSetUp()
        {
            geoPolygon = "POLYGON ((-1 -1, -1 1, 1 1, 1 -1))";
            geoPoint = "POINT (1 1)";
            geoLine = "LINESTRING (-1 -1, 1 1)";
            geoGeomCollection = "GEOMETRYCOLLECTION (POLYGON ((-1 -1, -1 1, 1 1, 1 -1)), LINESTRING (-1 -1, 1 1), POINT (0 0))";
        }

        #region ParseShape_ShouldGetAMapPolygonWithNameAndShortName

        [TestMethod]
        public void ParseShape_ShouldGetAMapPolygonWithNameAndShortName()
        {
            //Arrange
            string shapeWktText = geoPolygon;
            
            WKTShape shape = new WKTShape();
            shape.WKT = shapeWktText;
            shape.Fields = new System.Collections.Generic.Dictionary<string, string>();
            shape.Fields.Add("lanamepc", "Rotherham");

            WktParser parser = new WktParser();

            //Act
            parser.ParseShape(shape, "localauthoritylayer");
       
            //Assert
            Assert.IsTrue(parser.MapPolygons.Count() == 1, "Should return 1 Polygon");
            Assert.IsTrue(parser.MapLines.Count() == 0, "Should be 0 Lines");
            Assert.IsTrue(parser.MapPoints.Count() == 0, "Should be 0 Points");
            Assert.IsTrue(parser.MapPolygons[0].Name == "lanamepc:Rotherham\n", "Name has not been set correctly");
            Assert.IsTrue(parser.MapPolygons[0].ShortName == "Rotherham", "ShortName has not been set corectly");
            Assert.IsTrue(parser.MapPolygons[0].MaxValue == 0, "MaxValue was not 0");
            Assert.IsTrue(parser.MapPolygons[0].Value == 0, "Value was not 0");
            Assert.IsTrue(parser.MapPolygons[0].Percent == 0, "Percent was not 0");
        }

        #endregion

        #region ParseShape_ShouldGetAMapPolygonWithNoNameNorShortName

        [TestMethod]
        public void ParseShape_ShouldGetAMapPolygonWithNoNameNorShortName()
        {
            //Arrange
            string shapeWktText = geoPolygon;

            WKTShape shape = new WKTShape();
            shape.WKT = shapeWktText;
            shape.Fields = new System.Collections.Generic.Dictionary<string, string>();
            shape.Fields.Add("NotARegionNorLocalAuthority", "Test value");

            WktParser parser = new WktParser();

            //Act
            parser.ParseShape(shape, "localauthoritylayer");

            //Assert
            Assert.IsTrue(parser.MapPolygons.Count() == 1, "Should return 1 Polygon");
            Assert.IsTrue(parser.MapLines.Count() == 0, "Should be 0 Lines");
            Assert.IsTrue(parser.MapPoints.Count() == 0, "Should be 0 Points");
            Assert.IsTrue(string.IsNullOrEmpty(parser.MapPolygons[0].Name), "Name should not be set");
            Assert.IsTrue(parser.MapPolygons[0].ShortName == null, "ShortName should not be set");
            Assert.IsTrue(parser.MapPolygons[0].MaxValue == 0, "MaxValue was not 0");
            Assert.IsTrue(parser.MapPolygons[0].Value == 0, "Value was not 0");
            Assert.IsTrue(parser.MapPolygons[0].Percent == 0, "Percent was not 0");
        }

        #endregion

        #region ParseShape_ShouldGetAMapLine

        [TestMethod]
        public void ParseShape_ShouldGetAMapLine()
        {
            //Arrange
            string shapeWktText = geoLine;

            WKTShape shape = new WKTShape();
            shape.WKT = shapeWktText;

            WktParser parser = new WktParser();

            //Act
            parser.ParseShape(shape, null);

            //Assert
            Assert.IsTrue(parser.MapPolygons.Count() == 0);
            Assert.IsTrue(parser.MapLines.Count() == 1);
            Assert.IsTrue(parser.MapPoints.Count() == 0);
        }

        #endregion

        #region ParseShape_ShouldGetAMapPoint

        [TestMethod]
        public void ParseShape_ShouldGetAMapPoint()
        {
            //Arrange
            string shapeWktText = geoPoint;

            WKTShape shape = new WKTShape();
            shape.WKT = shapeWktText;

            WktParser parser = new WktParser();

            //Act
            parser.ParseShape(shape, null);

            //Assert
            Assert.IsTrue(parser.MapPolygons.Count() == 0);
            Assert.IsTrue(parser.MapLines.Count() == 0);
            Assert.IsTrue(parser.MapPoints.Count() == 1);
        }

        #endregion

        #region ParseShape_ShouldGetAMapPointMapLineAndAMapPolygon

        [TestMethod]
        public void ParseShape_ShouldGetAMapPointMapLineAndAMapPolygon()
        {
            //Arrange
            string shapeWktText = geoGeomCollection;

            WKTShape shape = new WKTShape();
            shape.WKT = shapeWktText;
            shape.Fields = new System.Collections.Generic.Dictionary<string, string>();
            shape.Fields.Add("region", "Cornwall");

            WktParser parser = new WktParser();

            //Act
            parser.ParseShape(shape, "localauthoritylayer");

            //Assert
            Assert.IsTrue(parser.MapPolygons.Count() == 1, "Should return 1 Polygon");
            Assert.IsTrue(parser.MapLines.Count() == 1, "Should be 1 Line");
            Assert.IsTrue(parser.MapPoints.Count() == 1, "Should be 1 Point");
            Assert.IsTrue(parser.MapPolygons[0].Name == "region:Cornwall\n", "Name has not been set correctly");
            Assert.IsTrue(parser.MapPolygons[0].ShortName == "Cornwall", "ShortName has not been set corectly");
            Assert.IsTrue(parser.MapPolygons[0].MaxValue == 0, "MaxValue was not 0");
            Assert.IsTrue(parser.MapPolygons[0].Value == 0, "Value was not 0");
            Assert.IsTrue(parser.MapPolygons[0].Percent == 0, "Percent was not 0");
        }

        #endregion

        #region ParseShape_ShouldGetAnException

        [TestMethod]
        // Not using ExpectedException attribute as it appears to be frowned upon.
        public void ParseShape_ShouldGetAnException()
        {
            //Arrange
            string shapeWktText = "This is not WKT";

            WKTShape shape = new WKTShape();
            shape.WKT = shapeWktText;

            WktParser parser = new WktParser();

            try
            {
                //Act
                parser.ParseShape(shape, null);
            }
            catch (Exception ex)
            {
                //Assert
                Assert.IsTrue(ex.GetType() == typeof(ArgumentOutOfRangeException), "Expected exception of type " + typeof(ArgumentOutOfRangeException) + " but type of " + ex.GetType() + " was thrown instead.");
                return;
            }

            //Assert
            Assert.Fail("Expected exception of type " + typeof(ArgumentOutOfRangeException) + " but no exception was thrown.");
        }

        #endregion

    }
}