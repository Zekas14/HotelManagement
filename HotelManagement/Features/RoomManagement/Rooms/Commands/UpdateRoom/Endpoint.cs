using FluentValidation;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.UpdateRoom
{
    #region Endpoint 
    public class UpdateRoomEndpoint(IMediator mediator,IValidator<UpdateRoomCommand> validator ) : PutEndpoint<UpdateRoomCommand,bool>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/update";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Room Management")
                .WithSummary("Update a room")
                .Accepts<UpdateRoomCommand>("application/json")
                .Produces<SuccessEndpointResult<bool>>(statusCode: 200, contentType: "application/json")
                .WithDescription("Updates the details of an existing room in the hotel management system."));
        }
        override public async Task HandleAsync(UpdateRoomCommand req, CancellationToken ct)
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
