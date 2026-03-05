using FluentValidation;

namespace HotelManagement.Features.PaymentManagement.ProcessPayment
{
    public class ProcessPaymentValidator : AbstractValidator<ProcessPaymentCommand>
    {
        public ProcessPaymentValidator()
        {
            RuleFor(x => x.ReservationId).GreaterThan(0).WithMessage("Reservation ID must be valid.");
            RuleFor(x => x.Amount).GreaterThan(0).WithMessage("Amount must be greater than zero.");
            RuleFor(x => x.PaymentMethod).NotEmpty().WithMessage("Payment method is required.");
        }
    }
}
