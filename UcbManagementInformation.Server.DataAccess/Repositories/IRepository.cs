using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace UcbManagementInformation.Server.DataAccess
{
    public interface IRepository<T> where T : class 
    { 
        IQueryable<T> GetQuery();
        IEnumerable<T> GetAll();
        IEnumerable<T> GetAll(params string[] children);
        IEnumerable<T> Find(Expression<Func<T, bool>> filter);
        IEnumerable<T> Find(ISpecification<T> specification);
        IEnumerable<T> Find(Expression<Func<T, bool>> filter, params string[] children);
        T Single(Expression<Func<T, bool>> filter);
        T Single(ISpecification<T> specification);
        T Single(Expression<Func<T, bool>> filter, params string[] children);
        T First(Expression<Func<T, bool>> filter);
        T First(Expression<Func<T, bool>> filter, params string[] children);
        T FirstOrDefault(Expression<Func<T, bool>> filter);

        void Delete(T entity);
        void Add(T entity);
        void Update(T entity);
        void Attach(T entity); 
        void SaveChanges();}
}
