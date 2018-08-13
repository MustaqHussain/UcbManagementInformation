using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IDataTableRelationshipJoinRepository : IUcbManagementInformationRepository<DataTableRelationshipJoin>
    {
        DataTableRelationshipJoin GetByCode(Guid code);
        DataTableRelationshipJoin GetByCode(string code);

        List<DataTableRelationshipJoin> GetByDataTableRelationshipCode(Guid code);
        List<DataTableRelationshipJoin> GetByDataTableRelationshipCode(string code);

    }
}
