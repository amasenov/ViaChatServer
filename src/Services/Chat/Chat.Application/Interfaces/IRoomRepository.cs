using Chat.Domain.Entities;

using System;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace Chat.Application.Interfaces
{
    public interface IRoomRepository
    {
        IQueryable<Room> CreateBaseQuery(EntityOptions<Room> options);
        Task<PagedResponse<Room>> GetEntitiesAsync(PagingOptions pagingOptions, EntityOptions<Room> options);
        Task<Room> GetEntityAsync(EntityOptions<Room> options);
        Task<(bool isSuccess, Exception exception)> TryAddAsync(Room entity);
        Task<(bool isSuccess, Exception exception)> TryUpdateAsync(Room entity);
        Task<(bool isSuccess, Exception exception)> TryDeleteAsync(Room entity);
        Task<bool> HandleValidationAsync(Expression<Func<Room, bool>> expression,
                                         HttpRequestException exception,
                                         bool handleError = true,
                                         bool validValue = false,
                                         bool excludeTracking = true,
                                         bool ignoreQueryFilters = false);
    }
}
