using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;
using HotelManagement.Features.PaymentManagement.GetInvoice;

namespace HotelManagement.Features.PaymentManagement.GetGuestInvoices
{
    public record GetGuestInvoicesQuery(int GuestId) : IRequest<RequestResult<List<InvoiceResponseDto>>>;

    public class GetGuestInvoicesHandler(IGenericRepository<Invoice> repository) : IRequestHandler<GetGuestInvoicesQuery, RequestResult<List<InvoiceResponseDto>>>
    {
        public async Task<RequestResult<List<InvoiceResponseDto>>> Handle(GetGuestInvoicesQuery request, CancellationToken cancellationToken)
        {
            var invoices = await repository.Get(i => i.Payment.Reservation.GuestId == request.GuestId)
                                          .Include(i => i.Payment)
                                          .Select(i => new InvoiceResponseDto
                                          {
                                              Id = i.Id,
                                              InvoiceNumber = i.InvoiceNumber,
                                              IssuedDate = i.IssuedDate.ToString(Constants.DateFormat),
                                              DueDate = i.DueDate.ToString(Constants.DateFormat),
                                              TotalAmount = i.TotalAmount,
                                              PaymentId = i.PaymentId,
                                              PaymentMethod = i.Payment.PaymentMethod,
                                              PaymentStatus = i.Payment.Status.ToString()
                                          }).ToListAsync(cancellationToken);

            return RequestResult<List<InvoiceResponseDto>>.Success(invoices);
        }
    }
}
