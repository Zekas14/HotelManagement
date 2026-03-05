using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.PaymentManagement.ProcessPayment
{
    public class ProcessPaymentEndpoint(IMediator mediator, IValidator<ProcessPaymentCommand> validator) : PostEndpoint<ProcessPaymentCommand, bool>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/payments/process";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Payment Management")
                .Accepts<ProcessPaymentCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(200, "application/json")
                .WithSummary("Process a Payment")
                .WithDescription("Processes a payment for a reservation and generates an invoice"));
        }

        public override async Task HandleAsync(ProcessPaymentCommand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);
                return;
            }

            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : new FailureEndpointResult<bool>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
