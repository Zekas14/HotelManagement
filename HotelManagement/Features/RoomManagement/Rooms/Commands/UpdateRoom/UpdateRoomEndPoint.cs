using FluentValidation;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.UpdateRoom
{
    #region Endpoint 
    public class UpdateRoomEndPoint(IMediator mediator,IValidator<UpdateRoomCommand> validator ) : PutEndpoint<UpdateRoomCommand,bool>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/update";
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
