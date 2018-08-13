using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data.Objects.DataClasses;

namespace UcbManagementInformation.Server.DataAccess
{
    public static class ICollectionExtensions
    {

        public static ObjectQuery<T> Attach<T>(this ICollection<T> iCollection) where T : class
        {
            EntityCollection<T> entityCollection = iCollection as EntityCollection<T>;
            if (entityCollection == null)
                throw new ArgumentNullException();
            return entityCollection.Attach();
        }

        public static void Attach<T>(this ICollection<T> iCollection, T entity) where T : class
        {
            EntityCollection<T> entityCollection = iCollection as EntityCollection<T>;
            if (entityCollection == null || entity == null)
                throw new ArgumentNullException();
            entityCollection.Attach(entity);
        }

        public static void Attach<T>(this ICollection<T> iCollection, IEnumerable<T> entities) where T : class
        {
            EntityCollection<T> entityCollection = iCollection as EntityCollection<T>;
            if (entityCollection == null || entities == null)
                throw new ArgumentNullException();
            entityCollection.Attach(entities);
        }

        public static ObjectQuery<T> CreateSourceQuery<T>(this ICollection<T> iCollection) where T : class
        {
            EntityCollection<T> entityCollection = iCollection as EntityCollection<T>;
            if (entityCollection == null)
                throw new ArgumentNullException();
            return entityCollection.CreateSourceQuery();
        }
    }
}
