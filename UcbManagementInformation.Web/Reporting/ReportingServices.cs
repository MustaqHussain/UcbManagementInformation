using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
//using UcbManagementInformation.Web.ReportingService2005;
using UcbManagementInformation.Web.ReportingService2010;
using UcbManagementInformation.Web.Reporting;
using System.Net;
using System.Configuration;
using UcbManagementInformation.Server.DataAccess;


namespace UcbManagementInformation.Web.Reporting
{
    public class ReportingServices : IReportingServices
    {
        /// <summary>
        /// Publish report through reporting service
        /// </summary>
        /// <param name="reportDefinition">Byte Array of RDL format report</param>
        /// <param name="reportName">Report Name</param>
        /// <param name="reportPath">Report Full Path</param>
        /// <returns>true: successful; false: failed</returns>
        public bool PublishReport(Byte[] reportDefinition, string reportName, string reportPath, string dataSourceName)
        {

            Warning[] warnings = null;

            ReportingService2010.ReportingService2010  CurrentService = new ReportingService2010.ReportingService2010();
            CurrentService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //set the credentials
            string UserReportID = ConfigurationManager.AppSettings["userReportID"];
            string UerReportPassword = ConfigurationManager.AppSettings["userReportPassword"];
            string UserReportDomain = ConfigurationManager.AppSettings["userReportDomain"];

            CurrentService.Credentials = new NetworkCredential(UserReportID, UerReportPassword, UserReportDomain);
            if (reportPath.Substring(reportPath.Length-1) == "/")
            {
                reportPath = reportPath.Substring(0,reportPath.Length-1);
            }
            CatalogItem myItem = CurrentService.CreateCatalogItem("Report", reportName, "/" + reportPath, true, reportDefinition,null,out warnings);
            
            //test if create worked OK
            if (warnings == null)
            {
                DataSource[] dsArray = CurrentService.GetItemDataSources("/" + reportPath + "/" + reportName);
                DataSourceReference dsRef = new DataSourceReference();
                dsRef.Reference = "/" + dataSourceName;
                dsArray[0].Item = dsRef;
                CurrentService.SetItemDataSources("/" + reportPath + "/" + reportName, dsArray);
                return true;
            }
            else
            {
                return false;
            }
        }



        public bool CopyReport(Report reportToPublish,StandardReport reportInfo)
        {
            Warning[] warnings = null;
            string reportPath = ConfigurationManager.AppSettings["ReportPublishedLocation"];
            string reportName = reportInfo.ReportName;

            string dataSourceName = reportToPublish.DataModel_1.DatasourceName;

            ReportingService2010.ReportingService2010 CurrentService = new ReportingService2010.ReportingService2010();
            CurrentService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //set the credentials
            string UserReportID = ConfigurationManager.AppSettings["userReportID"];
            string UerReportPassword = ConfigurationManager.AppSettings["userReportPassword"];
            string UserReportDomain = ConfigurationManager.AppSettings["userReportDomain"];

            CurrentService.Credentials = new NetworkCredential(UserReportID, UerReportPassword, UserReportDomain);

            byte[] reportDefinition = CurrentService.GetItemDefinition("/"+ reportToPublish.ReportGroup.PathName  + reportToPublish.Name);

            CatalogItem myItem = CurrentService.CreateCatalogItem("Report", reportName, "/" + reportPath, true, reportDefinition, null, out warnings);
            //test if create worked OK
            if (warnings == null)
            {
                DataSource[] dsArray = CurrentService.GetItemDataSources("/" + reportPath + "/" + reportName);
                DataSourceReference dsRef = new DataSourceReference();
                dsRef.Reference = "/" + dataSourceName;
                dsArray[0].Item = dsRef;
                CurrentService.SetItemDataSources("/" + reportPath + "/" + reportName, dsArray);
                return true;
            }
            else
            {
                return false;
            }

        }

        public bool InsertFolder(ReportGroup folder)
        {
            ReportingService2010.ReportingService2010 CurrentService = new ReportingService2010.ReportingService2010();
            CurrentService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //set the credentials
            string UserReportID = ConfigurationManager.AppSettings["userReportID"];
            string UerReportPassword = ConfigurationManager.AppSettings["userReportPassword"];
            string UserReportDomain = ConfigurationManager.AppSettings["userReportDomain"];

            string ParentPath = folder.ParentPath;
            if (ParentPath.Substring(ParentPath.Length - 1) == "/")
            {
                ParentPath = ParentPath.Substring(0, ParentPath.Length - 1);
            }
            CurrentService.Credentials = new NetworkCredential(UserReportID, UerReportPassword, UserReportDomain);
            CatalogItem newFolder = CurrentService.CreateFolder(folder.Name, "/" + ParentPath, null);
            return true;
        }

        public bool DeleteFolder(ReportGroup folder)
        {
            ReportingService2010.ReportingService2010 CurrentService = new ReportingService2010.ReportingService2010();
            CurrentService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //set the credentials
            string UserReportID = ConfigurationManager.AppSettings["userReportID"];
            string UerReportPassword = ConfigurationManager.AppSettings["userReportPassword"];
            string UserReportDomain = ConfigurationManager.AppSettings["userReportDomain"];

            string PathName = folder.PathName;
            if (PathName.Substring(PathName.Length - 1) == "/")
            {
                PathName = PathName.Substring(0, PathName.Length - 1);
            }
            CurrentService.Credentials = new NetworkCredential(UserReportID, UerReportPassword, UserReportDomain);
            CurrentService.DeleteItem("/" + PathName);
            return true;
        }

        public bool RenameFolder(ReportGroup folder,string oldPath)
        {
            ReportingService2010.ReportingService2010 CurrentService = new ReportingService2010.ReportingService2010();
            CurrentService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //set the credentials
            string UserReportID = ConfigurationManager.AppSettings["userReportID"];
            string UerReportPassword = ConfigurationManager.AppSettings["userReportPassword"];
            string UserReportDomain = ConfigurationManager.AppSettings["userReportDomain"];

            string PathName = folder.PathName;
            if (PathName.Substring(PathName.Length - 1) == "/")
            {
                PathName = PathName.Substring(0, PathName.Length - 1);
            }
            string OldPathName = oldPath;
            if (OldPathName.Substring(OldPathName.Length - 1) == "/")
            {
                OldPathName = OldPathName.Substring(0, OldPathName.Length - 1);
            }
            CurrentService.Credentials = new NetworkCredential(UserReportID, UerReportPassword, UserReportDomain);
            CurrentService.MoveItem("/" + OldPathName, "/" + PathName);
            return true;
        }

        public bool DeleteReport(ReportGroup folder,Report reportItem)
        {
            ReportingService2010.ReportingService2010 CurrentService = new ReportingService2010.ReportingService2010();
            CurrentService.Credentials = System.Net.CredentialCache.DefaultCredentials;
            //set the credentials
            string UserReportID = ConfigurationManager.AppSettings["userReportID"];
            string UerReportPassword = ConfigurationManager.AppSettings["userReportPassword"];
            string UserReportDomain = ConfigurationManager.AppSettings["userReportDomain"];

            CurrentService.Credentials = new NetworkCredential(UserReportID, UerReportPassword, UserReportDomain);
            
            CurrentService.DeleteItem("/" + folder.PathName + reportItem.Name);
            return true;
        }
    }
}