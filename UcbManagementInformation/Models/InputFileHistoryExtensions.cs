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

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class InputFileHistory
    {

        private int daysAfterNewestFile;

        public int DaysAfterNewestFile
        {
            get { return daysAfterNewestFile; }
            set { daysAfterNewestFile = value; this.RaisePropertyChanged("DaysAfterNewestFile"); }
        }

        private int noOfRecordsUploaded;

        public int NoOfRecordsUploaded
        {
            get
            {
                switch (this.Status)
                {
                    case "Validated Loaded 0":
                        noOfRecordsUploaded = this.NumberOfValidRecords;
                        break;
                    case "Validated Loaded 1":
                        noOfRecordsUploaded = this.NumberOfValidRecords +
                            this.NumberOfInformationalRecords;
                        break;
                    case "Validated Loaded 2":
                        noOfRecordsUploaded = this.NumberOfValidRecords +
                            this.NumberOfInformationalRecords +
                            this.NumberOfWarningRecords;
                        break;
                    default:
                        noOfRecordsUploaded = 0;
                        break;
                } return noOfRecordsUploaded;
            }
          
        }

        
                    
                    
    }
}
