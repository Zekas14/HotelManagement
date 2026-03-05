using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.ReportingManagement.OccupancyAnalytics
{
    public record OccupancyAnalyticsQuery(DateTime? FromDate, DateTime? ToDate) : IRequest<RequestResult<OccupancyAnalyticsDto>>;

    public record OccupancyAnalyticsDto(int TotalRooms, int OccupiedRooms, double OccupancyRatePercent);

    public class OccupancyAnalyticsHandler(IGenericRepository<Room> roomRepository, IGenericRepository<Reservation> reservationRepository) : IRequestHandler<OccupancyAnalyticsQuery, RequestResult<OccupancyAnalyticsDto>>
    {
        public async Task<RequestResult<OccupancyAnalyticsDto>> Handle(OccupancyAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var from = request.FromDate ?? DateTime.UtcNow;
            var to = request.ToDate ?? DateTime.UtcNow;

            var totalRooms = await roomRepository.Get(r => true).CountAsync(cancellationToken);

            var reservationsInPeriod = await reservationRepository.Get(r => 
                r.Status != Domain.Enums.ReservationStatus.Cancelled &&
                r.CheckInDate < to && 
                r.CheckOutDate > from)
                .Select(r => r.RoomId)
                .Distinct()
                .CountAsync(cancellationToken);

            var occupancyRate = totalRooms == 0 ? 0 : Math.Round((double)reservationsInPeriod / totalRooms * 100, 2);

            return RequestResult<OccupancyAnalyticsDto>.Success(new OccupancyAnalyticsDto(totalRooms, reservationsInPeriod, occupancyRate), "Occupancy analytics generated.");
        }
    }
}
