using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Linq.Expressions;
using System.Data.Entity;

namespace UcbManagementInformation.Server.DataAccess
{
    public static class ObjectQueryExtensions
    {
        public static IQueryable<T> Include<T>(this IQueryable<T> source, string path) where T : class
        {
            var objectQuery = source as ObjectQuery<T>;
            if (objectQuery != null)
            {
                return objectQuery.Include(path);
            }

            return source;
        }

        public static IQueryable<T> IncludeMultiple<T>(this IQueryable<T> query, params Expression<Func<T, object>>[] includes) where T : class
        {
            if (includes != null)
            {
                query = includes.Aggregate(query,
                    (current, include) => current.Include(include));
            }
            return query;
        } 
    }
}
