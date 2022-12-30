using Chat.Domain.Entities;

using System.Collections.Generic;
using System.Linq;

namespace Chat.Application.Models
{
    /// <summary>
    /// The user response model
    /// </summary>
    public record UserDto
    {
        /// <summary>
        /// The user name
        /// </summary>
        public string Name { get; init; }
        /// <summary>
        /// The current active state of user
        /// </summary>
        public bool IsActive { get; init; }
        /// <summary>
        /// The user posts
        /// </summary>
        public IEnumerable<PostDto> Posts { get; init; }

        public static implicit operator UserDto(User entity) => (entity != null) ? new()
        {
            Name = entity.Name,
            IsActive = entity.IsActive,
            Posts = entity.Posts?.Select(x => (PostDto)x)
        } : null;
    }
}
