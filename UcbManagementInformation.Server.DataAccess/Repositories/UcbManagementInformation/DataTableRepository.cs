using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{
    public class DataTableRepository : UcbManagementInformationRepository<DataTable>, IDataTableRepository
    {
        
        public DataTableRepository(IObjectContext context)
            : base(context)
        { }

        public DataTable GetByCode(Guid code)
        {
            return base.FirstOrDefault(x => x.Code == code);
        }
        public DataTable GetByCode(string code)
        {
            Guid GuidForFilter = Guid.Parse(code);
            return GetByCode(GuidForFilter);
        }
        
    }
}
