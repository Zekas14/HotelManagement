using FluentValidation;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.UpdateFacility
{
    #region Endpoint
    public class UpdateFacilityEndpoint(IMediator mediator, IValidator<UpdateFacilityCommand> validator) : PostEndpoint<UpdateFacilityCommand, bool>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/facilities/update";
        public override void Configure()
        {
            base.Configure();
            Description(b => b
            .WithTags("Facilities")
            .WithSummary("Update Facility")
            .WithDescription("Update an existing facility")
            .Accepts<UpdateFacilityCommand>("application/json")
            .Produces<SuccessEndpointResult<bool>>(StatusCodes.Status200OK, "application/json")
            );
        }

        public override async Task HandleAsync(UpdateFacilityCommand req, CancellationToken ct)
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
