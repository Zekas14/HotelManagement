using FluentValidation;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Domain.Models;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands
{
    #region Command 
    public record UpdateRoomCommand(int RoomID, int RoomNumber, string Name,int Capacity, string Type, decimal PricePerNight, bool IsAvailable) : IRequest<RequestResult<bool>>;
    #endregion

    #region Validator
    public class UpdateRoomCommandValidator: AbstractValidator<UpdateRoomCommand>
    {
        private readonly IGenericRepository<Room> repository;


        public UpdateRoomCommandValidator(IGenericRepository<Room> repository)
        {
            this.repository = repository;
            RuleFor(x => x.RoomNumber)
                .GreaterThan(0)
                .Must(BeUniqueRoomNumber)
                .WithMessage("Room number must be unique.");
            RuleFor(x => x.Capacity).GreaterThan(0);
            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(20)
                .Must(type => new List<string> { "single", "double", "suite", "deluxe" }.Contains(type.ToLowerInvariant()))
                .WithMessage("Type must be one of: Single, Double, Suite, Deluxe.");
            RuleFor(x => x.PricePerNight).GreaterThan(0);
            RuleFor(x => x.Name).NotEmpty().MinimumLength(2).MaximumLength(100);
        }
        private bool BeUniqueRoomNumber(int roomNumber)
        {
            return !repository.GetAll().Any(r => r.RoomNumber == roomNumber);
        }

    }
    #endregion

    #region Command Handler
    public class UpdateRoomCommandHandler(IGenericRepository<Room> repository, IMediator mediator) : IRequestHandler<UpdateRoomCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Room> repository = repository;
        private readonly IMediator mediator = mediator;

        public async Task<RequestResult<bool>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
          var isRoomExistsResult = await mediator.Send(new IsEntityExistsQuery<Room>(request.RoomID),cancellationToken);
            string[] values =
                [nameof(Room.RoomNumber), nameof(Room.Type), nameof(Room.PricePerNight), nameof(Room.IsAvailable)];

            repository.SaveInclude(isRoomExistsResult.Data,values);
            await repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true, "Room Updated Sucessfully");
        }
    }

    #endregion

    #region Endpoint 
    public class UpdateRoomEndPoint(IMediator mediator,IValidator<UpdateRoomCommand> validator ) : PutEndpoint<UpdateRoomCommand,bool>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/update";
        override public async Task HandleAsync(UpdateRoomCommand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);
                return;
            }
            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }

    #endregion
}
