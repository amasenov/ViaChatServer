using Chat.Domain.Entities;

namespace Chat.Application.Models
{
    /// <summary>
    /// The model that is used to transfer the data for creating a user
    /// </summary>
    public record CreateUser : UpdateUser
    {
        public static implicit operator User(CreateUser model) => (model != null) ? new()
        {
            Name = model.Name
        } : null;
    }
}
