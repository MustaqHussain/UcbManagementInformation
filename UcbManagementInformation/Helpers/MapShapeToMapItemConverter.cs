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
using System.Windows.Data;
using System.Text;
using System.Collections.Generic;
using Microsoft.Maps.MapControl;
using System.Text.RegularExpressions;
using UcbManagementInformation.Models;

namespace UcbManagementInformation
{
    public class MapShapeToMapItemConverter : IValueConverter
    {

        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            IMapShape ValueAsMapShape = (IMapShape)value;
            //get individual simple feature from WKT
            string shapeType = ValueAsMapShape.GetType().ToString();
            shapeType = shapeType.Substring(shapeType.LastIndexOf('.') + 1).ToUpper();
            // render according to simple feature type
            switch (shapeType)
            {
                case "MAPPOINT":
                    {
                        return GetPosition(ValueAsMapShape as MapPoint);
                        break;
                    }
                case "MAPLINE":
                    {
                        return GetLine(ValueAsMapShape as MapLine);
                        break;
                    }

                case "MAPPOLYGON":
                    {
                        return GetPolygon(ValueAsMapShape as UcbManagementInformation.Models.MapPolygon);
                        break;
                    }
                default:
                    return null;
            }
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            throw new NotImplementedException();
        }



        /// <summary>
        /// Render simple point features
        /// </summary>
        /// <param name="shapegeo">simple point feature</param>
        /// <param name="label">attributes for tooltip</param>
        /// <param name="currentLayer">MapLayer to add to</param>
        private Location GetPosition(MapPoint shapegeo)
        {
            return new Location(shapegeo.Location.Latitude, shapegeo.Location.Longitude);
        }
        /// <summary>
        /// Add Ellipse to indicate point location
        /// </summary>
        /// <param name="loc">Location lat,lon</param>
        /// <param name="fields">string of attributes for tooltip</param>
        /// <param name="layer">MapLeyer to add to</param>

        /// <summary>
        /// Render simple linestring features
        /// </summary>
        /// <param name="shapegeo">simple point feature</param>
        /// <param name="label">attributes for tooltip</param>
        /// <param name="currentLayer">MapLayer to add to</param>
        private LocationCollection GetLine(MapLine shapegeo)
        {
            LocationCollection p = new LocationCollection();

            foreach (LatLong pt in shapegeo.Locations)
            {
                p.Add(new Location(pt.Latitude, pt.Longitude));
            }
            return p;
        }

        /// <summary>
        /// Render simple polygon features
        /// </summary>
        /// <param name="shapegeo">simple point feature</param>
        /// <param name="label">attributes for tooltip</param>
        /// <param name="currentLayer">MapLayer to add to</param>
        private LocationCollection GetPolygon(UcbManagementInformation.Models.MapPolygon shapegeo)
        {

            LocationCollection p = new LocationCollection();
            foreach (LatLong pt in shapegeo.Locations)
            {
                p.Add(new Location(pt.Latitude, pt.Longitude));
            }
            // p.MouseEnter += polygon_MouseEnter;
            // p.MouseLeave += polygon_MouseLeave;

            // ToolTipService.SetToolTip(p, AddToolTip(label));
            return p;
        }
    }


}

