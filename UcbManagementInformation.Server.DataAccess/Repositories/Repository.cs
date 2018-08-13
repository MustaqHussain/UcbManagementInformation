using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;
using UcbManagementInformation.Server.IoC.ServiceLocation;
using UcbManagementInformation.Server.DataAccess.Repositories;
using System.Data.Objects.DataClasses;
using System.Data;
using System.Data.Entity;
namespace UcbManagementInformation.Server.DataAccess
{
    public abstract class Repository<T> : IRepository<T> where T : class
    {
        protected readonly IObjectContext _objectContext;
        protected readonly IObjectSet<T> _objectSet;
        //DH Added temporary paramaterless constructor to generate the objectContext.
        //In future this could be done via dependency injection using a suitable IoC container like Unity.
        /*public Repository() 
        {
            
                // Get context based upon repository
                
        }*/


        // Temporary change to force use of param-less constructor without breaking loads of code.
        // Change method calls to Repository to use the parameterless constructor.
        protected Repository(IObjectContext objectContext, string EntityFrameworkContextName)
        {
            if (!(objectContext is NullObjectContext))
            {
                _objectContext = objectContext;
                _objectSet = _objectContext.CreateObjectSet<T>();
            }
            else
            {
                _objectContext = SimpleServiceLocator.Instance.Get<IObjectContext>(EntityFrameworkContextName);
                _objectSet = _objectContext.CreateObjectSet<T>();
            }
        }


        public IQueryable<T> GetQuery()
        {
            return _objectSet;
        }

        public IEnumerable<T> GetAll()
        {
            return GetQuery().ToList();
        }
        public IEnumerable<T> GetAll(params string[] children)
        {
            return EagerQuery(children).ToList();
        }

        public IEnumerable<T> Find(ISpecification<T> specification)
        {
            return specification.SatisfyingEntitiesFrom(GetQuery());
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter)
        {
            return _objectSet.Where(filter);
        }

        public IEnumerable<T> Find(Expression<Func<T, bool>> filter, params string[] children)
        {
            return EagerQuery(children).Where(filter);

        }
        public T Single(ISpecification<T> specification)
        {
            return specification.SatisfyingEntityFrom(GetQuery());
        }
        public T Single(Expression<Func<T, bool>> filter)
        {
            return _objectSet.Single(filter);
        }
        public T Single(Expression<Func<T, bool>> filter, params string[] children)
        {
            return EagerQuery(children).Single(filter);
        }
        public T FirstOrDefault(Expression<Func<T, bool>> filter)
        {
            return _objectSet.FirstOrDefault(filter);
        }

        public T First(Expression<Func<T, bool>> filter)
        {
            return _objectSet.First(filter);
        }

        public T First(Expression<Func<T, bool>> filter, params string[] children)
        {
            return EagerQuery(children).First(filter);
        }

        public void Delete(T entity)
        {
            //attach as modified as may already be in the context
            _objectSet.AttachAsModified(entity, true);
            _objectSet.DeleteObject(entity);
        }
        public void Update(T entity)
        {
            _objectSet.AttachAsModified(entity, true);
        }
        public void Add(T entity)
        {
            _objectSet.AddObject(entity);

        }

        public void Attach(T entity)
        {
            _objectSet.Attach(entity);
        }

        public void SaveChanges()
        {
            _objectContext.SaveChanges();
        }


        private IQueryable<T> EagerQuery(params string[] children)
        {
            IQueryable<T> query = (IQueryable<T>)_objectSet; //TODO: untested change.

            foreach (string child in children)
            {
                query = query.Include(child);

            }
            return query;
        }

        

    }
}
