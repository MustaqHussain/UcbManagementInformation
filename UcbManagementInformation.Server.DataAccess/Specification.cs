using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Linq.Expressions;

namespace UcbManagementInformation.Server.DataAccess
{
    public class Specification<TEntity> : ISpecification<TEntity>
    {
        public Specification(Expression<Func<TEntity, bool>> predicate)
        {
            Predicate = predicate;
        }

        public Specification<TEntity> And(Specification<TEntity> specification) 
        {
            return new Specification<TEntity>(this.Predicate.And(specification.Predicate));
        }
        
        public Specification<TEntity> And(Expression<Func<TEntity, bool>> predicate) 
        {
            return new Specification<TEntity>(this.Predicate.And(predicate));
        }
        public Specification<TEntity> Or(Specification<TEntity> specification) 
        {
            return new Specification<TEntity>(this.Predicate.Or(specification.Predicate));
        }
        public Specification<TEntity> Or(Expression<Func<TEntity, bool>> predicate)
        {
            return new Specification<TEntity>(this.Predicate.Or(predicate));
        }
        public TEntity SatisfyingEntityFrom(IQueryable<TEntity> query) 
        { 
            return query.Where(Predicate).SingleOrDefault();
        }
        public IQueryable<TEntity> SatisfyingEntitiesFrom(IQueryable<TEntity> query) 
        {
            return query.Where(Predicate);
        }
        public Expression<Func<TEntity, bool>> Predicate 
        {
            get; set;
        }
    }

    public class ParameterRebinder : ExpressionVisitor
    {
        private readonly Dictionary<ParameterExpression, ParameterExpression> map;


        public ParameterRebinder(Dictionary<ParameterExpression, ParameterExpression> map)
        {
            this.map = map ?? new Dictionary<ParameterExpression, ParameterExpression>();
        }


        public static Expression ReplaceParameters(Dictionary<ParameterExpression, ParameterExpression> map, Expression exp)
        {
            return new ParameterRebinder(map).Visit(exp);
        }


        protected override Expression VisitParameter(ParameterExpression p)
        {
            ParameterExpression replacement;
            if (map.TryGetValue(p, out replacement))
            {
                p = replacement;
            }
            return base.VisitParameter(p);
        }
    }

    public static class ExpressionExtension
    {
        public static Expression<T> Compose<T>(this Expression<T> first, Expression<T> second, Func<Expression, Expression, Expression> merge)
        {
            // build parameter map (from parameters of second to parameters of first)
            var map = first.Parameters.Select((f, i) => new { f, s = second.Parameters[i] }).ToDictionary(p => p.s, p => p.f);

            // replace parameters in the second lambda expression with parameters from the first
            var secondBody = ParameterRebinder.ReplaceParameters(map, second.Body);

            // apply composition of lambda expression bodies to parameters from the first expression
            return Expression.Lambda<T>(merge(first.Body, secondBody), first.Parameters);
        }

        public static Expression<Func<T, bool>> And<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.And);
        }

        public static Expression<Func<T, bool>> Or<T>(this Expression<Func<T, bool>> first, Expression<Func<T, bool>> second)
        {
            return first.Compose(second, Expression.Or);
        }
    }

    


    

}
