using Chat.Domain.Entities;

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

        public static implicit operator PostDto(Post entity) => (entity != null) ? new()
        {
            Message = entity.Message
        } : null;
    }
}
