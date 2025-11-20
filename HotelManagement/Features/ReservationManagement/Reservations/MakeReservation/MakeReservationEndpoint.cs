using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement.Reservations.MakeReservation.Commands;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.MakeReservation
{
    #region Endpoint
    public class MakeReservationEndpoint(IMediator mediator ,IValidator<MakeReservationCommmand> validator) : PostEndpoint<MakeReservationCommmand, bool>(validator,mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reservations/makeReservation";
        public override async Task HandleAsync(MakeReservationCommmand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);
                return;
            }
            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);
            
        }
    }
    #endregion

}
 