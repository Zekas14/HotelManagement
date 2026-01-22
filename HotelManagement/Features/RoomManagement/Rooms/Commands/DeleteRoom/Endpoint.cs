using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.DeleteRoom
{
    #region Endpoint 
    public class DeleteRoomEndpoint(IMediator mediator , IValidator<DeleteRoomCommand> validator) : DeleteEndpoint<DeleteRoomCommand>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/delete/"+"{id:int}";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
                .WithTags("Room Management")
                .Produces<SuccessEndpointResult<bool>>(statusCode: 200, contentType: "application/json")
                .WithSummary("Delete a room")
                .WithDescription("Deletes a room from the hotel management system by its ID."));
        }
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
