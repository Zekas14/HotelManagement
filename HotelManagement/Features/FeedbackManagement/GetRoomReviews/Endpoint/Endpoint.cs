using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.FeedbackManagement.GetRoomReviews
{
    public class GetRoomReviewsEndpoint(IMediator mediator) : GetEndpoint<List<ReviewResponseDto>>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reviews/room/{{RoomId:int}}";

        public override void Configure()
        {
            base.Configure();
            AllowAnonymous();
            Description(b => b
                .WithTags("Customer Feedback")
                .Produces<SuccessEndpointResult<List<ReviewResponseDto>>>(200, "application/json")
                .WithSummary("Get Reviews By Room Id"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var roomId = Route<int>("RoomId");
            var result = await _mediator.Send(new GetRoomReviewsQuery(roomId), ct);

            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<List<ReviewResponseDto>>(result.Data, result.Message)
                : new FailureEndpointResult<List<ReviewResponseDto>>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
