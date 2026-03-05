using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.FeedbackManagement.RespondToReview
{
    public class RespondToReviewEndpoint(IMediator mediator, IValidator<RespondToReviewCommand> validator) : PutEndpoint<RespondToReviewCommand, bool>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reviews/{{ReviewId:int}}/respond";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Customer Feedback")
                .Accepts<RespondToReviewCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(200, "application/json")
                .WithSummary("Respond to a Guest Review (Staff)"));
        }

        public override async Task HandleAsync(RespondToReviewCommand req, CancellationToken ct)
        {
            var reviewId = Route<int>("ReviewId");
            // Overwrite ReviewId in command with route param just in case
            var command = req with { ReviewId = reviewId };

            var validationResult = await Validate(command);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);
                return;
            }

            var result = await mediator.Send(command, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : new FailureEndpointResult<bool>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
