using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Facilities.Queries.GetFacilities
{
    #region Endpoint
    public class GetFacilitiesEndpoint(IMediator mediator) : GetEndpoint<GetFacilitiesQuery>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/facilities";
        public override void Configure()
        {
            base.Configure();
            Description(builder => builder
                .WithTags("Facilities")
                .WithSummary("Get all facilities")
                .Produces<SuccessEndpointResult<IReadOnlyList<GetFacilitiesResponseDto>>>(StatusCodes.Status200OK)
                .WithDescription("Retrieves a list of all facilities available in the hotel management system."));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var query = new GetFacilitiesQuery();
            var result = await mediator.Send(query, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<IReadOnlyList<GetFacilitiesResponseDto>>(result.Data, result.Message)
                : FailureEndpointResult<IReadOnlyList<GetFacilitiesResponseDto>>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}
