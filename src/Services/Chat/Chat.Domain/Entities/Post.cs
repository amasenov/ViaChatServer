using System;

using ViaChatServer.BuildingBlocks.Infrastructure.Entities;

namespace Chat.Domain.Entities
{
    public class Post : BaseIdEntity
    {
        public string Message { get; set; }
        public string UserName { get; set; }

        public Guid UserId { get; set; }
        public Guid RoomId { get; set; }
        public virtual User User { get; set; }
        public virtual Room Room { get; set; }
    }
}
