using FluentValidation;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.RoomManagement.Rooms.Commands.AddRoom;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation
{
    #region Endpoint
    public class MakeReservationEndpoint(IMediator mediator ,IValidator<MakeReservationCommand> validator) : PostEndpoint<MakeReservationCommand, bool>(validator,mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reservations/makeReservation";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Reservation Management")
                .Accepts<MakeReservationCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(200, "application/json")
                .WithSummary("Make a New Reservation")
                .WithDescription("A guest reserve a  room for a certain duration"));


        }
        public override async Task HandleAsync(MakeReservationCommand req, CancellationToken ct)
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
 