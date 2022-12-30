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
using System.Linq.Expressions;
using System.Threading.Tasks;

using ViaChatServer.BuildingBlocks.Infrastructure.Exceptions;
using ViaChatServer.BuildingBlocks.Infrastructure.Extensions;
using ViaChatServer.BuildingBlocks.Infrastructure.Functions;
using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Models;
using ViaChatServer.BuildingBlocks.Infrastructure.Models.Enumerations;
using ViaChatServer.BuildingBlocks.Infrastructure.Utilities;

namespace Chat.Application.Utilities
{
    internal static class UserUtility
    {
        internal static async Task<User> GetUserByIdAsync(IUserRepository repository, Guid userId, UserIncludes includes = UserIncludes.All, bool excludeTracking = false)
        {
            EntityOptions<User> options = new()
            {
                Filters = new UserFilters() { Id = userId },
                Includes = UserUtility.GetIncludes(includes),
                ExcludeTracking = excludeTracking,
                UseSingleQuery = true
            };

            return await repository.GetEntityAsync(options);
        }
        internal static async Task<User> GetUserByNameAsync(IUserRepository repository, string name, UserIncludes includes = UserIncludes.None, bool excludeTracking = false, bool handleError = true)
        {
            EntityOptions<User> options = new()
            {
                Filters = new UserFilters() { Name = name },
                Includes = UserUtility.GetIncludes(includes),
                ExcludeTracking = excludeTracking,
                UseSingleQuery = true,
                HandleError = handleError
            };

            return await repository.GetEntityAsync(options);
        }
        internal static async Task<bool> HasUniqueNameAsync(IUserRepository repository, string name, bool handleError = true)
        {
            Expression<Func<User, bool>> expression = p => p.Name.Trim().ToUpper().Equals(name.Trim().ToUpper());
            ValidationException exception = new(GeneralValidationError.DuplicateUser);

            return await repository.HandleValidationAsync(expression, exception, handleError);
        }
        internal static List<Func<IQueryable<User>, IIncludableQueryable<User, object>>> GetIncludes(UserIncludes includes = UserIncludes.All)
        {
            List<Func<IQueryable<User>, IIncludableQueryable<User, object>>> result = new();
            if (includes.HasFlag(UserIncludes.Posts))
            {
                result.Add(x => x.Include(u => u.Posts));
            }
            if (includes.HasFlag(UserIncludes.Room))
            {
                result.Add(x => x.Include(u => u.Posts).ThenInclude(p => p.Room));
            }

            return result;
        }
        internal static List<ISortingExpression<User>> GetSorting(UserSortByType sortBy, bool orderDescending, string searchTerm = null)
        {
            List<ISortingExpression<User>> sorting = new();

            if (searchTerm.HasValue())
            {
                sorting.Add(new SortingExpression<User, int>(p => SqlFunctions.SearchScore(searchTerm.Trim().ToUpper(), p.Name.ToUpper()), true));
            }

            if (sortBy == UserSortByType.Name)
            {
                sorting.Add(new SortingExpression<User, string>(x => x.Name, orderDescending));
            }

            if (sortBy == UserSortByType.CreateDate)
            {
                sorting.Add(new SortingExpression<User, DateTime>(x => x.Created, orderDescending));
            }

            sorting.Add(new SortingExpression<User, Guid>(x => x.Id, orderDescending));

            return sorting;
        }
    }
}
