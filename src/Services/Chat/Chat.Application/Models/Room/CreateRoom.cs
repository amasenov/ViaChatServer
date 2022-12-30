using Chat.Domain.Entities;

namespace Chat.Application.Models
{
    /// <summary>
    /// The model that is used to transfer the data for creating a room
    /// </summary>
    public record CreateRoom : UpdateRoom
    {
        public static implicit operator Room(CreateRoom model) => (model != null) ? new()
        {
            Name = model.Name
        } : null;
    }
}
