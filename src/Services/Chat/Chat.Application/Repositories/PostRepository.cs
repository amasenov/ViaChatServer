using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Repositories;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    internal record PostRepository : BaseIdEntityRepository<Post>, IPostRepository
    {
        public PostRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
