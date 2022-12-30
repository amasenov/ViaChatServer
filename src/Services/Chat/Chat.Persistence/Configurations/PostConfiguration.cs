using Chat.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ViaChatServer.BuildingBlocks.Infrastructure.Configurations;

namespace Chat.Persistence.Configurations
{
    public class PostConfiguration : BaseIdEntityConfiguration<Post>
    {
        public override void Configure(EntityTypeBuilder<Post> builder)
        {
            base.Configure(builder);

            builder.Property(x => x.Message)
                .HasColumnType("varchar(256)");
            builder.Property(x => x.UserName)
                .HasColumnType("varchar(64)");

            builder.ToTable($"{nameof(Post)}s");
        }
    }
}
