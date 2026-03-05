using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.PaymentManagement.GetInvoice
{
    public class GetInvoiceEndpoint(IMediator mediator) : GetEndpoint<InvoiceResponseDto>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/invoices/{{InvoiceId:int}}";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Payment Management")
                .Produces<SuccessEndpointResult<InvoiceResponseDto>>(200, "application/json")
                .WithSummary("Get Invoice")
                .WithDescription("Gets a specific invoice by ID"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var invoiceId = Route<int>("InvoiceId");
            var result = await _mediator.Send(new GetInvoiceQuery(invoiceId), ct);

            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<InvoiceResponseDto>(result.Data, result.Message)
                : new FailureEndpointResult<InvoiceResponseDto>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
