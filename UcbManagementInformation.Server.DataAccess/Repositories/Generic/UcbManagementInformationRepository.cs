using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UcbManagementInformation.Server.IoC.ServiceLocation;

namespace UcbManagementInformation.Server.DataAccess
{
    public class UcbManagementInformationRepository<T> : Repository<T>,IUcbManagementInformationRepository<T> where T :  class
    {
       
        public UcbManagementInformationRepository(IObjectContext context)
            : base(context,"UcbManagementInformation")
        { }

        
    
    }
}
