using FluentValidation;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace HotelManagement.Features.RoomManagement.Facilities
{
    #region Command
    public record DeleteFacilityCommand(int Id) : IRequest<RequestResult<bool>>;
    public class DeleteFacilityCommandValidator : AbstractValidator<DeleteFacilityCommand>
    {
        public DeleteFacilityCommandValidator()
        {
            RuleFor(x => x.Id).NotEmpty();
        }
    }
    #endregion

    #region Handler
    public class DeleteFacilityCommandHandler(IGenericRepository<Facility> repository , IMediator mediator) : IRequestHandler<DeleteFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> _repository = repository;
        private readonly IMediator _mediator = mediator;

        public async Task<RequestResult<bool>> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
        {
            var isFacilityExistsResult = await _mediator.Send(new IsEntityExistsQuery<Facility>(request.Id), cancellationToken);
            if (!isFacilityExistsResult.IsSuccess)
            {
                return RequestResult<bool>.Failure("Facility not found.");
            }
            _repository.Delete(request.Id);
            await _repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true, "Facility deleted successfully");
        }
    }
    #endregion

    #region Endpoint
    public class DeleteFacilityEndPoint(IMediator mediator, IValidator<DeleteFacilityCommand> validator) : DeleteEndpoint<DeleteFacilityCommand>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/facilities/delete/" + "{id:int}";

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
