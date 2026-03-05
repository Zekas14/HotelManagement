using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;
using HotelManagement.Features.PaymentManagement.GetInvoice;

namespace HotelManagement.Features.PaymentManagement.GetGuestInvoices
{
    public class GetGuestInvoicesEndpoint(IMediator mediator) : GetEndpoint<List<InvoiceResponseDto>>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/invoices/guest/{{GuestId:int}}";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Payment Management")
                .Produces<SuccessEndpointResult<List<InvoiceResponseDto>>>(200, "application/json")
                .WithSummary("Get Guest Invoices")
                .WithDescription("Gets all invoices for a specific guest"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var guestId = Route<int>("GuestId");
            var result = await _mediator.Send(new GetGuestInvoicesQuery(guestId), ct);

            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<List<InvoiceResponseDto>>(result.Data, result.Message)
                : new FailureEndpointResult<List<InvoiceResponseDto>>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
