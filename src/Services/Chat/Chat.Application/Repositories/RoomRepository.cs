using ViaChatServer.BuildingBlocks.Infrastructure.Interfaces;
using ViaChatServer.BuildingBlocks.Infrastructure.Repositories;
using Chat.Application.Interfaces;
using Chat.Domain.Entities;

namespace Chat.Application.Repositories
{
    internal record RoomRepository : BaseIdEntityRepository<Room>, IRoomRepository
    {
        public RoomRepository(IUnitOfWork unitOfWork) : base(unitOfWork)
        {
        }
    }
}
