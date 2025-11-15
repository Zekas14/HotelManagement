using FluentValidation;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using HotelManagement.Infrastructure.Data;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Facilities
{
    #region Command
    public record UpdateFacilityCommand(int Id, string Name) : IRequest<RequestResult<bool>>;
    #endregion

    #region Validator
    public class UpdateFacilityCommandValidator : AbstractValidator<UpdateFacilityCommand>
    {

        private readonly IGenericRepository<Facility> repository;

        public UpdateFacilityCommandValidator(IGenericRepository<Facility> repository)
        {
            this.repository = repository;
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Must(BeUniqueName)
                .WithMessage("Facility name already used");
        }

        private bool BeUniqueName(string name)
        {
            return !repository.GetAll().Any(f => f.Name == name);
        }
    }
    #endregion

    #region Handler
    public class UpdateFacilityCommandHandler(IGenericRepository<Facility> repository, IMediator mediator) : IRequestHandler<UpdateFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> _repository = repository;
        private readonly IMediator mediator = mediator;

        public async Task<RequestResult<bool>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
        {
           var isFacilityExists=  await mediator.Send(new IsEntityExistsQuery<Facility>(request.Id), cancellationToken);
            if (!isFacilityExists.IsSuccess)
            {
                return RequestResult<bool>.Failure(isFacilityExists.Message);
            }
            _repository.SaveInclude(isFacilityExists.Data, nameof(Facility.Name));
            await _repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true, "Facility updated successfully");
        }
    }
    #endregion

    #region Endpoint
    public class UpdateFacilityEndPoint(IMediator mediator, IValidator<UpdateFacilityCommand> validator) : PostEndpoint<UpdateFacilityCommand, bool>(validator, mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/facilities/update";

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
