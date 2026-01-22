using FluentValidation;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.AddRoom
{
    public class AddRoomValidator : AbstractValidator<AddRoomCommand>
    {
        private readonly IGenericRepository<Room> repository;
        public AddRoomValidator(IGenericRepository<Room> repository)
        {
            this.repository = repository;
            RuleFor(x => x.RoomNumber).GreaterThan(0);
            RuleFor(x => x.Type)
                .NotEmpty()
                .MaximumLength(20)
                .Must(type => new List<string> { "Single", "Double", "Suite", "Deluxe" }.Contains(type))
                .WithMessage("Type must be one of: Single, Double, Suite, Deluxe.");
            RuleFor(x => x.PricePerNight).GreaterThan(0);
            RuleFor(x=>x.ImageUrl).NotEmpty().MinimumLength(2);
            RuleFor(x => x.RoomNumber)
                .Must(BeUniqueRoomNumber)
                .WithMessage("Room number must be unique.");
        }
        private bool BeUniqueRoomNumber(int roomNumber)
        {
            return !repository.GetAll().Any(r => r.RoomNumber == roomNumber);
        }
    }

}

