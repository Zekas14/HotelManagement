using FluentValidation;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.AddRoom
{
    #region Endpoint 
    public class AddRoomEndpoint(IMediator mediator , IValidator<AddRoomCommand> validator) : PostEndpoint<AddRoomCommand, bool>(validator , mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/add";

        public override async Task HandleAsync(AddRoomCommand req, CancellationToken ct)
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

