using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public class ReportGroupRepository : UcbManagementInformationRepository<ReportGroup>,IReportGroupRepository
    {
        public ReportGroupRepository(IObjectContext context)
            : base(context)
        {}

        public IEnumerable<ReportGroup> GetAllReportGroupsForUser(MCUser user)
        {

            var result = (from rg in _objectSet
                          select new
                          {
                              rg,
                              urg = (
                                  from urg in rg.UserReportGroups
                                  where
                                      urg.UserCode == user.Code
                                  select urg)
                          }).AsEnumerable();
            return result.Select(rg => rg.rg);
        }
    }
}
