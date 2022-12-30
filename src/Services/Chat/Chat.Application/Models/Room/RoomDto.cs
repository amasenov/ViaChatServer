using Chat.Domain.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Chat.Application.Models
{
    /// <summary>
    /// The room response model
    /// </summary>
    public record RoomDto
    {
        /// <summary>
        /// The room name
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// The room users
        /// </summary>
        public IEnumerable<UserDto> Users { get; init; }

        public static implicit operator RoomDto(Room entity) => (entity != null) ? new()
        {
            Name = entity.Name,
            Users = entity.Posts?.Where(x => x.User != null).Select(x => (UserDto)x.User)
        } : null;
    }
}
