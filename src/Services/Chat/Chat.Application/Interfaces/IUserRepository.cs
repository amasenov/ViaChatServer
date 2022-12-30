using Chat.Domain.Entities;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Http;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace Chat.Application.Interfaces
{
    public interface IUserRepository
    {
        IQueryable<User> CreateBaseQuery(EntityOptions<User> options);
        Task<PagedResponse<User>> GetEntitiesAsync(PagingOptions pagingOptions, EntityOptions<User> options);
        Task<List<User>> GetEntitiesAsync(EntityOptions<User> options);
        Task<User> GetEntityAsync(EntityOptions<User> options);
        Task<(bool isSuccess, Exception exception)> TryAddAsync(User entity);
        Task<(bool isSuccess, Exception exception)> TryUpdateAsync(User entity);
        Task<(bool isSuccess, Exception exception)> TryDeleteAsync(User entity);
        Task<bool> HandleValidationAsync(Expression<Func<User, bool>> expression,
                                         HttpRequestException exception,
                                         bool handleError = true,
                                         bool validValue = false,
                                         bool excludeTracking = true,
                                         bool ignoreQueryFilters = false);
    }
}
