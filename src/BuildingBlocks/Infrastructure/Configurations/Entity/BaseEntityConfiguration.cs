using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViaChatServer.BuildingBlocks.Infrastructure.Converters;
using ViaChatServer.BuildingBlocks.Infrastructure.Entities;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Configurations
{
    public class BaseEntityConfiguration<TEntity> : IEntityTypeConfiguration<TEntity> where TEntity : BaseEntity
    {
        public virtual void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.Property(d => d.Created)
                .HasConversion(EntityValueConverters.DateTimeConverter())
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("(sysutcdatetime())")
                .ValueGeneratedOnAdd();

            builder.Property(d => d.Modified)
                .HasConversion(EntityValueConverters.DateTimeConverter())
                .HasColumnType("datetime2(0)")
                .HasDefaultValueSql("(sysutcdatetime())");

            builder.Property(d => d.Deleted)
                .HasDefaultValue(false);

            builder.HasQueryFilter(d => !d.Deleted);
        }
    }
}
