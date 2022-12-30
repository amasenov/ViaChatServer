using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class QueryableExtentions
    {
        public static IQueryable<TEntity> Sort<TEntity>(this IQueryable<TEntity> query, IEnumerable<ISortingExpression<TEntity>> sortings = null) where TEntity : class
        {
            if (!sortings.HasValues())
            {
                return query;
            }

            IOrderedQueryable<TEntity> orderedQuery = null;
            foreach (var sorting in sortings)
            {
                if (orderedQuery.IsNull())
                {
                    orderedQuery = sorting.ApplyOrderBy(query);
                }
                else
                {
                    orderedQuery = sorting.ApplyThenBy(orderedQuery);
                }
            }

            return orderedQuery;
        }

        public static IQueryable<TEntity> Paginate<TEntity>(this IQueryable<TEntity> query, PagingOptions pagingOptions) where TEntity : class => (pagingOptions != null) ? query.Skip(pagingOptions.Skip).Take(pagingOptions.PageSize) : throw new ArgumentNullException(nameof(pagingOptions));
        public static Task<TSource> AggregateAsync<TSource>(this IQueryable<TSource> source, Expression<Func<TSource, TSource, TSource>> func, CancellationToken cancellationToken = default) => Task.Run(() => source.Aggregate(func), cancellationToken);
    }
}
