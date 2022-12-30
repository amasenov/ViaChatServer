using Chat.Domain.Entities;

using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

using ViaChatServer.BuildingBlocks.Infrastructure.Configurations;

namespace Chat.Persistence.Configurations
{
    public class RoomConfiguration : BaseIdEntityConfiguration<Room>
    {
        public override void Configure(EntityTypeBuilder<Room> builder)
        {
            base.Configure(builder);

            builder.HasIndex(x => x.Name);

            builder.Property(x => x.Name)
                .HasColumnType("varchar(64)");

            builder.HasMany<Post>(r => r.Posts)
                .WithOne(p => p.Room)
                .HasForeignKey(p => p.RoomId)
                .OnDelete(DeleteBehavior.Cascade);

            builder.ToTable($"{nameof(Room)}s");
        }
    }
}
