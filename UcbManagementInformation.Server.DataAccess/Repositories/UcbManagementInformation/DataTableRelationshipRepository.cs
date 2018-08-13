using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{
    public class DataTableRelationshipRepository : UcbManagementInformationRepository<DataTableRelationship>, IDataTableRelationshipRepository
    {
        
        public DataTableRelationshipRepository(IObjectContext context)
            : base(context)
        { }

        public DataTableRelationship GetByCode(Guid code)
        {
            return base.FirstOrDefault(x => x.Code == code);
        }
        public DataTableRelationship GetByCode(string code)
        {
            Guid GuidForFilter = Guid.Parse(code);
            return GetByCode(GuidForFilter);
        }

        public DataTableRelationship GetByTableFromAndTableTo(Guid fromCode, Guid toCode)
        {
            return base.FirstOrDefault(x => x.DataTableFromCode == fromCode && x.DataTableToCode == toCode);
        }

        public DataTableRelationship GetByTableFromAndTableTo(string fromCode, string toCode)
        {
            Guid GuidFromForFilter = Guid.Parse(fromCode);
            Guid GuidToForFilter = Guid.Parse(toCode);
            return GetByTableFromAndTableTo(GuidFromForFilter, GuidToForFilter);
        }
    }
}
