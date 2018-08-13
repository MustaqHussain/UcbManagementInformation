using System;
using UcbManagementInformation.Server.DataAccess;
namespace UcbManagementInformation.Web.Reporting
{
    public interface IReportingServices
    {
        bool DeleteFolder(UcbManagementInformation.Server.DataAccess.ReportGroup folder);
        bool DeleteReport(UcbManagementInformation.Server.DataAccess.ReportGroup folder, UcbManagementInformation.Server.DataAccess.Report reportItem);
        bool InsertFolder(UcbManagementInformation.Server.DataAccess.ReportGroup folder);
        bool PublishReport(byte[] reportDefinition, string reportName, string reportPath, string dataSourceName);
        bool CopyReport(Report reportToPublish,StandardReport reportInfo);
        bool RenameFolder(UcbManagementInformation.Server.DataAccess.ReportGroup folder, string oldPath);
    }
}
