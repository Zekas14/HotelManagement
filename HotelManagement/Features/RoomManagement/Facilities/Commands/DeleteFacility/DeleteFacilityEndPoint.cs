using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.DeleteFacility
{
    #region Endpoint
    public class DeleteFacilityEndPoint(IMediator mediator, IValidator<DeleteFacilityCommand> validator) : DeleteEndpoint<DeleteFacilityCommand>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/facilities/delete/" + "{id:int}";
        public override void Configure()
        {
            base.Configure();
            Description(builder => builder
                .WithTags("Facilities")
                .WithSummary("Delete a facility")
                .Produces<SuccessEndpointResult<bool>>(StatusCodes.Status200OK)
                .WithDescription("Deletes a facility from the hotel management system by its ID."));
        }

        public override async Task HandleAsync([FromRoute] DeleteFacilityCommand req, CancellationToken ct)
        {
            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}
