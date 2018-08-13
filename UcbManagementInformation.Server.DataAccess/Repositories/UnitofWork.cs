using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using UcbManagementInformation.Server.IoC.ServiceLocation;

namespace UcbManagementInformation.Server.DataAccess
{
    public class UnitOfWork : IUnitOfWork, IDisposable 
    { 
        private readonly IObjectContext _objectContext;

        //DH Added temporary paramaterless constructor to generate the objectContext.
        //In future this could be done via dependency injection using a suitable IoC container like Unity.
        public UnitOfWork(string entityFrameWorkContextName)
        {
            //_objectContext = (IObjectContext)new UcbManagementInformationEntities();
            
                // Get context based upon repository
            _objectContext = SimpleServiceLocator.Instance.Get<IObjectContext>(entityFrameWorkContextName);
           

            
        }
        public UnitOfWork(IObjectContext objectContext) 
        {
            _objectContext = objectContext;
        }

        public IObjectContext ObjectContext
        {
            get { return _objectContext; }
        }
        public void Dispose() 
        { 
            if (_objectContext != null) 
            { 
                _objectContext.Dispose(); 
            }
            GC.SuppressFinalize(this);
        }
        
        public void Commit() 
        {
            _objectContext.SaveChanges(); 
        }
    }
}
