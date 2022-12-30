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
    public class RoomService
    {
        private readonly IRoomRepository _roomRepository;

        public RoomService(IRoomRepository roomRepository)
        {
            _roomRepository = roomRepository;
        }

        public async Task<PagedResponse<RoomDto>> GetRoomsAsync(PagingOptions pagingOptions, RoomSortByType sortBy = RoomSortByType.Id, bool orderDescending = false, string searchTerm = null)
        {
            EntityOptions<Room> options = new()
            {
                Filters = new RoomFilters() { SearchTerm = searchTerm },
                Includes = RoomUtility.GetIncludes(RoomIncludes.None),
                Sortings = RoomUtility.GetSorting(sortBy, orderDescending, searchTerm),
                ExcludeTrackingWithIdentityResolution = true,
                UseSingleQuery = true
            };

            var result = await _roomRepository.GetEntitiesAsync(pagingOptions, options);

            return new PagedResponse<RoomDto>(result.PageNumber, result.PageSize, result.Total)
            {
                Results = result.Results.Select(x => (RoomDto)x)
            };
        }
        public async Task<RoomDto> GetRoomByIdAsync(Guid roomId)
        {
            var entity = await RoomUtility.GetRoomByIdAsync(_roomRepository, roomId, excludeTracking: true);

            return (RoomDto)entity;
        }
        public async Task<(UpdateRoom, Room)> GetRoomForPatchingAsync(Guid roomId)
        {
            var entity = await RoomUtility.GetRoomByIdAsync(_roomRepository, roomId);

            UpdateRoom model = new()
            {
                Name = entity.Name
            };

            return (model, entity);
        }
        public async Task<RoomDto> CreateRoomAsync(CreateRoom model)
        {
            _ = await RoomUtility.HasUniqueNameAsync(_roomRepository, model.Name);

            Room entity = model;
            _ = await _roomRepository.TryAddAsync(entity);

            return (RoomDto)entity;
        }
        public async Task<RoomDto> UpdateRoomAsync(Guid roomId, UpdateRoom model, Room entity = null)
        {
            entity ??= await RoomUtility.GetRoomByIdAsync(_roomRepository, roomId);

            if (model.HasValue())
            {
                if (!entity.Name.IsEqual(model.Name))
                {
                    _ = await RoomUtility.HasUniqueNameAsync(_roomRepository, model.Name);
                }

                entity.Name = model.Name;
            }

            _ = await _roomRepository.TryUpdateAsync(entity);

            return (RoomDto)entity;
        }
        public async Task DeleteRoomAsync(Guid roomId)
        {
            var entity = await RoomUtility.GetRoomByIdAsync(_roomRepository, roomId);

            _ = await _roomRepository.TryDeleteAsync(entity);
        }
    }
}
