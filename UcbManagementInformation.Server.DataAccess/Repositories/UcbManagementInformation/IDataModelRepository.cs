using System;
using UcbManagementInformation.Server.DataAccess;
namespace UcbManagementInformation.Server.DataAccess
{
    public interface IDataModelRepository : IUcbManagementInformationRepository<DataModel>
    {
        DataModel GetByCode(Guid code);
        DataModel GetByCode(string code);
    }
}
