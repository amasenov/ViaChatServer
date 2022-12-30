using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Entities;
using ViaChatServer.BuildingBlocks.Infrastructure.Exceptions;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Repositories
{
    public record BaseEntityRepository<TEntity> : IBaseEntityRepository<TEntity> where TEntity : BaseEntity
    {
        private readonly IUnitOfWork _unitOfWork;

        protected DbSet<TEntity> _entity;

        public BaseEntityRepository(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;

            _entity = unitOfWork.Set<TEntity>();
        }

        public async Task<List<T>> ExecuteRawSqlQueryAsync<T>(string query, Func<DbDataReader, T> map, bool closeConnection = false)
        {
            return await _unitOfWork.ExecuteRawSqlQueryAsync<T>(query, map, closeConnection);
        }

        public IQueryable<TEntity> FromSqlRaw(string query)
        {
            return _entity.FromSqlRaw<TEntity>(query);
        }

        public virtual TEntity Find(int id)
        {
            // Here we are working with a DbContext, not specific DbContext.   
            // So we don't have DbSets we need to use the generic Set() method to access them.  
            return _entity.Find(id);
        }

        public IQueryable<TEntity> CreateBaseQuery(EntityOptions<TEntity> options = null)
        {
            options ??= new();

            IQueryable<TEntity> query = _entity;
            if (options.IgnoreQueryFilters)
            {
                query = query.IgnoreQueryFilters();
            }

            if (options.ExcludeTracking)
            {
                query = query.AsNoTracking();
            }

            if (options.ExcludeTrackingWithIdentityResolution)
            {
                query = query.AsNoTrackingWithIdentityResolution();
            }

            if (options.UseSingleQuery)
            {
                query = query.AsSingleQuery();
            }
            else
            {
                query = query.AsSplitQuery();
            }

            if (options.Filters.HasValue())
            {
                foreach (var filter in options.Filters)
                {
                    query = query.Where(filter);
                }
            }

            if (options.Sortings.HasValues())
            {
                query = query.Sort(options.Sortings);
            }

            if (options.Includes.HasValue())
            {
                foreach (var include in options.Includes)
                {
                    query = include(query);
                }
            }

            if (options.Projection.HasValue())
            {
                query = query.Select(options.Projection);
            }

            return query;
        }
        public PagedResponseQuery<TEntity> CreatePagedResponse(PagingOptions pagingOptions, EntityOptions<TEntity> options)
        {
            var query = CreateBaseQuery(options);

            int totalResults = query.Count();
            query = query.Paginate(pagingOptions);

            return new PagedResponseQuery<TEntity>(pagingOptions.PageNumber, pagingOptions.PageSize, totalResults)
            {
                Query = query
            };
        }

        public async Task<List<TEntity>> GetEntitiesAsync(EntityOptions<TEntity> options)
        {
            var query = this.CreateBaseQuery(options);

            return await query.ToListAsync();
        }

        public async Task<PagedResponse<TEntity>> GetEntitiesAsync(PagingOptions pagingOptions, EntityOptions<TEntity> options)
        {
            try
            {
                var result = this.CreatePagedResponse(pagingOptions, options);

                return new PagedResponse<TEntity>(result.PageNumber, result.PageSize, result.Total)
                {
                    Results = await result.Query.ToListAsync()
                };
            }
            catch (Exception ex)
            {
                // TODO: Add error logging
                _ = ex.Message;

                return null;
            }
        }
        public async Task<TEntity> GetEntityAsync(EntityOptions<TEntity> options)
        {
            try
            {
                var query = this.CreateBaseQuery(options);

                var entity = await query.FirstOrDefaultAsync();
                if (entity.HasValue() || !options.HandleError)
                {
                    return entity;
                }
            }
            catch (Exception ex)
            {
                // TODO: Add error logging
                _ = ex.Message;
            }

            return options.HandleError ? throw new EntityNotFoundException($"{typeof(TEntity).Name}Id") : null;
        }

        #region Enity add/update/remove methods
        public async Task<(bool isSuccess, Exception exception)> TryAddAsync(TEntity entity)
        {
            try
            {
                var utcNow = DateTime.UtcNow;
                entity.Created = utcNow;
                entity.Modified = utcNow;
                _entity.Add(entity);

                await SaveAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }
        public async Task<(bool isSuccess, Exception exception)> TryUpdateAsync(TEntity entity)
        {
            try
            {
                entity.Modified = DateTime.UtcNow;
                _entity.Update(entity);
                await SaveAsync();

                return (true, null);
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                var (isResolved, ex) = await TryResolveConcurrencyAsync(dbEx);
                return (isResolved, ex);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }
        public async Task<(bool isSuccess, Exception exception)> TryDeleteAsync(TEntity entity)
        {
            try
            {
                entity.Modified = DateTime.UtcNow;
                // Soft deleting the device
                entity.Deleted = true;
                _entity.Update(entity);

                await SaveAsync();

                return (true, null);
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                var (isResolved, ex) = await TryResolveConcurrencyAsync(dbEx);
                return (isResolved, ex);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }
        public async Task<(bool isSuccess, Exception exception)> TryRemoveAsync(TEntity entity)
        {
            try
            {
                _entity.Remove(entity);

                await SaveAsync();

                return (true, null);
            }
            catch (Exception ex)
            {
                return (false, ex);
            }
        }

        public async Task AddAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddAsync)} {typeof(TEntity).Name} entity must not be null");
            }

            try
            {
                var utcNow = DateTime.UtcNow;
                entity.Created = utcNow;
                entity.Modified = utcNow;

                _entity.Add(entity);

                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} could not be added", ex);
            }
        }
        public async Task UpdateAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateAsync)} {typeof(TEntity).Name} entity must not be null");
            }

            try
            {
                entity.Modified = DateTime.UtcNow;

                _entity.Update(entity);

                await SaveAsync();
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                var (isResolved, ex) = await TryResolveConcurrencyAsync(dbEx);
                if (!isResolved)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} could not be updated", ex);
            }
        }
        public async Task DeleteAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(DeleteAsync)} {typeof(TEntity).Name} entity must not be null");
            }

            try
            {
                entity.Modified = DateTime.UtcNow;
                // Soft deleting the device
                entity.Deleted = true;

                _entity.Update(entity);

                await SaveAsync();
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                var (isResolved, ex) = await TryResolveConcurrencyAsync(dbEx);
                if (!isResolved)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} could not be deleted", ex);
            }
        }
        public async Task RemoveAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(RemoveAsync)} {typeof(TEntity).Name} entity must not be null");
            }

            try
            {
                _entity.Remove(entity);

                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} could not be removed", ex);
            }
        }

        public async Task AddRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"{nameof(AddRangeAsync)} {typeof(TEntity).Name} entities must not be null");
            }

            try
            {
                var entries = entities.ToArray();
                await _entity.AddRangeAsync(entries);

                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} entities could not be added", ex);
            }
        }

        public async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateRangeAsync)} {typeof(TEntity).Name} entities must not be null");
            }

            try
            {
                var entries = entities.ToArray();
                _entity.UpdateRange(entities);

                await SaveAsync();
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                var (isResolved, ex) = await TryResolveRangeConcurrencyAsync(dbEx);
                if (!isResolved)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} entities could not be updated", ex);
            }
        }

        public async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"{nameof(DeleteRangeAsync)} {typeof(TEntity).Name} entities must not be null");
            }

            try
            {
                var utcNow = DateTime.UtcNow;
                foreach(var entity in entities)
                {
                    entity.Deleted = true;
                    entity.Modified = utcNow;
                }

                var entries = entities.ToArray();
                _entity.UpdateRange(entities);

                await SaveAsync();
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                var (isResolved, ex) = await TryResolveRangeConcurrencyAsync(dbEx);
                if (!isResolved)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} entities could not be deleted", ex);
            }
        }

        public async Task RemoveRangeAsync(IEnumerable<TEntity> entities)
        {
            if (entities == null)
            {
                throw new ArgumentNullException($"{nameof(RemoveRangeAsync)} {typeof(TEntity).Name} entities must not be null");
            }

            try
            {
                var entries = entities.ToArray();
                _entity.RemoveRange(entries);

                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} entities could not be removed", ex);
            }
        }

        // TODO: Remove redundant functions we are not using. Choose the best implementation.
        public virtual async Task AddEntityAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(AddEntityAsync)} {typeof(TEntity).Name} entity must not be null");
            }

            try
            {
                if (_unitOfWork.Entry<TEntity>(entity).State == EntityState.Detached)
                {
                    _entity.Attach(entity);
                }

                _entity.Add(entity);

                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} could not be added", ex);
            }
        }

        public virtual async Task UpdateEntityAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(UpdateEntityAsync)} {typeof(TEntity).Name} entity must not be null");
            }

            try
            {
                if (_unitOfWork.Entry<TEntity>(entity).State == EntityState.Detached)
                {
                    _entity.Attach(entity);
                }

                _entity.Update(entity);

                await SaveAsync();
            }
            catch (DbUpdateConcurrencyException dbEx)
            {
                var (isResolved, ex) = await TryResolveConcurrencyAsync(dbEx);
                if (!isResolved)
                {
                    throw;
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} could not be updated", ex);
            }
        }

        public virtual async Task RemoveEntityAsync(TEntity entity)
        {
            if (entity == null)
            {
                throw new ArgumentNullException($"{nameof(RemoveEntityAsync)} {typeof(TEntity).Name} entity must not be null");
            }

            try
            {
                if (_unitOfWork.Entry<TEntity>(entity).State == EntityState.Detached)
                {
                    _entity.Attach(entity);
                }

                _entity.Remove(entity);

                await SaveAsync();
            }
            catch (Exception ex)
            {
                throw new Exception($"{typeof(TEntity).Name} could not be removed", ex);
            }
        }

        public virtual void UpdateEntityState(TEntity entity, EntityState state)
        {
            _unitOfWork.Entry<TEntity>(entity).State = state;
        }
        #endregion

        public virtual async Task SaveAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            await _unitOfWork.SaveAsync(cancellationToken);
        }

        public virtual void Save()
        {
            _unitOfWork.Save();
        }

        public virtual void ClearTrackedEntities(TEntity trackedEntity)
        {
            _unitOfWork.ChangeTracker<TEntity>(trackedEntity).Clear();
        }

        private async Task<(bool isResolved, DbUpdateConcurrencyException ex)> TryResolveConcurrencyAsync(DbUpdateConcurrencyException dbEx)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    EntityEntry entry = dbEx.Entries.FirstOrDefault();
                    if (entry.HasValue())
                    {
                        // Update original values from the database
                        entry.OriginalValues.SetValues(entry.GetDatabaseValues());

                        await this.SaveAsync();
                        return (true, null);
                    }
                }
                catch
                {
                }
            }

            return (false, dbEx);
        }
        private async Task<(bool isResolved, DbUpdateConcurrencyException ex)> TryResolveRangeConcurrencyAsync(DbUpdateConcurrencyException dbEx)
        {
            for (int i = 0; i < 3; i++)
            {
                try
                {
                    if (dbEx.Entries.HasValues())
                    {
                        foreach (var entry in dbEx.Entries)
                        {
                            if (entry.HasValue())
                            {
                                // Update original values from the database
                                entry.OriginalValues.SetValues(entry.GetDatabaseValues());
                            }
                        }

                        await this.SaveAsync();
                        return (true, null);
                    }
                }
                catch
                {
                }
            }

            return (false, dbEx);
        }
    }
}
