using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace UcbManagementInformation.Web.MIFileUpload
{
    public class PostCode
    {
        private string _postCode7;

        public string PostCode7
        {
            get { return _postCode7; }
            set { _postCode7 = value; }
        }
        private string _postCode8;

        public string PostCode8
        {
            get { return _postCode8; }
            set { _postCode8 = value; }
        }
        private string _county;

        public string County
        {
            get { return _county; }
            set { _county = value; }
        }
        private string _lad_Ua;

        public string Lad_Ua
        {
            get { return _lad_Ua; }
            set { _lad_Ua = value; }
        }
        private string _ward;

        public string Ward
        {
            get { return _ward; }
            set { _ward = value; }
        }
        private string _parl_Con;

        public string Parl_Con
        {
            get { return _parl_Con; }
            set { _parl_Con = value; }
        }
        private string _lea;

        public string Lea
        {
            get { return _lea; }
            set { _lea = value; }
        }
        private string _constituencyID;

        public string ConstituencyID
        {
            get { return _constituencyID; }
            set { _constituencyID = value; }
        }
        private string _nuts1;

        public string Nuts1
        {
            get { return _nuts1; }
            set { _nuts1 = value; }
        }
        private string _easting;

        public string Easting
        {
            get { return _easting; }
            set { _easting = value; }
        }
        private string _northinging;

        public string Northinging
        {
            get { return _northinging; }
            set { _northinging = value; }
        }
        private string _postCodeCompressed;

        public string PostCodeCompressed
        {
            get { return _postCodeCompressed; }
            set { _postCodeCompressed = value; }
        }
        private string _countyName;

        public string CountyName
        {
            get { return _countyName; }
            set { _countyName = value; }
        }
        private string _lEAName;

        public string LEAName
        {
            get { return _lEAName; }
            set { _lEAName = value; }
        }
        private string _wardName;

        public string WardName
        {
            get { return _wardName; }
            set { _wardName = value; }
        }
        private string _constituencyName;

        public string ConstituencyName
        {
            get { return _constituencyName; }
            set { _constituencyName = value; }
        }
        private string _localAuthorityName;

        public string LocalAuthorityName
        {
            get { return _localAuthorityName; }
            set { _localAuthorityName = value; }
        }
        private string _regionName;

        public string RegionName
        {
            get { return _regionName; }
            set { _regionName = value; }
        }
    }
}