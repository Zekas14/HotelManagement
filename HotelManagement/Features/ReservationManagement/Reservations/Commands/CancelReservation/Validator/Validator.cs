using FluentValidation;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.CancelReservation
{
    public class CancelReservationValidator : AbstractValidator<CancelReservationCommand>
    {
        public CancelReservationValidator()
        {
            RuleFor(x => x.ReservationId).GreaterThan(0).WithMessage("ReservationId must be greater than 0");
            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }


}
