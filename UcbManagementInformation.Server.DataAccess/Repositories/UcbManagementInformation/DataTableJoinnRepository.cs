using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{
    public class DataTableJoinRepository : UcbManagementInformationRepository<DataTableJoin>, IDataTableJoinRepository
    {
       
        public DataTableJoinRepository(IObjectContext context)
            : base(context)
        { }

        public DataTableJoin GetByCode(Guid code)
        {
            return base.FirstOrDefault(x => x.Code == code);
        }
        public DataTableJoin GetByCode(string code)
        {
            Guid GuidForFilter = Guid.Parse(code);
            return GetByCode(GuidForFilter);
        }
    }
}
