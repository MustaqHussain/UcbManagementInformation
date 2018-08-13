using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess.Repositories
{
    public class NullObjectContext : IObjectContext
    {
        
    
        IObjectSet<T>  IObjectContext.CreateObjectSet<T>()
        {
 	        throw new NotImplementedException();
        }

        public int  SaveChanges()
        {
 	        throw new NotImplementedException();
        }

        public void  Dispose()
        {
 	        throw new NotImplementedException();
        }
    }
}
