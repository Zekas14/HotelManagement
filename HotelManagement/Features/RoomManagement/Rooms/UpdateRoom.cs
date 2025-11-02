using FluentValidation;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Data;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;

namespace HotelManagement.Features.RoomManagement.Rooms
{
    #region Command 
    public record UpdateRoomCommand(int RoomID, int RoomNumber, string Name,int Capacity, string Type, decimal PricePerNight, bool IsAvailable) : IRequest<RequestResult<bool>>;
    #endregion

    #region Validator
    public class UpdateRoomCommandValidator: AbstractValidator<UpdateRoomCommand>
    {
        private readonly ApplicationDbContext context;

        public UpdateRoomCommandValidator(ApplicationDbContext context)
        {
            this.context = context;
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
            return !context.Rooms.Any(r => r.RoomNumber == roomNumber);
        }

    }
    #endregion

    #region Command Handler
    public class UpdateRoomCommandHandler(IGenericRepository<Room> repository) : IRequestHandler<UpdateRoomCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Room> repository = repository;
        public async Task<RequestResult<bool>> Handle(UpdateRoomCommand request, CancellationToken cancellationToken)
        {
            var room = repository.GetById(request.RoomID);
            if (room == null)
            {
                return RequestResult<bool>.Failure("Room not found.");
            }
            string[] values =
                [nameof(Room.RoomNumber), nameof(Room.Name), nameof(Room.Capacity), nameof(Room.Type), nameof(Room.PricePerNight), nameof(Room.IsAvailable)];

            repository.SaveInclude(room,values);
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
