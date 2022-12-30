using System;
using System.Linq;
using System.Linq.Expressions;

using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Utilities
{
    /// <summary>
    /// An entity specific expression that holds both the sorting query expression and sorting order.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    /// <typeparam name="TOrderKey">The type of the order key.</typeparam>
    public record struct SortingExpression<TEntity, TOrderKey> : ISortingExpression<TEntity> where TEntity : class
    {
        private readonly Expression<Func<TEntity, TOrderKey>> _expression;
        private readonly bool _descendingOrder;

        public SortingExpression(Expression<Func<TEntity, TOrderKey>> expression, bool descendingOrder = false)
        {
            _expression = expression;
            _descendingOrder = descendingOrder;
        }

        public IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query) => _descendingOrder ? query.OrderByDescending(_expression) : query.OrderBy(_expression);
        public IOrderedQueryable<TEntity> ApplyThenBy(IOrderedQueryable<TEntity> query) => _descendingOrder ? query.ThenByDescending(_expression) : query.ThenBy(_expression);
    }
}
