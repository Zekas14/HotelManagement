using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Queries.ViewReservation
{
    #region Endpoint
    public class ViewReservationEndPoint(IMediator mediator) : GetEndpoint<ViewReservationResponseDto>(mediator)
    {
        protected override string GetRoute()=> "apis/reservations/{ReservationId:int}";
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
