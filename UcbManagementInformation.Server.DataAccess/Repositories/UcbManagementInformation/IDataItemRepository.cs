using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IDataItemRepository : IUcbManagementInformationRepository<DataItem>
    {
        DataItem GetByCode(Guid code);
        DataItem GetByCode(string code);

        List<DataItem> GetByTableCode(Guid code);
        List<DataItem> GetByTableCode(string code);

    }
}
