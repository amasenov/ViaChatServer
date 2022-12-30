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
    internal static class RoomUtility
    {
        internal static async Task<Room> GetRoomByIdAsync(IRoomRepository repository, Guid roomId, RoomIncludes includes = RoomIncludes.All, bool excludeTracking = false)
        {
            EntityOptions<Room> options = new()
            {
                Filters = new RoomFilters() { Id = roomId },
                Includes = RoomUtility.GetIncludes(includes),
                ExcludeTracking = excludeTracking,
                UseSingleQuery = true
            };

            return await repository.GetEntityAsync(options);
        }
        internal static async Task<Room> GetRoomByNameAsync(IRoomRepository repository, string name, RoomIncludes includes = RoomIncludes.None, bool excludeTracking = false, bool handleError = true)
        {
            EntityOptions<Room> options = new()
            {
                Filters = new RoomFilters() { Name = name },
                ExcludeTracking = excludeTracking,
                UseSingleQuery = true,
                HandleError = handleError
            };

            return await repository.GetEntityAsync(options);
        }
        internal static async Task<bool> HasUniqueNameAsync(IRoomRepository repository, string name, bool handleError = true)
        {
            Expression<Func<Room, bool>> expression = p => p.Name.Trim().ToUpper().Equals(name.Trim().ToUpper());
            ValidationException exception = new(GeneralValidationError.DuplicateRoom);

            return await repository.HandleValidationAsync(expression, exception, handleError);
        }
        internal static List<Func<IQueryable<Room>, IIncludableQueryable<Room, object>>> GetIncludes(RoomIncludes includes = RoomIncludes.All)
        {
            List<Func<IQueryable<Room>, IIncludableQueryable<Room, object>>> result = new();
            if (includes.HasFlag(RoomIncludes.Posts))
            {
                result.Add(x => x.Include(r => r.Posts));
            }
            if (includes.HasFlag(RoomIncludes.Users))
            {
                result.Add(x => x.Include(r => r.Posts).ThenInclude(p => p.User));
            }

            return result;
        }
        internal static List<ISortingExpression<Room>> GetSorting(RoomSortByType sortBy, bool orderDescending, string searchTerm = null)
        {
            List<ISortingExpression<Room>> sorting = new();

            if (searchTerm.HasValue())
            {
                sorting.Add(new SortingExpression<Room, int>(p => SqlFunctions.SearchScore(searchTerm.Trim().ToUpper(), p.Name.ToUpper()), true));
            }

            if (sortBy == RoomSortByType.Name)
            {
                sorting.Add(new SortingExpression<Room, string>(x => x.Name, orderDescending));
            }

            if (sortBy == RoomSortByType.Limit)
            {
                sorting.Add(new SortingExpression<Room, int>(x => x.Limit, orderDescending));
            }

            if (sortBy == RoomSortByType.CreateDate)
            {
                sorting.Add(new SortingExpression<Room, DateTime>(x => x.Created, orderDescending));
            }

            sorting.Add(new SortingExpression<Room, Guid>(x => x.Id, orderDescending));

            return sorting;
        }
    }
}
