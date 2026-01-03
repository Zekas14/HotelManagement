using FluentValidation;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.DeleteRoom
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
            var isRoomExistsResult = await mediator.Send(new EntityExistsQuery<Room>(request.RoomID),cancellationToken);
            if (!isRoomExistsResult.IsSuccess)
            {
                return RequestResult<bool>.Failure(ErrorCode.NotFound,isRoomExistsResult.Message);
            }
            repository.Delete(request.RoomID);
            await repository.SaveChangesAsync();
            cache.Remove("rooms");
            return RequestResult<bool>.Success(true,"Room Deleted Successfully");
        }
    }

    #endregion
    
}
