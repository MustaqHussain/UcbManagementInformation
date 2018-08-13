using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IDataTableRepository : IUcbManagementInformationRepository<DataTable>
    {
        DataTable GetByCode(string code);
        DataTable GetByCode(Guid code);
        
    }
}
