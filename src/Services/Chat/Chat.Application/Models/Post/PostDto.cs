using Chat.Domain.Entities;

using System;

namespace Chat.Application.Models
{
    /// <summary>
    /// The User response model
    /// </summary>
    public record PostDto
    {
        /// <summary>
        /// The user message
        /// </summary>
        public string Message { get; init; }
        /// <summary>
        /// The created date
        /// </summary>
        public DateTime CreateDate { get; init; }
        /// <summary>
        /// The user that made the post
        /// </summary>
        public string User { get; init; }

        public static implicit operator PostDto(Post entity) => (entity != null) ? new()
        {
            Message = entity.Message,
            CreateDate = entity.Created,
            User = entity.User?.Name
        } : null;
    }
}
