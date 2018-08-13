using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IDataTableJoinRepository : IUcbManagementInformationRepository<DataTableJoin>
    {
        DataTableJoin GetByCode(Guid code);
        DataTableJoin GetByCode(string code);
    }
}
