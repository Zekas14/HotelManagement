using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.ReportingManagement.BookingsReport
{
    public record BookingsReportQuery(DateTime? FromDate, DateTime? ToDate) : IRequest<RequestResult<BookingsReportDto>>;

    public record BookingsReportDto(int TotalBookings, int CancelledBookings, int CompletedBookings);

    public class BookingsReportHandler(IGenericRepository<Reservation> reservationRepository) : IRequestHandler<BookingsReportQuery, RequestResult<BookingsReportDto>>
    {
        public async Task<RequestResult<BookingsReportDto>> Handle(BookingsReportQuery request, CancellationToken cancellationToken)
        {
            var query = reservationRepository.Get(r => true);

            if (request.FromDate.HasValue)
                query = query.Where(r => r.CheckInDate >= request.FromDate.Value);
            
            if (request.ToDate.HasValue)
                query = query.Where(r => r.CheckInDate <= request.ToDate.Value);

            var reservations = await query.ToListAsync(cancellationToken);

            var total = reservations.Count;
            var cancelled = reservations.Count(r => r.Status == Domain.Enums.ReservationStatus.Cancelled);
            var completed = reservations.Count(r => r.Status == Domain.Enums.ReservationStatus.Completed);

            return RequestResult<BookingsReportDto>.Success(new BookingsReportDto(total, cancelled, completed), "Bookings report generated.");
        }
    }
}
