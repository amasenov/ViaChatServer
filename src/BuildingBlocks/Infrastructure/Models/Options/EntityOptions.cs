using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore.Query;
using ViaChatServer.BuildingBlocks.Infrastructure.Entities;
using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Models
{
    public record EntityOptions<TEntity> where TEntity : BaseEntity
    {
        public EntityOptions()
        {

        }
        public EntityOptions(List<Expression<Func<TEntity, bool>>> filters = null,
                             List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null,
                             List<ISortingExpression<TEntity>> sortings = null,
                             bool excludeTracking = false,
                             bool ignoreQueryFilters = false,
                             bool useSingleQuery = false,
                             bool handleError = true,
                             Expression<Func<TEntity, TEntity>> projection = null)
        {
            Filters = filters;
            Includes = includes;
            Sortings = sortings;
            ExcludeTracking = excludeTracking;
            IgnoreQueryFilters = ignoreQueryFilters;
            UseSingleQuery = useSingleQuery;
            Projection = projection;
            HandleError = handleError;
        }

        public List<Expression<Func<TEntity, bool>>> Filters { get; init; } = null;
        public List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> Includes { get; init; } = null;
        public List<ISortingExpression<TEntity>> Sortings { get; init; } = null;
        public Expression<Func<TEntity, TEntity>> Projection { get; init; } = null;
        public bool ExcludeTracking { get; init; } = false;
        public bool ExcludeTrackingWithIdentityResolution { get; init; } = false;
        public bool IgnoreQueryFilters { get; init; } = false;
        public bool UseSingleQuery { get; init; } = false;
        public bool HandleError { get; init; } = false;
    }
}
