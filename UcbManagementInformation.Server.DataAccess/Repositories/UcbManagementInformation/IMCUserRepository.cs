using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IMCUserRepository : IUcbManagementInformationRepository<MCUser>
    {
        List<string> GetAllRoleNamesByUserCode(Guid code);
    }
}
