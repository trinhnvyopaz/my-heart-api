using MyHeart.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;

namespace MyHeart.Services
{
    public static class QueryableExtensions
    {
        public static IQueryable<T> ApplyOrdering<T>(this IQueryable<T> source, List<ColumnOrder> columnOrders)
        {
            if (columnOrders == null || columnOrders.Count == 0)
                return source;

            var expression = source.Expression;
            foreach (var item in columnOrders)
            {
                var parameter = Expression.Parameter(typeof(T), "param");
                Expression body = parameter;
                foreach (var member in item.PropertyPath.Split('.'))
                {
                    body = Expression.PropertyOrField(body, member);
                }

                var orderName = item.Direction == SortDirection.Ascending ? nameof(Enumerable.OrderBy) : nameof(Enumerable.OrderByDescending);

                expression = Expression.Call(typeof(Queryable), orderName, new Type[] { source.ElementType, body.Type }, expression, Expression.Quote(Expression.Lambda(body, parameter)));

            }

            return source.Provider.CreateQuery<T>(expression);
        }

    }
}
