using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.ServiceModel.DomainServices.Server.ApplicationServices;
using System.Runtime.Serialization;

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class MCUser : IMCUser, System.ServiceModel.DomainServices.Server.ApplicationServices.IUser
    {
        private IEnumerable<string> _roles;

        [DataMember]
        public IEnumerable<string> Roles
        {
            get
            {
                return _roles;
            }
            set
            {
                _roles = value;
            }
        }
    }
}
