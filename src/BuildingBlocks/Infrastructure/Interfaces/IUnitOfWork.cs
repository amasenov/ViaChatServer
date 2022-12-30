using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;

using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;
using System.Threading.Tasks;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Interfaces
{
    public interface IUnitOfWork
    {
        Task<List<T>> ExecuteRawSqlQueryAsync<T>(string query, Func<DbDataReader, T> map, bool closeConnection = false);

        DbSet<TEntity> Set<TEntity>() where TEntity : class;

        public EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

        public ChangeTracker ChangeTracker<TEntity>(TEntity entity) where TEntity : class;
        Task SaveAsync(CancellationToken cancellationToken = new CancellationToken());

        void Save();

        void Dispose();
    }
}
