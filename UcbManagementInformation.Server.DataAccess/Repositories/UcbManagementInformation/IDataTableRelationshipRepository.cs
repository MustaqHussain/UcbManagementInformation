using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IDataTableRelationshipRepository : IUcbManagementInformationRepository<DataTableRelationship>
    {
        DataTableRelationship GetByCode(string code);
        DataTableRelationship GetByCode(Guid code);
        DataTableRelationship GetByTableFromAndTableTo(Guid fromCode, Guid toCode);
        DataTableRelationship GetByTableFromAndTableTo(string fromCode, string toCode);

    }
}
