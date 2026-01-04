using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;
using HotelManagement.Features.RoomManagement.Rooms.Queries.GetRooms;

namespace HotelManagement.Features.RoomManagement.Rooms.Queries.GetAvailableRooms
{
    #region Endpoint
    public class GetAvailableRoomsEndpoint(IMediator mediator) : GetEndpoint<GetAvailableRoomsQuery>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/available/";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Room Management")
                .Produces<IReadOnlyList<GetRoomResponseDto>>(200, "application/json")
                .Produces<FailureEndpointResult<IReadOnlyList<GetRoomResponseDto>>>(400, "application/json")
                .WithSummary("Get Available Rooms")
                .WithDescription("Retrieves a list of available rooms based on optional filtering criteria such as capacity, type, price range, and facilities."));

        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            int? capacity = Query<int?>("capacity", false);
            string? type = Query<string?>("type",false);
            decimal? minPrice = Query<decimal?>("minPrice", false);
            decimal? maxPrice = Query<decimal?>("maxPrice", false);
            var facilityIds = Query<List<int>>("facilityIds", false)?.ToArray();

            var query = new GetAvailableRoomsQuery(
                Capacity: capacity,
                Type: type,
                MinPrice: minPrice,
                MaxPrice: maxPrice,
                FacilityIds: facilityIds
            );


            var result = await mediator.Send(query, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<IReadOnlyList<GetRoomResponseDto>>(result.Data, result.Message)
                : FailureEndpointResult<IReadOnlyList<GetRoomResponseDto>>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}