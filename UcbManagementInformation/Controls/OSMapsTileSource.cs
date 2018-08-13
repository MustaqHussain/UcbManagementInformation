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

namespace UcbManagementInformation.Controls
{
    public class OSMapsTileSource : Microsoft.Maps.MapControl.TileSource
    {
        
        //These tiles are ours on our map server.
        public OSMapsTileSource()
            : base("http://mgmt-con-web.esffiredom.co.uk:81/tiles_index/{2}/{0}/{1}.png")
        { }

        public override Uri GetUri(int x, int y, int zoomLevel)
        {
            //Why are our tiles upside down i have no idea?
            return new Uri(string.Format(this.UriFormat, x, Convert.ToInt32((System.Math.Pow(2, zoomLevel))) - y - 1, zoomLevel));
        }

        //These tiles are free from open street map on the web.
        //public OSMapsTileSource()
        //    : base("http://tile.openstreetmap.org/{2}/{0}/{1}.png")
        //{ }

        //public override Uri GetUri(int x, int y, int zoomLevel)
        //{

        //    return new Uri(string.Format(this.UriFormat, x, y, zoomLevel));
        //}

    }
    
    
    
}
