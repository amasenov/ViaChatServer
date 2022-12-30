using Chat.Domain.Entities;

using System;
using System.Linq;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace Chat.Application.Interfaces
{
    public interface IPostRepository
    {
        IQueryable<Post> CreateBaseQuery(EntityOptions<Post> options);
        Task<PagedResponse<Post>> GetEntitiesAsync(PagingOptions pagingOptions, EntityOptions<Post> options);
        Task<Post> GetEntityAsync(EntityOptions<Post> options);
        Task<(bool isSuccess, Exception exception)> TryAddAsync(Post entity);
        Task<(bool isSuccess, Exception exception)> TryUpdateAsync(Post entity);
        Task<(bool isSuccess, Exception exception)> TryDeleteAsync(Post entity);
    }
}
