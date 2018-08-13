using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public class MCUserRepository: UcbManagementInformationRepository<MCUser>,IMCUserRepository
    {
       public MCUserRepository(IObjectContext context)
            : base(context)
        { }

        public List<string> GetAllRoleNamesByUserCode(Guid code)
        {
            List<string> roles = new List<string>();

            var query = base.Find(x => x.Code == code, "UserRoles", "UserRoles.Role");
            foreach (MCUserRole ur in query.FirstOrDefault().UserRoles)
            {
                roles.Add(ur.Role.Name);
            }
            
            return roles;

        }
    }
}
