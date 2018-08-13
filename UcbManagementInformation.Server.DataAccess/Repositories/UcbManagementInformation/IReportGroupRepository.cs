using System;
using System.Collections.Generic;
namespace UcbManagementInformation.Server.DataAccess
{
    public interface IReportGroupRepository : IUcbManagementInformationRepository<ReportGroup>
    {
        IEnumerable<ReportGroup> GetAllReportGroupsForUser(MCUser user);
    }
}
