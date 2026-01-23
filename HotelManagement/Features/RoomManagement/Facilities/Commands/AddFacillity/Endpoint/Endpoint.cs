using FluentValidation;
using MediatR;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Facilities.Commands.AddFacillity
{
    #region Endpoint 
    public class AddFacillityEndpoint( IValidator<AddFacillityCommand> validator, IMediator mediator) : PostEndpoint<AddFacillityCommand,bool>(validator,mediator)
    {
        protected override string GetRoute()=> $"{Constants.BaseApiUrl}/facilities/add";
        public override void Configure()
        {
            base.Configure();
            Description(builder => builder
                .WithTags("Facilities")
                .WithSummary("Add a new facility")
                .Produces<SuccessEndpointResult<bool>>(StatusCodes.Status200OK)
                .WithDescription("Adds a new facility to the hotel management system."));
        }

        public override async Task HandleAsync(AddFacillityCommand req, CancellationToken ct)
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
