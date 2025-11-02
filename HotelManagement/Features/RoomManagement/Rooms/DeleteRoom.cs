using FastEndpoints;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManagement.Features.RoomManagement.Rooms
{
    #region Command    
    public record DeleteRoomCommand([FromRoute] int RoomID): IRequest<RequestResult<bool>>;

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
    public class DeleteRoomEndPoint(IMediator mediator) : DeleteModule<DeleteRoomCommand>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/delete";
        override public async Task HandleAsync(DeleteRoomCommand req, CancellationToken ct)
        {
            var result =  await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}
