using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Repositories
{
    public record UnitOfWork<TContext> : IDisposable, IUnitOfWork where TContext : DbContext
    {
        private bool _disposed = false;
        private readonly TContext _context;

        public UnitOfWork(TContext context)
        {
            _context = context;
        }

        public async Task<List<T>> ExecuteRawSqlQueryAsync<T>(string query, Func<DbDataReader, T> map, bool closeConnection = false)
        {
            return await _context.RawSqlQueryAsync<T>(query, map, closeConnection);
        }

        public DbSet<TEntity> Set<TEntity>() where TEntity : class
        {
            return _context.Set<TEntity>();
        }

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class
        {
            return _context.Entry<TEntity>(entity);
        }

        public async Task SaveAsync(CancellationToken cancellationToken = new CancellationToken())
        {
            //foreach (var entry in _context.ChangeTracker.Entries<BaseEntity>())
            //{
            //    if (entry.State == EntityState.Added)
            //    {
            //        entry.Entity.Created = DateTime.UtcNow;
            //        entry.Entity.Modified = DateTime.UtcNow;
            //    }
            //    if (entry.State == EntityState.Modified)
            //    {
            //        entry.Entity.Modified = DateTime.UtcNow;
            //    }
            //}

            await _context.SaveChangesAsync(cancellationToken);
        }

        public void Save()
        {
            _context.SaveChanges();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        public ChangeTracker ChangeTracker<TEntity>(TEntity entity) where TEntity : class
        {
            return _context.ChangeTracker;
        }
    }
}
