using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.ReportingManagement.CustomerAnalytics
{
    public record CustomerAnalyticsQuery() : IRequest<RequestResult<CustomerAnalyticsDto>>;

    public record CustomerAnalyticsDto(int TotalCustomers, List<CustomerSummaryDto> TopCustomersBySpend);
    public record CustomerSummaryDto(int GuestId, string FullName, decimal TotalSpend);

    public class CustomerAnalyticsHandler(IGenericRepository<Guest> guestRepository, IGenericRepository<Payment> paymentRepository) : IRequestHandler<CustomerAnalyticsQuery, RequestResult<CustomerAnalyticsDto>>
    {
        public async Task<RequestResult<CustomerAnalyticsDto>> Handle(CustomerAnalyticsQuery request, CancellationToken cancellationToken)
        {
            var totalCustomers = await guestRepository.Get(g => true).CountAsync(cancellationToken);

            var topCustomers = await paymentRepository.Get(p => p.Status == Domain.Enums.PaymentStatus.Completed)
                .Include(p => p.Reservation)
                .ThenInclude(r => r.Guest)
                .GroupBy(p => new { p.Reservation.Guest.Id, p.Reservation.Guest.FullName })
                .Select(g => new CustomerSummaryDto(g.Key.Id, g.Key.FullName, g.Sum(p => p.Amount)))
                .OrderByDescending(c => c.TotalSpend)
                .Take(10)
                .ToListAsync(cancellationToken);

            return RequestResult<CustomerAnalyticsDto>.Success(new CustomerAnalyticsDto(totalCustomers, topCustomers), "Customer analytics generated.");
        }
    }
}
