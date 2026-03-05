using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.FeedbackManagement.GetRoomReviews
{
    public record GetRoomReviewsQuery(int RoomId) : IRequest<RequestResult<List<ReviewResponseDto>>>;

    public record ReviewResponseDto(int Id, int GuestId, string GuestName, int Rating, string? Comment, string? StaffResponse, DateTime? ResponseDate, DateTime CreatedAt);

    public class GetRoomReviewsHandler(IGenericRepository<Review> reviewRepository) : IRequestHandler<GetRoomReviewsQuery, RequestResult<List<ReviewResponseDto>>>
    {
        public async Task<RequestResult<List<ReviewResponseDto>>> Handle(GetRoomReviewsQuery request, CancellationToken cancellationToken)
        {
            var reviews = await reviewRepository.Get(r => r.RoomId == request.RoomId)
                .Include(r => r.Guest)
                .Select(r => new ReviewResponseDto(r.Id, r.GuestId, r.Guest.FullName, r.Rating, r.Comment, r.StaffResponse, r.ResponseDate, r.CreatedAt))
                .ToListAsync(cancellationToken);

            return RequestResult<List<ReviewResponseDto>>.Success(reviews);
        }
    }
}
