using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.DeleteRoom
{
    #region Endpoint 
    public class DeleteRoomEndPoint(IMediator mediator , IValidator<DeleteRoomCommand> validator) : DeleteEndpoint<DeleteRoomCommand>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/delete/"+"{id:int}";
        override public async Task HandleAsync([FromRoute] DeleteRoomCommand req, CancellationToken ct)
        {
            var result =  await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}
