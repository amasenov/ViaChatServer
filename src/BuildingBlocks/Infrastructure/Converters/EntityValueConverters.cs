using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Converters
{
    public static class EntityValueConverters
    {
        public static ValueConverter<DateTime, DateTime> DateTimeConverter() => new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));
    }
}
