using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.ReportingManagement.OccupancyAnalytics
{
    public class OccupancyAnalyticsEndpoint(IMediator mediator) : GetEndpoint<OccupancyAnalyticsDto>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reports/occupancy";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reporting and Analytics")
                .Produces<SuccessEndpointResult<OccupancyAnalyticsDto>>(200, "application/json")
                .WithSummary("Get Occupancy Analytics"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var fromDateStr = Query<string?>("from", false);
            var toDateStr = Query<string?>("to", false);
            
            DateTime? fromDate = string.IsNullOrEmpty(fromDateStr) ? null : DateTime.Parse(fromDateStr);
            DateTime? toDate = string.IsNullOrEmpty(toDateStr) ? null : DateTime.Parse(toDateStr);

            var result = await _mediator.Send(new OccupancyAnalyticsQuery(fromDate, toDate), ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<OccupancyAnalyticsDto>(result.Data, result.Message)
                : new FailureEndpointResult<OccupancyAnalyticsDto>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
