using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;

namespace HotelManagement.Features.RoomManagement.Rooms.Queries.GetRooms
{
    #region Endpoint 
    public class GetRoomsEndPoint(IMediator mediator) : GetEndpoint<GetRoomsQuery>(mediator)
    {

        protected override string GetRoute() => "/apis/rooms";
        public override void Configure()
        {
            base.Configure();
            Description(b =>b
            .WithTags("Room Management")
            .Produces<IReadOnlyList<GetRoomResponseDto>>(200, "application/json")
            .Produces<FailureEndpointResult<IReadOnlyList<GetRoomResponseDto>>>(400, "application/json")
            .WithSummary("Get all rooms")
            .WithDescription("Retrieves a list of all rooms in the hotel management system.")
            );
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            var query = new GetRoomsQuery();
            var result = await mediator.Send(query, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<IReadOnlyList<GetRoomResponseDto>>(result.Data, result.Message)
                : FailureEndpointResult<IReadOnlyList<GetRoomResponseDto>>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }

    }
    #endregion
}
