using FluentValidation;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.RoomFacilities.Commands.AddRoomFacility
{
    #region Endpoint
    public class AddRoomFacilityEndPoint(IValidator<AddRoomFacilityCommand> validator, IMediator mediator) : PostEndpoint<AddRoomFacilityCommand, bool>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/roomfacility/add";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Room Facilities")
                .WithSummary("Add Facility to Room")
                .Accepts<AddRoomFacilityCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(StatusCodes.Status200OK, "application/json")
                .WithDescription("Adds a facility to a specific room."));
        }

        public override async Task HandleAsync(AddRoomFacilityCommand req, CancellationToken ct)
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
