using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.FeedbackManagement.RespondToReview
{
    public record RespondToReviewCommand(int ReviewId, string StaffResponse) : IRequest<RequestResult<bool>>;

    public class RespondToReviewHandler(IGenericRepository<Review> reviewRepository) : IRequestHandler<RespondToReviewCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(RespondToReviewCommand request, CancellationToken cancellationToken)
        {
            var review = reviewRepository.GetById(request.ReviewId);
            if (review == null) return RequestResult<bool>.Failure(ErrorCode.NotFound, "Review not found.");

            review.StaffResponse = request.StaffResponse;
            review.ResponseDate = DateTime.UtcNow;

            reviewRepository.Update(review);
            await reviewRepository.SaveChangesAsync();

            return RequestResult<bool>.Success(true, "Response to review submitted successfully.");
        }
    }
}
