using FluentValidation;
using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.EditReservation
{
    public class EditReservationCommandValidator : AbstractValidator<EditReservationCommand>
    {
        private readonly IGenericRepository<Reservation> _repository;

        public EditReservationCommandValidator(IGenericRepository<Reservation> repository)
        {
            _repository = repository;

            RuleFor(x => x.ReservationId).GreaterThan(0);
            RuleFor(x => x.CheckIn)
                .LessThan(x => x.CheckOut)
                .WithMessage("Check-in must be before check-out.");
            RuleFor(x => x.CheckOut)
                .GreaterThan(x => x.CheckIn)
                .WithMessage("Check-out must be after check-in.");
            RuleFor(x => x.RoomId)
                .GreaterThan(0);
            RuleFor(x => x.NumberOfGuests)
                .GreaterThan(0)
                .WithMessage("Number of guests must be at least 1.");

        }
    }

}
