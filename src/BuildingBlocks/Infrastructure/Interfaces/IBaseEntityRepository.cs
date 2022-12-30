using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Entities;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Interfaces
{
    internal interface IBaseEntityRepository<TEntity> where TEntity  : BaseEntity
    {
        Task<List<T>> ExecuteRawSqlQueryAsync<T>(string query, Func<DbDataReader, T> map, bool closeConnection = false);
        IQueryable<TEntity> FromSqlRaw(string query);
        TEntity Find(int id);

        IQueryable<TEntity> CreateBaseQuery(EntityOptions<TEntity> options);
        PagedResponseQuery<TEntity> CreatePagedResponse(PagingOptions pagingOptions, EntityOptions<TEntity> options);
        Task<List<TEntity>> GetEntitiesAsync(EntityOptions<TEntity> options);
        Task<PagedResponse<TEntity>> GetEntitiesAsync(PagingOptions pagingOptions, EntityOptions<TEntity> options);
        Task<TEntity> GetEntityAsync(EntityOptions<TEntity> options);

        Task<(bool isSuccess, Exception exception)> TryAddAsync(TEntity entity);
        Task<(bool isSuccess, Exception exception)> TryUpdateAsync(TEntity entity);
        Task<(bool isSuccess, Exception exception)> TryDeleteAsync(TEntity entity);
        Task<(bool isSuccess, Exception exception)> TryRemoveAsync(TEntity entity);

        Task AddAsync(TEntity entity);
        Task UpdateAsync(TEntity entity);
        Task DeleteAsync(TEntity entity);
        Task RemoveAsync(TEntity entity);

        Task AddRangeAsync(IEnumerable<TEntity> entities);
        Task UpdateRangeAsync(IEnumerable<TEntity> entities);
        Task DeleteRangeAsync(IEnumerable<TEntity> entities);
        Task RemoveRangeAsync(IEnumerable<TEntity> entities);

        Task AddEntityAsync(TEntity entity);
        Task UpdateEntityAsync(TEntity entity);
        Task RemoveEntityAsync(TEntity entity);

        void UpdateEntityState(TEntity entity, EntityState state);

        Task SaveAsync(CancellationToken cancellationToken = new CancellationToken());
        void Save();
    }
}
