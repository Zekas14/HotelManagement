using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.ReportingManagement.BookingsReport
{
    public class BookingsReportEndpoint(IMediator mediator) : GetEndpoint<BookingsReportDto>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reports/bookings";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reporting and Analytics")
                .Produces<SuccessEndpointResult<BookingsReportDto>>(200, "application/json")
                .WithSummary("Get Booking Report"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var fromDateStr = Query<string?>("from", false);
            var toDateStr = Query<string?>("to", false);
            
            DateTime? fromDate = string.IsNullOrEmpty(fromDateStr) ? null : DateTime.Parse(fromDateStr);
            DateTime? toDate = string.IsNullOrEmpty(toDateStr) ? null : DateTime.Parse(toDateStr);

            var result = await _mediator.Send(new BookingsReportQuery(fromDate, toDate), ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<BookingsReportDto>(result.Data, result.Message)
                : new FailureEndpointResult<BookingsReportDto>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
