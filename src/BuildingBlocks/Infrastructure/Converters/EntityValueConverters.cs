using System;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Converters
{
    public static class EntityValueConverters
    {
        public static ValueConverter<DateTime?, DateTime?> DateTimeNullableConverter() => new(v => v, v => DateTime.SpecifyKind(v.Value, DateTimeKind.Utc));

        public static ValueConverter<DateTime, DateTime> DateTimeConverter() => new(v => v, v => DateTime.SpecifyKind(v, DateTimeKind.Utc));

        public static ValueConverter<T, string> EnumConverter<T>() where T : Enum => new(v => v.ToString(), v => (T)Enum.Parse(typeof(T), v));
    }
}
