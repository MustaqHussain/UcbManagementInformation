using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Data.Objects;
using System.Data;
using System.Data.Objects.DataClasses;
using System.ServiceModel.DomainServices.EntityFramework;

namespace UcbManagementInformation.Server.DataAccess
{
    public static class IObjectSetExtensions
    {
        public static void AttachAsModified<T>(this IObjectSet<T> iObjectSet, T entity,bool isUseless) where T : class
        {
            ObjectSet<T> objectSet = iObjectSet as ObjectSet<T>;
            if (objectSet == null || entity == null)
                throw new ArgumentNullException();
            objectSet.AttachAsModified(entity);
        }
        public static void AttachAsModified<T>(this IObjectSet<T> iObjectSet, T entity, T original, bool isUseless) where T : class
        {
            ObjectSet<T> objectSet = iObjectSet as ObjectSet<T>;
            if (objectSet == null || entity == null)
                throw new ArgumentNullException();
            objectSet.AttachAsModified(entity,original);
        }

        public static void AttachAllAsModified<T>(this IObjectSet<T> iObjectSet, IEnumerable<T> entities) where T : class
        {
            ObjectSet<T> objectSet = iObjectSet as ObjectSet<T>;
            if (objectSet == null || entities == null)
                throw new ArgumentNullException();
            objectSet.AttachAllAsModified(entities);
        }

    }
}
