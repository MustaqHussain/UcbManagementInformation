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
using System.ComponentModel.DataAnnotations;

namespace UcbManagementInformation.Web.Models
{
    public class EsfTickerItemDto
    {
        private int tickerItemKey;
        [Key]
        public int TickerItemKey
        {
            get { return tickerItemKey; }
            set { tickerItemKey = value; }
        }
        private string genericText;

        public string GenericText
        {
            get { return genericText; }
            set { genericText = value; }
        }
        private string retrievedValue;

        public string RetrievedValue
        {
            get { return retrievedValue; }
            set { retrievedValue = value; }
        }

    }
}
