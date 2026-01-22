using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.CancelReservation.Commands;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.CancelReservation
{
    #region Endpoint
    public class CancelReservationEndpoint(IMediator mediator , IValidator<CancelReservationCommand> validator) :DeleteEndpoint<CancelReservationCommand>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reservations/cancel";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reservation Management")
                .Accepts<CancelReservationCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(200, "application/json")
                .WithSummary("Cancel a Reservation")
                .WithDescription("A guest Cancels his reservation "));
        }
        public override async Task HandleAsync(CancelReservationCommand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);

            }
            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);

        }
    }
    #endregion

}
