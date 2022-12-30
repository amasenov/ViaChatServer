using System.Threading.Tasks;

namespace Chat.Persistence
{
    public record DatabaseSeeder
    {
        private readonly ChatDbContext _dbContext;
        public DatabaseSeeder(
            ChatDbContext dbContext)
        {
            dbContext.Database.EnsureCreated();

            _dbContext = dbContext;
        }

        public async Task SeedAsync()
        {
            //await SeedChatAsync();
        }

        private async Task SeedChatAsync()
        {
        }
    }
}
