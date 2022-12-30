using System;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Entities
{
    public class BaseIdEntity : BaseEntity
    {
        public Guid Id { get; set; }
    }
}
