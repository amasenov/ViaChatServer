using Chat.Application.Interfaces;
using Chat.Application.Models;
using Chat.Application.Models.Enumerations.Includes;
using Chat.Application.Models.Enumerations.Sort;
using Chat.Application.Models.Filters;
using Chat.Application.Utilities;
using Chat.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace Chat.Application.Services
{
    public class PostService
    {
        private readonly IPostRepository _postRepository;
        private readonly IUserRepository _userRepository;
        private readonly IRoomRepository _roomRepository;

        public PostService(IPostRepository postRepository, IUserRepository userRepository, IRoomRepository roomRepository)
        {
            _postRepository = postRepository;
            _userRepository = userRepository;
            _roomRepository = roomRepository;
        }

        public async Task<PagedResponse<PostDto>> GetPostsAsync(PagingOptions pagingOptions, PostSortByType sortBy = PostSortByType.Id, bool orderDescending = false, string searchTerm = null)
        {
            EntityOptions<Post> options = new()
            {
                Filters = new PostFilters() { SearchTerm = searchTerm },
                Includes = PostUtility.GetIncludes(PostIncludes.User),
                Sortings = PostUtility.GetSorting(sortBy, orderDescending, searchTerm),
                ExcludeTrackingWithIdentityResolution = true,
                UseSingleQuery = true
            };

            var result = await _postRepository.GetEntitiesAsync(pagingOptions, options);

            return new PagedResponse<PostDto>(result.PageNumber, result.PageSize, result.Total)
            {
                Results = result.Results.Select(x => (PostDto)x)
            };
        }
        public async Task<List<PostDto>> GetPostsAsync(string roomName, int limit = 20)
        {
            EntityOptions<Post> options = new()
            {
                Filters = new PostFilters() { RoomName = roomName },
                Includes = PostUtility.GetIncludes(PostIncludes.Room),
                Sortings = PostUtility.GetSorting(PostSortByType.CreateDate, true),
                ExcludeTrackingWithIdentityResolution = true,
                UseSingleQuery = true
            };

            var query = _postRepository.CreateBaseQuery(options).Take(limit);
            var entities = await query.ToListAsync();

            return entities.Select(x => (PostDto)x).ToList();
        }
        public async Task<PostDto> GetPostByIdAsync(Guid postId)
        {
            var entity = await PostUtility.GetPostByIdAsync(_postRepository, postId, excludeTracking: true);

            return (PostDto)entity;
        }
        public async Task<(UpdatePost, Post)> GetPostForPatchingAsync(Guid postId)
        {
            var entity = await PostUtility.GetPostByIdAsync(_postRepository, postId);

            UpdatePost model = new()
            {
                Msg = entity.Message
            };

            return (model, entity);
        }
        public async Task<PostDto> CreatePostAsync(CreatePost model)
        {
            var user = await UserUtility.GetUserByNameAsync(_userRepository, model.User, handleError: false);
            if(user.IsNull())
            {
                // if user doesn't exist, create it
                user = new User() { Name = model.User };
                _ = await _userRepository.TryAddAsync(user);
            }
            // if room doesn't exist, it will throw an exception
            var room = await RoomUtility.GetRoomByNameAsync(_roomRepository, model.Room);

            Post entity = model;
            entity.UserId = user.Id;
            entity.RoomId = room.Id;

            _ = await _postRepository.TryAddAsync(entity);

            return (PostDto)entity;
        }
        public async Task<PostDto> UpdatePostAsync(Guid postId, UpdatePost model, Post entity = null)
        {
            entity ??= await PostUtility.GetPostByIdAsync(_postRepository, postId);

            if (model.HasValue())
            {
                entity.Message = model.Msg;
            }

            _ = await _postRepository.TryUpdateAsync(entity);

            return (PostDto)entity;
        }
        public async Task DeletePostAsync(Guid postId)
        {
            var entity = await PostUtility.GetPostByIdAsync(_postRepository, postId);

            _ = await _postRepository.TryDeleteAsync(entity);
        }
    }
}
