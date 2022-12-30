using Microsoft.EntityFrameworkCore.Metadata.Builders;
using ViaChatServer.BuildingBlocks.Infrastructure.Entities;

namespace ViaChatServer.BuildingBlocks.Infrastructure.Configurations
{
    public class BaseIdEntityConfiguration<TEntity> : BaseEntityConfiguration<TEntity> where TEntity : BaseIdEntity
    {
        public override void Configure(EntityTypeBuilder<TEntity> builder)
        {
            builder.HasKey(d => d.Id);

            base.Configure(builder);
        }
    }
}
