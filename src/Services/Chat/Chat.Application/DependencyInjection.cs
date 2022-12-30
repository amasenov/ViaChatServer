using Microsoft.Extensions.DependencyInjection;
using Chat.Application.Interfaces;
using Chat.Application.Repositories;
using Chat.Application.Services;

namespace Chat.Application
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddRepositories(this IServiceCollection services)
        {
            services.AddScoped<IPostRepository, PostRepository>();
            services.AddScoped<IUserRepository, UserRepository>();
            services.AddScoped<IRoomRepository, RoomRepository>();

            return services;
        }

        public static IServiceCollection AddServices(this IServiceCollection services)
        {
            services.AddScoped<PostService>();
            services.AddScoped<UserService>();
            services.AddScoped<RoomService>();

            return services;
        }
    }
}
