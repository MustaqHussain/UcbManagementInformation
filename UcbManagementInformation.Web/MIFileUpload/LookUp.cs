using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UcbManagementInformation.Web.MIFileUpload
{
    public class LookUp
    {
        private string _recordType;

        public string RecordType
        {
            get { return _recordType; }
            set { _recordType = value; }
        }
        private string _lookUpKey;

        public string LookUpKey
        {
            get { return _lookUpKey; }
            set { _lookUpKey = value; }
        }

        private string _name;

        public string Name
        {
            get { return _name; }
            set { _name = value; }
        }
    }
}