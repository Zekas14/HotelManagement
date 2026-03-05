using HotelManagement.Domain.Enums;
using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using Microsoft.EntityFrameworkCore;

namespace HotelManagement.Features.PaymentManagement.ProcessPayment
{
    public record ProcessPaymentCommand(int ReservationId, decimal Amount, string PaymentMethod) : IRequest<RequestResult<bool>>;

    public class ProcessPaymentHandler(IGenericRepository<Payment> paymentRepository, IGenericRepository<Reservation> reservationRepository, IGenericRepository<Invoice> invoiceRepository) : IRequestHandler<ProcessPaymentCommand, RequestResult<bool>>
    {
        public async Task<RequestResult<bool>> Handle(ProcessPaymentCommand request, CancellationToken cancellationToken)
        {
            var reservation = reservationRepository.GetById(request.ReservationId);
            if (reservation == null)
                return RequestResult<bool>.Failure(ErrorCode.NotFound, "Reservation not found.");

            var existingPayment = await paymentRepository.Get(p => p.ReservationId == request.ReservationId && p.Status == PaymentStatus.Completed)
                                                         .FirstOrDefaultAsync(cancellationToken);
            if (existingPayment != null)
                return RequestResult<bool>.Failure(ErrorCode.PaymentAlreadyProcessed, "Payment already processed for this reservation.");

            var payment = new Payment
            {
                ReservationId = request.ReservationId,
                Amount = request.Amount,
                PaymentDate = DateTime.UtcNow,
                Status = PaymentStatus.Completed,
                PaymentMethod = request.PaymentMethod
            };

            paymentRepository.Add(payment);
            await paymentRepository.SaveChangesAsync();

            var invoice = new Invoice
            {
                PaymentId = payment.Id,
                InvoiceNumber = $"INV-{DateTime.UtcNow:yyyyMMdd}-{Guid.NewGuid().ToString().Substring(0, 5).ToUpper()}",
                IssuedDate = DateTime.UtcNow,
                DueDate = DateTime.UtcNow.AddDays(14),
                TotalAmount = request.Amount
            };

            invoiceRepository.Add(invoice);
            await invoiceRepository.SaveChangesAsync();

            return RequestResult<bool>.Success(true, "Payment processed and invoice generated successfully.");
        }
    }
}
