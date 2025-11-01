using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManagement.Features.RoomManagement.Rooms
{

    #region Command    
    public record DeleteRoomCommand(int RoomID): IRequest<RequestResult<bool>>;

    #endregion

    #region Command Handler
    public class DeleteRoomCommandHandler(IGenericRepository<Room> repository , IMemoryCache cache) : IRequestHandler<DeleteRoomCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Room> repository = repository;
        private readonly IMemoryCache cache = cache;

        public async Task<RequestResult<bool>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var room =  repository.GetById(request.RoomID);
            if (room == null)
            {
                return RequestResult<bool>.Failure("Room not found.");
            }
            repository.Delete(room);
            await repository.SaveChangesAsync();
            cache.Remove("rooms");
            return RequestResult<bool>.Success(true,"Room Deleted Successfully");
        }
    }
    #endregion

    #region Endpoint 
    public class DeleteRoomEndPoint : DeleteModule<DeleteRoomCommand>
    {
        protected override string Route => "/api/rooms/delete/{id:int}";
        protected override Delegate Handler => async (int id, IMediator mediator) =>
        {
            return await mediator.Send(new DeleteRoomCommand(id));
        };
    }
    #endregion
}
