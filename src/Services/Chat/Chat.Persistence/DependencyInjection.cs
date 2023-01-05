using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

using System;
using System.Reflection;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Repositories;

namespace Chat.Persistence
{
    public static class DependencyInjection
    {

        public static readonly LoggerFactory _myLoggerFactory =
                new LoggerFactory(new[] {
                    new Microsoft.Extensions.Logging.Debug.DebugLoggerProvider()
                });

        public static IServiceCollection AddPersistence(this IServiceCollection services, IConfiguration configuration)
        {
            var isDebug = false;
#if DEBUG
            isDebug = true;
#endif
            // Using LocalDB
            // https://learn.microsoft.com/en-us/sql/database-engine/configure-windows/sql-server-express-localdb?view=sql-server-ver16
            string connectionString = Environment.GetEnvironmentVariable("ConnectionStrings__ChatDatabaseConnectionString");
            if (connectionString.IsEmpty())
            {
                connectionString = configuration.GetValue<string>("ConnectionStrings__ChatDatabaseConnectionString");
                if (connectionString.IsEmpty())
                {
                    throw new ArgumentNullException(nameof(connectionString));
                }
            }

            services.AddDbContext<ChatDbContext>(options =>
            {
                options
                .UseLazyLoadingProxies(false)
                .EnableDetailedErrors(isDebug)
                .EnableSensitiveDataLogging(isDebug)
                .UseLoggerFactory(_myLoggerFactory)
                .UseSqlServer(connectionString, x =>
                {
                    x.UseQuerySplittingBehavior(QuerySplittingBehavior.SingleQuery);

                    x.MigrationsAssembly(Assembly.GetExecutingAssembly().GetName().Name);
                    x.MigrationsHistoryTable($"__{nameof(ChatDbContext)}");

                    x.EnableRetryOnFailure(
                        maxRetryCount: 10,
                        maxRetryDelay: TimeSpan.FromSeconds(30),
                        errorNumbersToAdd: null);
                });
            });
            services.AddScoped<IUnitOfWork, UnitOfWork<ChatDbContext>>();

            return services;
        }
    }
}
