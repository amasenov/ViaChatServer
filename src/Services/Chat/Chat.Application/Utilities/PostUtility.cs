using Chat.Application.Interfaces;
using Chat.Application.Models.Enumerations.Includes;
using Chat.Application.Models.Enumerations.Sort;
using Chat.Application.Models.Filters;
using Chat.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Functions;
using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;
using ViaChatServer.BuildingBlocks.Infrastructure.Utilities;

namespace Chat.Application.Utilities
{
    internal static class PostUtility
    {
        internal static async Task<Post> GetPostByIdAsync(IPostRepository repository, Guid postId, PostIncludes includes = PostIncludes.All, bool excludeTracking = false)
        {
            EntityOptions<Post> options = new()
            {
                Filters = new PostFilters() { Id = postId },
                Includes = PostUtility.GetIncludes(includes),
                ExcludeTracking = excludeTracking,
                UseSingleQuery = true
            };

            return await repository.GetEntityAsync(options);
        }
        internal static List<Func<IQueryable<Post>, IIncludableQueryable<Post, object>>> GetIncludes(PostIncludes includes = PostIncludes.All)
        {
            List<Func<IQueryable<Post>, IIncludableQueryable<Post, object>>> result = new();
            if (includes.HasFlag(PostIncludes.User))
            {
                result.Add(x => x.Include(p => p.User));
            }
            if (includes.HasFlag(PostIncludes.Room))
            {
                result.Add(x => x.Include(p => p.Room));
            }

            return result;
        }
        internal static List<ISortingExpression<Post>> GetSorting(PostSortByType sortBy, bool orderDescending, string searchTerm = null)
        {
            List<ISortingExpression<Post>> sorting = new();

            if (searchTerm.HasValue())
            {
                sorting.Add(new SortingExpression<Post, int>(p => SqlFunctions.SearchScore(searchTerm.Trim().ToUpper(), p.Message.ToUpper()), true));
            }

            if (sortBy == PostSortByType.Message)
            {
                sorting.Add(new SortingExpression<Post, string>(x => x.Message, orderDescending));
            }

            if (sortBy == PostSortByType.CreateDate)
            {
                sorting.Add(new SortingExpression<Post, DateTime>(x => x.Created, orderDescending));
            }

            sorting.Add(new SortingExpression<Post, Guid>(x => x.Id, orderDescending));

            return sorting;
        }
    }
}
