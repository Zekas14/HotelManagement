using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.ReportingManagement.RevenueReport
{
    public record RevenueReportQuery(DateTime? FromDate, DateTime? ToDate) : IRequest<RequestResult<RevenueReportDto>>;

    public record RevenueReportDto(decimal TotalRevenue, decimal PendingRevenue, decimal RefundedRevenue);

    public class RevenueReportHandler(IGenericRepository<Payment> paymentRepository) : IRequestHandler<RevenueReportQuery, RequestResult<RevenueReportDto>>
    {
        public async Task<RequestResult<RevenueReportDto>> Handle(RevenueReportQuery request, CancellationToken cancellationToken)
        {
            var query = paymentRepository.Get(p => true);

            if (request.FromDate.HasValue)
                query = query.Where(p => p.PaymentDate >= request.FromDate.Value);
            
            if (request.ToDate.HasValue)
                query = query.Where(p => p.PaymentDate <= request.ToDate.Value);

            var payments = await query.ToListAsync(cancellationToken);

            var total = payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Completed).Sum(p => p.Amount);
            var pending = payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Pending).Sum(p => p.Amount);
            var refunded = payments.Where(p => p.Status == Domain.Enums.PaymentStatus.Refunded).Sum(p => p.Amount);

            return RequestResult<RevenueReportDto>.Success(new RevenueReportDto(total, pending, refunded), "Revenue report generated.");
        }
    }
}
