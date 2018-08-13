using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IUcbManagementInformationRepository<T> : IRepository<T> where T : class
    {
        
    }
}
