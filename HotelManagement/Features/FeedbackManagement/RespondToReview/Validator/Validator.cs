using FluentValidation;

namespace HotelManagement.Features.FeedbackManagement.RespondToReview
{
    public class RespondToReviewValidator : AbstractValidator<RespondToReviewCommand>
    {
        public RespondToReviewValidator()
        {
            RuleFor(x => x.ReviewId).GreaterThan(0);
            RuleFor(x => x.StaffResponse).NotEmpty().MaximumLength(1000);
        }
    }
}
