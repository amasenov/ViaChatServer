using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class DbContextExtensions
    {
        public static async Task<List<T>> RawSqlQueryAsync<T>(this DbContext dbContext, string query, Func<DbDataReader, T> map, bool closeConnection = false)
        {
            using DbCommand command = dbContext.Database.GetDbConnection().CreateCommand();
            command.CommandText = query;
            command.CommandType = CommandType.Text;

            await dbContext.Database.OpenConnectionAsync();

            using DbDataReader result = await command.ExecuteReaderAsync();

            List<T> entities = new();
            while (await result.ReadAsync())
            {
                entities.Add(map(result));
            }

            if(closeConnection)
            {
                await dbContext.Database.CloseConnectionAsync();
            }

            return entities;
        }

        public static async Task CloseConnectionAsync(this DbContext dbContext)
        {
            await dbContext.Database.CloseConnectionAsync();
        }
    }
}
