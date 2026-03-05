using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.ReportingManagement.RevenueReport
{
    public class RevenueReportEndpoint(IMediator mediator) : GetEndpoint<RevenueReportDto>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reports/revenue";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reporting and Analytics")
                .Produces<SuccessEndpointResult<RevenueReportDto>>(200, "application/json")
                .WithSummary("Get Revenue Report"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var fromDateStr = Query<string?>("from", false);
            var toDateStr = Query<string?>("to", false);
            
            DateTime? fromDate = string.IsNullOrEmpty(fromDateStr) ? null : DateTime.Parse(fromDateStr);
            DateTime? toDate = string.IsNullOrEmpty(toDateStr) ? null : DateTime.Parse(toDateStr);

            var result = await _mediator.Send(new RevenueReportQuery(fromDate, toDate), ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<RevenueReportDto>(result.Data, result.Message)
                : new FailureEndpointResult<RevenueReportDto>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
