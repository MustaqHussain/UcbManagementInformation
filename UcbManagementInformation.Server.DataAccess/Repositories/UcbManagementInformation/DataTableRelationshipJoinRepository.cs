using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{
    public class DataTableRelationshipJoinRepository : UcbManagementInformationRepository<DataTableRelationshipJoin>, IDataTableRelationshipJoinRepository
    {
        
        public DataTableRelationshipJoinRepository(IObjectContext context)
            : base(context)
        { }

        public DataTableRelationshipJoin GetByCode(Guid code)
        {
            return base.FirstOrDefault(x => x.Code == code);
        }
        public DataTableRelationshipJoin GetByCode(string code)
        {
            Guid GuidForFilter = Guid.Parse(code);
            return GetByCode(GuidForFilter);
        }

        public List<DataTableRelationshipJoin> GetByDataTableRelationshipCode(Guid dtrCode)
        {
            return base.Find(x => x.DataTableRelationshipCode == dtrCode).ToList<DataTableRelationshipJoin>();
        }
        public List<DataTableRelationshipJoin> GetByDataTableRelationshipCode(string dtrCode)
        {
            Guid GuidForFilter = Guid.Parse(dtrCode);
            
            return GetByDataTableRelationshipCode(dtrCode);
        }
    }
}
