using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement.Reservations.Queries.ViewReservation;
using HotelManagement.Features.RoomManagement.Rooms.Queries;
using MediatR;

namespace HotelManagement.Features.RoomManagement.RoomFacilities.Queries.GetRoomFacilities
{
    #region Endpoint 
    public class GetRoomFacilitiesEndpoint(IMediator mediator) : GetEndpoint<IReadOnlyList<RoomFacilitiesResponseDto>>(mediator)
    {
        public override void Configure()
        {
            base.Configure();
            Description(b => b
             .WithTags("Room Facilities")
             .Produces<SuccessEndpointResult<ViewReservationResponseDto>>(200, "application/json")
             .WithSummary("Get Facilities For a certain room "));

        }
        protected override string GetRoute()=> "/rooms/{RoomId}/facilities";
        public async override Task HandleAsync(CancellationToken ct)
        {
            var roomId = Route<int>("RoomId");
            var result = await mediator.Send(new GetRoomFacilitiesQuery(roomId), ct);
            IResult response = result.IsSuccess ? new SuccessEndpointResult<IReadOnlyList<RoomFacilitiesResponseDto>>(result.Data, result.Message) :
                new FailureEndpointResult<IReadOnlyList<GetRoomResponseDto>>(result.ErrorCode, result.Message);
            await Send.ResultAsync(response);
        }
  

    }

    #endregion
}


