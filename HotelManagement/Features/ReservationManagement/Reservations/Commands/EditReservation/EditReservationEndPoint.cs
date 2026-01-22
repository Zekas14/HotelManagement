using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation.Commands;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.EditReservation
{
    #region Endpoint
    public class EditReservationEndPoint(IMediator mediator, IValidator<EditReservationCommand> validator) : PutEndpoint<EditReservationCommand, bool>(mediator,validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reservations/edit";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reservation Management")
                .Accepts<EditReservationCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(200, "application/json")
                .WithSummary("Edit a Reservation")
                .WithDescription("A guest edits his reservation "));
        }
        public override async Task HandleAsync(EditReservationCommand req, CancellationToken ct)
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
