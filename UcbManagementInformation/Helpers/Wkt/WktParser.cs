using System;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Ink;
using System.Windows.Input;
using System.Linq;
using System.Linq.Expressions;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Shapes;
using UcbManagementInformation.MVVM;
using UcbManagementInformation.Models;
using System.Collections.ObjectModel;
using System.Text;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UcbManagementInformation.Commanding;
using UcbManagementInformation.Server.DataAccess;
using System.ServiceModel.DomainServices.Client;
using System.ServiceModel.DomainServices.Client.ApplicationServices;
using UcbManagementInformation.Server.DataAccess.BusinessObjects;
using UcbManagementInformation.Server;
using UcbManagementInformation.Web.Services;

namespace UcbManagementInformation.Helpers
{
    public class WktParser
    {
        List<MapLine> mapLines = new List<MapLine>();
        List<MapPoint> mapPoints = new List<MapPoint>();
        List<MapPolygon> mapPolygons = new List<MapPolygon>();

        public List<MapLine> MapLines
        {
            get { return mapLines; }
            set { mapLines = value; }
        }

        public List<MapPoint> MapPoints
        {
            get { return mapPoints; }
            set { mapPoints = value; }
        }

        public List<MapPolygon> MapPolygons
        {
            get { return mapPolygons; }
            set { mapPolygons = value; }
        }

        public virtual void ParseShape(WKTShape shapeToParse, string layer)
        {
            mapLines.Clear();
            mapPoints.Clear();
            mapPolygons.Clear();

            StringBuilder label = new StringBuilder();
            string shortName = null;

            try
            {
                if (null != layer && layer.ToLower() == "localauthoritylayer")
                {

                    //build attribute list for display as tooltip
                    foreach (KeyValuePair<string, string> field in shapeToParse.Fields)
                    {

                        if (field.Key.ToLower() == "lanamepc" || field.Key.ToLower() == "region")
                        {
                            shortName = field.Value;
                            label.Append(field.Key + ":" + field.Value + "\n");
                        }
                    }
                }

                string shape = shapeToParse.WKT.Substring(0, shapeToParse.WKT.IndexOf("(")).Trim();
                string shapegeo = shapeToParse.WKT.Substring(shapeToParse.WKT.IndexOf("(")).Trim(new char[] { '(', ')' });

                switch (shape)
                {
                    case "GEOMETRYCOLLECTION":
                        {
                            DrawGeometryCollection(shapegeo, label.ToString(), shortName);
                            break;
                        }
                    case "POINT":
                    case "MULTIPOINT":
                        {
                            DrawPoints(shapegeo);
                            break;
                        }
                    case "LINESTRING":
                    case "MULTILINESTRING":
                        {
                            DrawLinestrings(shapegeo);
                            break;
                        }
                    case "POLYGON":
                    case "MULTIPOLYGON":
                        {
                            DrawPolygons(shapegeo, label.ToString(), shortName);
                            break;
                        }
                }
            }
            catch
            {
                throw new ArgumentOutOfRangeException("The text supplied was not valid Well Known Text.");
            }
        }

        private void DrawGeometryCollection(string shapegeo, string label, string shortName)
        {
            string[] geoms = Regex.Split(shapegeo, "\\)+,+");
            foreach (string geom in geoms)
            {
                string[] shapes = Regex.Split(geom, " +\\(+");
                switch (shapes[0].Trim())
                {
                    case "POINT":
                    case "MULTIPOINT":
                        {
                            DrawPoints(shapes[1]);
                            break;
                        }
                    case "LINESTRING":
                    case "MULTILINESTRING":
                        {
                            DrawLinestrings(shapes[1]);
                            break;
                        }

                    case "POLYGON":
                    case "MULTIPOLYGON":
                        {
                            DrawPolygons(shapes[1], label.ToString(), shortName);
                            break;
                        }
                }
            }
        }

        private void DrawPolygons(string shapegeo, string label, string shortName)
        {
            mapPolygons = new List<MapPolygon>();

            string[] rings = Regex.Split(shapegeo, "\\)+, \\(+");
            foreach (string ring in rings)
            {
                MapPolygon p = new MapPolygon();
                p.Locations = new ObservableCollection<LatLong>();

                string[] pts = Regex.Split(ring, @",\s+");
                foreach (string pt in pts)
                {
                    p.Locations.Add(ConvertPointToLatLong(pt));
                }

                p.Name = label;
                p.ShortName = shortName;

                mapPolygons.Add(p);
            }

        }

        private void DrawPoints(string shapegeo)
        {
            mapPoints = new List<MapPoint>();

            string[] points = Regex.Split(shapegeo, "\\)+, \\(+");
            foreach (string pt in points)
            {
                MapPoint p = new MapPoint { Location = ConvertPointToLatLong(pt) };
                mapPoints.Add(p);
            }

        }

        private void DrawLinestrings(string shapegeo)
        {
            mapLines = new List<MapLine>();

            string[] lines = Regex.Split(shapegeo, "\\)+, \\(+");
            foreach (string line in lines)
            {
                MapLine p = new MapLine();
                p.Locations = new ObservableCollection<LatLong>();
                string[] pts = Regex.Split(line, @",\s+");
                foreach (string pt in pts)
                {
                    p.Locations.Add(ConvertPointToLatLong(pt));
                }

                mapLines.Add(p);
            }
        }

        private LatLong ConvertPointToLatLong(string pt)
        {
            string[] values = pt.Split(' ');
            double lon = double.Parse(values[0]);
            double lat = double.Parse(values[1]);
            LatLong loc = new LatLong(lat, lon);

            return loc;
        }
    }
}
