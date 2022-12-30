using System;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Extensions
{
    public static class GuidExtensions
    {
        public static bool IsValid(this Guid guid) => !guid.Equals(Guid.Empty);
        public static bool IsValid(this Guid? guid) => guid.HasValue && guid.Value.IsValid();
    }
}
