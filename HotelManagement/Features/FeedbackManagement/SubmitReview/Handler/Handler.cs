using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.FeedbackManagement.SubmitReview
{
    public record SubmitReviewCommand(int GuestId, int RoomId, int Rating, string? Comment) : IRequest<RequestResult<bool>>;

    public class SubmitReviewHandler(IGenericRepository<Review> reviewRepository, IGenericRepository<Guest> guestRepository, IGenericRepository<Room> roomRepository) : IRequestHandler<SubmitReviewCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(SubmitReviewCommand request, CancellationToken cancellationToken)
        {
            var guest = guestRepository.GetById(request.GuestId);
            if (guest == null) return RequestResult<bool>.Failure(ErrorCode.NotFound, "Guest not found.");

            var room = roomRepository.GetById(request.RoomId);
            if (room == null) return RequestResult<bool>.Failure(ErrorCode.NotFound, "Room not found.");

            var review = new Review
            {
                GuestId = request.GuestId,
                RoomId = request.RoomId,
                Rating = request.Rating,
                Comment = request.Comment,
                CreatedAt = DateTime.UtcNow
            };

            reviewRepository.Add(review);
            await reviewRepository.SaveChangesAsync();

            return RequestResult<bool>.Success(true, "Review submitted successfully.");
        }
    }
}
