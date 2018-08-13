/* ******************************************************************************
 * 
 * Copyright 2010 Microsoft Corporation
 * 
 * Licensed under the Apache License, Version 2.0 (the "License"); you may not 
 * use this file except in compliance with the License. You may obtain a copy of 
 * the License at 
 * 
 * http://www.apache.org/licenses/LICENSE-2.0 
 * 
 * THIS CODE IS PROVIDED *AS IS* BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY 
 * KIND, EITHER EXPRESS OR IMPLIED, INCLUDING WITHOUT LIMITATION ANY IMPLIED
 * WARRANTIES OR CONDITIONS OF TITLE, FITNESS FOR A PARTICULAR PURPOSE,
 * MERCHANTABLITY OR NON-INFRINGEMENT. 
 *  
 * See the Apache 2 License for the specific language governing permissions and
 * limitations under the License.
 * 
 ******************************************************************************* */
using System;
using System.Data;
using System.Data.Objects.DataClasses;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.DomainServices.Hosting;
using System.ServiceModel.DomainServices.Server;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.ServiceModel.Activation;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.SqlServer.Types;
using log4net;
using System.Configuration;
using System.Data.SqlClient;
using System.Web;
using System.ServiceModel.Web;

namespace UcbManagementInformation.Web.Services
{
    [EnableClientAccess()]
    public class WKTService : DomainService
    {

        /* setup log4net logger */
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(
            System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);

        /* default global parameters */
        private string geomField = "Shape"; // name of Geography DataType column in tables
        private string geomFieldDB = null;
        private string srid = "4326"; // name of SRID Constraint, EPSG:4326, of tables
        private int recordLimit = 1500;

        /// <summary>
        /// GetSQLDataWKT
        ///     returns WKT with results of SQL Server query
        /// </summary>
        /// <param name="Parameters">WKTParameters required for query</param>
        [Invoke(HasSideEffects = true)]
        public WKTResponse GetSQLDataWKT(WKTParameters Parameters)
        {

            DateTime queryStart = DateTime.Now;
            DateTime queryStop;
            int recordCnt = 0;

            WKTResponse _WKTResponse = new WKTResponse();
            _WKTResponse.ErrorCode = 0;
            _WKTResponse.OutputMessage = "Success";
            _WKTResponse.OutputShapes = new List<WKTShape>();
            string connStr = ConfigurationManager.ConnectionStrings["DataConnectionString"].ConnectionString;
            SqlConnection conn = new SqlConnection(connStr);
            SqlDataReader rdr = null;
            try
            {
                #region Build Query

                if (Parameters.Querytype == null || Parameters.Querytype.Length == 0) throw new ArgumentException("Invalid Parameters: querytype=\"" + Parameters.Querytype + "\"");
                if (Parameters.Querytype.ToLower().Equals("buffer") && Parameters.Radius <= 0) throw new ArgumentException("Invalid Parameters: querytype requires radius >0 - \"" + Parameters.Radius + "\"");
                if (Parameters.Points.Length == 0 && Parameters.Querytype.ToLower().Equals("buffer")) throw new ArgumentException("Invalid Parameters: points must contain at least one point for querytype buffer");
                if (Parameters.Points.Split(',').Length != 5 && Parameters.Querytype.ToLower().Equals("bbox")) throw new ArgumentException("Invalid Parameters: points must contain 5 points for a closed querytype bbox");

                StringBuilder query = new StringBuilder();

                geomFieldDB = geomField;
                string RegionIndexName;
                string LocalAuthorityIndexName;
                string ReduceQuery;

                if (Parameters.Reduce >= 1000)
                {
                    // Use Shape1000 data - pre-calculated data from Shape field, already reduced by 1000
                    geomFieldDB = geomField + "1000";
                    RegionIndexName = "geog_sidx1";
                    LocalAuthorityIndexName = "geog_sidx1";
                    ReduceQuery = string.Empty;
                }
                else
                {
                    // Use Shape data
                    geomFieldDB = geomField;
                    RegionIndexName = "shape_sidx";
                    LocalAuthorityIndexName = "geog_sidx";
                    ReduceQuery = ".Reduce(50)";
                    //ReduceQuery = string.Empty;
                }

                if (Parameters.Querytype.ToLower().Equals("bbox"))
                {   //BBox
                    if (Parameters.Table.ToLower() == "region")
                    {
                        query.Append("SELECT Code, RegionID, RegionName, DateUpdated, LastUpdatedUserCode, RowIdentifier, " + geomFieldDB + ReduceQuery + ".STAsText() as " + geomField + "wkt FROM [dbo].[" + Parameters.Table + "] WITH(INDEX(" + (Parameters.Table.ToLower() == "localauthority" ? LocalAuthorityIndexName : RegionIndexName) + ")) WHERE ");
                    }
                    else if (Parameters.Table.ToLower() == "localauthority")
                    {
                        query.Append("SELECT ID, NAME, AREA_CODE, DESCRIPTIO, FILE_NAME, NUMBER, NUMBER0, POLYGON_ID, UNIT_ID, CODE, HECTARES, AREA, TYPE_CODE, DESCRIPT0, TYPE_COD0, DESCRIPT1, Region, LANamePc, " + geomFieldDB + ReduceQuery + ".STAsText() as " + geomField + "wkt FROM [dbo].[" + Parameters.Table + "] WITH(INDEX(" + (Parameters.Table.ToLower() == "localauthority" ? LocalAuthorityIndexName : RegionIndexName) + ")) WHERE ");
                    }
                    else
                    {
                        query.Append("SELECT *, " + geomFieldDB + ReduceQuery + ".STAsText() as " + geomField + "wkt FROM [dbo].[" + Parameters.Table + "] WITH(INDEX(" + (Parameters.Table.ToLower() == "localauthority" ? LocalAuthorityIndexName : RegionIndexName) + ")) WHERE ");
                    }
                    query.Append(geomFieldDB + ".STIntersects(geography::STGeomFromText('POLYGON(('+@points+'))', @srid))=1 AND " + Parameters.RegionField + " NOT IN ('Scotland','Wales')");
                }
                else if (Parameters.Querytype.ToLower().Equals("buffer"))
                {
                    query.Append("SELECT *," + geomFieldDB + ReduceQuery + ".STAsText() as " + geomField + "wkt FROM [dbo].[" + Parameters.Table + "] WITH(INDEX(" + (Parameters.Table.ToLower() == "localauthority" ? LocalAuthorityIndexName : RegionIndexName) + ")) WHERE ");
                    if (Parameters.Points.Split(',').Length > 1)
                    {   //Polyline Buffer
                        query.Append(geomFieldDB + ".STIntersects(geography::STGeomFromText('LINESTRING('+@points+')', @srid).STBuffer(@radius))=1 AND " + Parameters.RegionField + " NOT IN ('Scotland','Wales')");
                    }
                    else
                    {   //Point Buffer
                        query.Append(geomFieldDB + ".STIntersects(geography::STGeomFromText('POINT('+@points+')', @srid).STBuffer(@radius))=1 AND " + Parameters.RegionField + " NOT IN ('Scotland','Wales')");
                    }
                }

                #endregion

                log.Info(query);

                queryStart = DateTime.Now;
                SqlCommand cmd = new SqlCommand(query.ToString(), conn);
                cmd.Parameters.Add(new SqlParameter("reduce", Parameters.Reduce));
                cmd.Parameters.Add(new SqlParameter("srid", srid));
                cmd.Parameters.Add(new SqlParameter("points", Parameters.Points));
                cmd.Parameters.Add(new SqlParameter("radius", Parameters.Radius));
                conn.Open();

                rdr = cmd.ExecuteReader();
                while (rdr.Read())
                {
                    WKTShape shp = new WKTShape();
                    shp.Fields = new Dictionary<string, string>();
                    for (int i = 0; i < rdr.FieldCount; i++)
                    {
                        //log.Debug(rdr[i].GetType() + "  " + rdr.GetName(i));
                        if (rdr[i].GetType().Equals(typeof(SqlGeography)) ||
                            rdr[i].GetType().Equals(typeof(SqlGeometry))
                            )
                        { //skip
                            //log.Debug("skip " + rdr[i].GetType());
                        }
                        else
                        {
                            if (rdr.GetName(i).Equals("ID"))
                            {
                                shp.ID = rdr[i].ToString();
                                //log.Debug(rdr[i].GetType() + "  " + rdr.GetName(i) + "  " + rdr[i].ToString());
                            }
                            else if (rdr.GetName(i).Equals(geomField + "wkt"))
                            {
                                //log.Debug(rdr[i].GetType() + "  " + rdr.GetName(i) + "  " + rdr[i].ToString());
                                shp.WKT = rdr[i].ToString();
                            }
                            else
                            {
                                shp.Fields.Add(rdr.GetName(i), rdr[i].ToString());
                                //log.Debug(rdr.GetName(i) + " : " + rdr[i].ToString());
                            }
                        }
                    }
                    //log.Debug(shp.ID + "  " + shp.WKT);
                    if (recordCnt++ > recordLimit) throw new Exception("Query result exceeds limit " + recordLimit);
                    _WKTResponse.OutputShapes.Add(shp);
                }
            }
            catch (ArithmeticException e)
            {
                ServiceException(_WKTResponse, "ArithmeticException " + e.Message, 3);
            }
            catch (ArgumentException e)
            {
                ServiceException(_WKTResponse, "ArgumentException " + e.Message, 1);
            }
            catch (Exception e)
            {
                ServiceException(_WKTResponse, e.Message, 2);
            }
            finally
            {
                if (rdr != null) rdr.Close();
                if (conn != null) conn.Close();
            }
            queryStop = DateTime.Now;
            //log.Debug(String.Format("Query Time: {0,0:0}ms", (queryStop - queryStart).TotalMilliseconds));
            _WKTResponse.QueryTime = (queryStop - queryStart).TotalMilliseconds;

            return _WKTResponse;
        }

        
        /// <summary>
        /// ServiceException
        ///     sets response for exception
        /// </summary>
        /// <param name="response">WKTResponse</param>
        /// <param name="message">string exception messsage</param>
        /// <param name="code">int error code</param>
        public void ServiceException(WKTResponse response, string message, int code)
        {
            response.ErrorCode = code;
            response.OutputMessage = message;
            response.OutputShapes.Clear();
            log.Error(message);
        }

    }

    public class WKTResponse : ComplexObject
    {
        public int ErrorCode { get; set; }
        public string OutputMessage { get; set; }
        public List<WKTShape> OutputShapes { get; set; }
        public double QueryTime { get; set; }
    }

    public class WKTParameters : ComplexObject
    {
        public string Table { get; set; }
        public string RegionField { get; set; }
        public string Querytype { get; set; }
        public double Radius { get; set; }
        public string Points { get; set; }
        public double Reduce { get; set; }
    }

    public class WKTShape : ComplexObject
    {
        public string ID { get; set; }
        public Dictionary<string, string> Fields { get; set; }
        public string WKT { get; set; }
    }
}
