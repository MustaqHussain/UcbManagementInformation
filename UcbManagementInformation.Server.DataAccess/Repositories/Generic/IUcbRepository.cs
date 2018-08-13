using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IUcbRepository<T> : IRepository<T> where T : class
    {
        
    }
}
