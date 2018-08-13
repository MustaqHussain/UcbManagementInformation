using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{
    public partial class UcbManagementInformationEntities : IUcbManagementInformationEntities
    {

        System.Data.Objects.IObjectSet<T> IObjectContext.CreateObjectSet<T>()
        {
            return (IObjectSet<T>)this.CreateObjectSet<T>();
        }
        
        
    }
}
