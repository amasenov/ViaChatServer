using Microsoft.EntityFrameworkCore.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Entities;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Interfaces
{
    internal interface IBaseIdEntityRepository<TEntity> where TEntity : BaseIdEntity
    {
        Task<TEntity> GetEntityAsync(Guid resourceId,
                                     List<Expression<Func<TEntity, bool>>> filters = null,
                                     List<Func<IQueryable<TEntity>, IIncludableQueryable<TEntity, object>>> includes = null,
                                     bool excludeTracking = false,
                                     bool ignoreQueryFilters = false,
                                     bool useSingleQuery = false,
                                     bool handleError = true,
                                     Expression<Func<TEntity, TEntity>> projection = null);

        Task<bool> HandleValidationAsync(Expression<Func<TEntity, bool>> expression,
                                         HttpRequestException exception,
                                         bool handleError = true,
                                         bool validValue = false,
                                         bool excludeTracking = true,
                                         bool ignoreQueryFilters = false);
        Task<bool> HandleValidationAsync(List<Expression<Func<TEntity, bool>>> expressions,
                                         HttpRequestException exception,
                                         bool handleError = true,
                                         bool validValue = false,
                                         bool excludeTracking = true,
                                         bool ignoreQueryFilters = false);
    }
}
