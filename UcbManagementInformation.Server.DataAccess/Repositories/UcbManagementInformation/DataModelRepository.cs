using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public class DataModelRepository : UcbManagementInformationRepository<DataModel>, IDataModelRepository
    {
        public DataModelRepository(IObjectContext context)
            : base(context)
        { }
        public DataModel GetByCode(string code)
        {
            return GetByCode(Guid.Parse(code));
        }

        public DataModel GetByCode(Guid code)
        {
            return this.FirstOrDefault(x => x.Code == code);
        }
    }
}
