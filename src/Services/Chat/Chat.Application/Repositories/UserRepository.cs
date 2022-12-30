using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Repositories;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    internal record UserRepository : BaseIdEntityRepository<User>, IUserRepository
    {
        public UserRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
