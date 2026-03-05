using FluentValidation;

namespace HotelManagement.Features.FeedbackManagement.SubmitReview
{
    public class SubmitReviewValidator : AbstractValidator<SubmitReviewCommand>
    {
        public SubmitReviewValidator()
        {
            RuleFor(x => x.GuestId).GreaterThan(0);
            RuleFor(x => x.RoomId).GreaterThan(0);
            RuleFor(x => x.Rating).InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");
            RuleFor(x => x.Comment).MaximumLength(1000);
        }
    }
}
