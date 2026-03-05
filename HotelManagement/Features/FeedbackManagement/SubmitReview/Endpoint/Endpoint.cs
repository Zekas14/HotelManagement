using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.FeedbackManagement.SubmitReview
{
    public class SubmitReviewEndpoint(IMediator mediator, IValidator<SubmitReviewCommand> validator) : PostEndpoint<SubmitReviewCommand, bool>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reviews";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Customer Feedback")
                .Accepts<SubmitReviewCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(200, "application/json")
                .WithSummary("Submit a Review"));
        }

        public override async Task HandleAsync(SubmitReviewCommand req, CancellationToken ct)
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
