using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IObjectContext : IDisposable 
    { 
        IObjectSet<T> CreateObjectSet<T>() where T : class;
        int SaveChanges();
    }
}
