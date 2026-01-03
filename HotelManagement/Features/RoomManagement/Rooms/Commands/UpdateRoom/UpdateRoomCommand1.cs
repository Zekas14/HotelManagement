using FluentValidation;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Domain.Models;
using MediatR;
using HotelManagement.Features.Common.Responses;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.UpdateRoom
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
          var isRoomExistsResult = await mediator.Send(new EntityExistsQuery<Room>(request.RoomID),cancellationToken);
            string[] values =
                [nameof(Room.RoomNumber), nameof(Room.Type), nameof(Room.PricePerNight), nameof(Room.IsAvailable)];

            repository.SaveInclude(isRoomExistsResult.Data,values);
            await repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true, "Room Updated Sucessfully");
        }
    }

    #endregion
}
