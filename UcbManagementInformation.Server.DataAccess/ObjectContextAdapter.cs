using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;

namespace UcbManagementInformation.Server.DataAccess
{/*
    // Generic version of ObjectContextAdapter found at
    // http://elegantcode.com/2009/12/15/entity-framework-ef4-generic-repository-and-unit-of-work-prototype/
    // Type instanced on Entity Framework classes derived from ObjectContext
    public class ObjectContextAdapter<Efc> : IObjectContext where Efc : ObjectContext
    {
        readonly Efc _context;

        public ObjectContextAdapter(Efc context) 
        {
            _context = context; 
        } 
        
        public void Dispose() 
        { 
            _context.Dispose(); 
        } 
        
        public IObjectSet<T> CreateObjectSet<T>() where T : class 
        {
            return _context.CreateObjectSet<T>() as IObjectSet<T>; 
        } 
        
        public int SaveChanges() 
        { 
            return _context.SaveChanges(); 
        }
    }*/
}
