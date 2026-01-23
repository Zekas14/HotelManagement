using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Queries.ViewReservation
{
    #region Endpoint
    public class ViewReservationEndpoint(IMediator mediator) : GetEndpoint<ViewReservationResponseDto>(mediator)
    {
        protected override string GetRoute()=> "apis/reservations/{ReservationId:int}";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reservation Management")
                .Produces< SuccessEndpointResult<ViewReservationResponseDto>>(200, "application/json")
                .WithSummary("View  Reservation")
                .WithDescription("A guest or stuff Can view  a reservation "));
        }
        public override async Task HandleAsync(CancellationToken ct)
        {
            var reservationId = Route<int>("ReservationId");
            var result = await _mediator.Send(new ViewReservationQuery(reservationId), ct);
            IResult response =result.IsSuccess? new SuccessEndpointResult<ViewReservationResponseDto>(result.Data!, result.Message):
                new FailureEndpointResult<ViewReservationResponseDto>(result.ErrorCode,result.Message);
            await Send.ResultAsync(response);
        }

    }
    #endregion
}
