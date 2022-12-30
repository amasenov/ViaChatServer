using System.Linq;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Interfaces
{
    /// <summary>
    /// The interface of a sorting expression that can be applied to a specific entity.
    /// </summary>
    /// <typeparam name="TEntity">The type of the entity.</typeparam>
    public interface ISortingExpression<TEntity> where TEntity : class
    {
        /// <summary>
        /// Applies the orderBy to the specified queryable.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>An ordered querably.</returns>
        IOrderedQueryable<TEntity> ApplyOrderBy(IQueryable<TEntity> query);

        /// <summary>
        /// Applies the orderBy on an already ordered queryable.
        /// </summary>
        /// <param name="query">The query.</param>
        /// <returns>An ordered querably.</returns>
        IOrderedQueryable<TEntity> ApplyThenBy(IOrderedQueryable<TEntity> query);
    }
}
