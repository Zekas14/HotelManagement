using FluentValidation;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.UpdateRoom
{
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
}
