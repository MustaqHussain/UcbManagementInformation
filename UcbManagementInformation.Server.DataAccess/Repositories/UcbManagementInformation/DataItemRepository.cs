using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{
    public class DataItemRepository : UcbManagementInformationRepository<DataItem>, IDataItemRepository
    {
       
        public DataItemRepository(IObjectContext context)
            : base(context)
        { }

        public DataItem GetByCode(Guid code)
        {
            return base.FirstOrDefault(x => x.Code == code);
        }
        public DataItem GetByCode(string code)
        {
            Guid GuidForFilter = Guid.Parse(code);
            return GetByCode(GuidForFilter);
        }


        public List<DataItem> GetByTableCode(Guid code)
        {
            return base.Find(x => x.DataTableCode == code).ToList<DataItem>();
        }
        public List<DataItem> GetByTableCode(string code)
        {
            Guid GuidForFilter = Guid.Parse(code);
            return GetByTableCode(GuidForFilter);
        }
        
    }
}
