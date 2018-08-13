using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcbManagementInformation.Server.IoC.ServiceLocation;

namespace UcbManagementInformation.Server.DataAccess
{
    public class UcbRepository<T> : Repository<T>,IUcbRepository<T> where T :  class
    {
       
        public UcbRepository(IObjectContext context)
            : base(context,"Ucb")
        { }

        
    
    }
}
