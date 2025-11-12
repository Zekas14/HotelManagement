using FluentValidation;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManagement.Features.RoomManagement.Rooms
{
    #region Command    
    public record DeleteRoomCommand( int RoomID): IRequest<RequestResult<bool>>;

    public class DeleteRoomCommandValidator: AbstractValidator<DeleteRoomCommand>
    {
        public DeleteRoomCommandValidator()
        {
            RuleFor(x=>x.RoomID).NotEmpty();
        }
    }
    #endregion

    #region Command Handler
    public class DeleteRoomCommandHandler(IGenericRepository<Room> repository, IMediator mediator, IMemoryCache cache) : IRequestHandler<DeleteRoomCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Room> repository = repository;
        private readonly IMediator mediator = mediator;
        private readonly IMemoryCache cache = cache;

        public async Task<RequestResult<bool>> Handle(DeleteRoomCommand request, CancellationToken cancellationToken)
        {
            var isRoomExistsResult = await mediator.Send(new IsEntityExistsQuery<Room>(request.RoomID),cancellationToken);
            if (!isRoomExistsResult.IsSuccess)
            {
                return RequestResult<bool>.Failure(isRoomExistsResult.Message);
            }
            repository.Delete(request.RoomID);
            await repository.SaveChangesAsync();
            cache.Remove("rooms");
            return RequestResult<bool>.Success(true,"Room Deleted Successfully");
        }
    }
    #endregion

    #region Endpoint 
    public class DeleteRoomEndPoint(IMediator mediator , IValidator<DeleteRoomCommand> validator) : DeleteEndpoint<DeleteRoomCommand>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/delete/"+"{id:int}";
        override public async Task HandleAsync([FromRoute] DeleteRoomCommand req, CancellationToken ct)
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
