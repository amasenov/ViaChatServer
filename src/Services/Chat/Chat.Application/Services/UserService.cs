using Chat.Application.Interfaces;
using Chat.Application.Models;
using Chat.Application.Models.Enumerations.Includes;
using Chat.Application.Models.Enumerations.Sort;
using Chat.Application.Models.Filters;
using Chat.Application.Utilities;
using Chat.Domain.Entities;

using System;
using System.Linq;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;

namespace Chat.Application.Services
{
    public class UserService
    {
        private readonly IUserRepository _userRepository;

        public UserService(IUserRepository userRepository)
        {
            _userRepository = userRepository;
        }

        public async Task<PagedResponse<UserDto>> GetUsersAsync(PagingOptions pagingOptions, UserSortByType sortBy = UserSortByType.Id, bool orderDescending = false, string searchTerm = null)
        {
            EntityOptions<User> options = new()
            {
                Filters = new UserFilters() { SearchTerm = searchTerm },
                Includes = UserUtility.GetIncludes(UserIncludes.None),
                Sortings = UserUtility.GetSorting(sortBy, orderDescending, searchTerm),
                ExcludeTrackingWithIdentityResolution = true,
                UseSingleQuery = true
            };

            var result = await _userRepository.GetEntitiesAsync(pagingOptions, options);

            return new PagedResponse<UserDto>(result.PageNumber, result.PageSize, result.Total)
            {
                Results = result.Results.Select(x => (UserDto)x)
            };
        }
        public async Task<UserDto> GetUserByIdAsync(Guid userId)
        {
            var entity = await UserUtility.GetUserByIdAsync(_userRepository, userId, excludeTracking: true);

            return (UserDto)entity;
        }
        public async Task<UserDto> GetUserByNameAsync(string name, UserIncludes includes = UserIncludes.None, bool handleError = true)
        {
            var entity = await UserUtility.GetUserByNameAsync(_userRepository, name, includes, true, handleError);

            return (UserDto)entity;
        }
        public async Task<(UpdateUser, User)> GetUserForPatchingAsync(Guid userId)
        {
            var entity = await UserUtility.GetUserByIdAsync(_userRepository, userId);

            UpdateUser model = new()
            {
                Name = entity.Name
            };

            return (model, entity);
        }
        public async Task<UserDto> CreateUserAsync(CreateUser model)
        {
            _ = await UserUtility.HasUniqueNameAsync(_userRepository, model.Name);

            User entity = model;
            _ = await _userRepository.TryAddAsync(entity);

            return (UserDto)entity;
        }
        public async Task<UserDto> UpdateUserAsync(Guid userId, UpdateUser model, User entity = null)
        {
            entity ??= await UserUtility.GetUserByIdAsync(_userRepository, userId);

            if (model.HasValue())
            {
                if (!entity.Name.IsEqual(model.Name))
                {
                    _ = await UserUtility.HasUniqueNameAsync(_userRepository, model.Name);
                }

                entity.Name = model.Name;
            }

            _ = await _userRepository.TryUpdateAsync(entity);

            return (UserDto)entity;
        }
        public async Task DeleteUserAsync(Guid userId)
        {
            var entity = await UserUtility.GetUserByIdAsync(_userRepository, userId);

            _ = await _userRepository.TryDeleteAsync(entity);
        }
    }
}
