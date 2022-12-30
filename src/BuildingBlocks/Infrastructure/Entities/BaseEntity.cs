using System;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Entities
{
    public class BaseEntity
    {
        public DateTime Created { get; set; }

        public DateTime Modified { get; set; }

        public bool Deleted { get; set; }
    }
}
