using FluentValidation;
using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation
{
    public class Validator : AbstractValidator<MakeReservationCommand>
        {
            private readonly IGenericRepository<Room> repository;
            public Validator(IGenericRepository<Room> repository)
            {
                this.repository = repository;
                RuleFor(x => x.CheckInDate)
                    .LessThan(x => x.CheckOutDate)
                    .WithMessage("Check-in date must be before check-out date.");
                RuleFor(x => x.CheckInDate)
                    .GreaterThanOrEqualTo(DateTime.Now)
                    .WithMessage("Chacke-in Date is Passed");
                RuleFor(x => x.NumberOfGuests)
                    .GreaterThan(0)
                    .WithMessage("Number of guests must be greater than zero.");
                RuleFor(x => x.PricePerNight)
                    .GreaterThan(0)
                    .WithMessage("Price per night must be greater than zero.");
                RuleFor(x => x.CheckOutDate)
                    .GreaterThan(x => x.CheckInDate)
                    .WithMessage("Check-out date must be after check-in date.");
                RuleFor(x => x.CheckOutDate)
                  .GreaterThan(DateTime.Now)
                  .WithMessage("Chacke-Out Date Must be in the Future");
                RuleFor(x => x.RoomId)
                    .Must(RoomExists)
                    .WithMessage("Room does not exist.");
            }
            private bool RoomExists(int roomId)
            {
                return repository.GetAll().Any(r => r.Id == roomId);
            }
        }

    
}
 