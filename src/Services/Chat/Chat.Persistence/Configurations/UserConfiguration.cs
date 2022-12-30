using Chat.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ViaChatServer.BuildingBlocks.Infrastructure.Configurations;

namespace Chat.Persistence.Configurations
{
    public class UserConfiguration : BaseIdEntityConfiguration<User>
    {
        public override void Configure(EntityTypeBuilder<User> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => x.Name);

            builder.Property(x => x.Name)
                .HasColumnType("varchar(64)");

            builder.HasMany<Post>(u => u.Posts)
                .WithOne(p => p.User)
                .HasForeignKey(u => u.UserId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable($"{nameof(User)}s");
        }
    }
}
