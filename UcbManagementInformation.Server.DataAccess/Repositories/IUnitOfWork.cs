using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IUnitOfWork : IDisposable
    {
        IObjectContext ObjectContext{get;}
        void Commit();
    }
}
