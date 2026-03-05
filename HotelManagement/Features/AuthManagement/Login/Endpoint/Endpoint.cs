using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.AuthManagement.Login
{
    public class LoginEndpoint(IMediator mediator, IValidator<LoginCommand> validator) : PostEndpoint<LoginCommand, string>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/auth/login";

        public override void Configure()
        {
            base.Configure();
            AllowAnonymous();
            Description(b => b
                .WithTags("Auth Management")
                .Accepts<LoginCommand>("application/json")
                .Produces<SuccessEndpointResult<string>>(200, "application/json")
                .WithSummary("Login for Customer and Staff"));
        }

        public override async Task HandleAsync(LoginCommand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);
                return;
            }

            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<string>(result.Data, result.Message)
                : new FailureEndpointResult<string>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
