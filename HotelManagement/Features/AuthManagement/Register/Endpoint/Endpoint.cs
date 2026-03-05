using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.AuthManagement.Register
{
    public class RegisterEndpoint(IMediator mediator, IValidator<RegisterCommand> validator) : PostEndpoint<RegisterCommand, bool>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/auth/register";

        public override void Configure()
        {
            base.Configure();
            AllowAnonymous();
            Description(b => b
                .WithTags("Auth Management")
                .Accepts<RegisterCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(200, "application/json")
                .WithSummary("Register a new Customer"));
        }

        public override async Task HandleAsync(RegisterCommand req, CancellationToken ct)
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
