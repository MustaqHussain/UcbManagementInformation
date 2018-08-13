using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.IO;

namespace UcbManagementInformation.Web.Models
{
    
    public class UploadFile
    {
        private string name;

        public string Name
        {
            get { return name; }
            set { name = value; }
        }
        private Stream file;

        public Stream File
        {
            get { return file; }
            set { file = value; }
        }
    }
    public class UploadContainer
    {
        private static volatile UploadContainer instance;
        private static object syncRoot = new Object();

        private UploadContainer()
        { }

        public static UploadContainer Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                        {
                            instance = new UploadContainer();
                        }
                    }
                }
                return instance;
            }
        }

        private List<UploadFile> masterList = new List<UploadFile>();

        public List<UploadFile> MasterList
        {
            get { return masterList; }

        }

    }
}