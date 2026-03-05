using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.PaymentManagement.GetInvoice
{
    public record GetInvoiceQuery(int InvoiceId) : IRequest<RequestResult<InvoiceResponseDto>>;

    public class InvoiceResponseDto
    {
        public int Id { get; set; }
        public string InvoiceNumber { get; set; }
        public string IssuedDate { get; set; }
        public string DueDate { get; set; }
        public decimal TotalAmount { get; set; }
        public int PaymentId { get; set; }
        public string PaymentMethod { get; set; }
        public string PaymentStatus { get; set; }
    }

    public class GetInvoiceHandler(IGenericRepository<Invoice> repository) : IRequestHandler<GetInvoiceQuery, RequestResult<InvoiceResponseDto>>
    {
        public async Task<RequestResult<InvoiceResponseDto>> Handle(GetInvoiceQuery request, CancellationToken cancellationToken)
        {
            var invoice = await repository.Get(i => i.Id == request.InvoiceId)
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
                                          }).FirstOrDefaultAsync(cancellationToken);

            if (invoice == null)
                return RequestResult<InvoiceResponseDto>.Failure(ErrorCode.NotFound, "Invoice not found.");

            return RequestResult<InvoiceResponseDto>.Success(invoice);
        }
    }
}
