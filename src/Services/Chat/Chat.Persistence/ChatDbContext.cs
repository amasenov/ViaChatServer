using Chat.Domain.Entities;

using Microsoft.EntityFrameworkCore;

using ViaChatServer.BuildingBlocks.Infrastructure.Functions;

namespace Chat.Persistence
{
    public class ChatDbContext : DbContext
    {
        public ChatDbContext(DbContextOptions<ChatDbContext> options) : base(options)
        {
            this.ChangeTracker.LazyLoadingEnabled = false;
        }

        #region Entities
        public DbSet<Room> Rooms { get; set; }
        public DbSet<User> Users { get; set; }
        public DbSet<Post> Posts { get; set; }
        #endregion

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            modelBuilder.ApplyConfigurationsFromAssembly(typeof(ChatDbContext).Assembly);

            modelBuilder.HasDbFunction(typeof(SqlFunctions).GetMethod(nameof(SqlFunctions.SearchScore)))
                .HasTranslation(SqlFunctionTranslations.GetSearchScoreTranslation());
        }
    }
}
