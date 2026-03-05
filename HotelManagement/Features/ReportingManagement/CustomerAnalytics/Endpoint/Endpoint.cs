using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.ReportingManagement.CustomerAnalytics
{
    public class CustomerAnalyticsEndpoint(IMediator mediator) : GetEndpoint<CustomerAnalyticsDto>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reports/customer-analytics";

        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reporting and Analytics")
                .Produces<SuccessEndpointResult<CustomerAnalyticsDto>>(200, "application/json")
                .WithSummary("Get Customer Analytics"));
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var result = await _mediator.Send(new CustomerAnalyticsQuery(), ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<CustomerAnalyticsDto>(result.Data, result.Message)
                : new FailureEndpointResult<CustomerAnalyticsDto>(result.ErrorCode, result.Message);

            await Send.ResultAsync(response);
        }
    }
}
