using FluentValidation;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;
using HotelManagement.Data;

namespace HotelManagement.Features.RoomManagement.Facilities
{
    #region Command
    public record UpdateFacilityCommand(int Id, string Name) : IRequest<RequestResult<bool>>;
    #endregion

    #region Validator
    public class UpdateFacilityCommandValidator : AbstractValidator<UpdateFacilityCommand>
    {
        private readonly ApplicationDbContext _context;

        public UpdateFacilityCommandValidator(ApplicationDbContext context)
        {
            _context = context;
            RuleFor(x => x.Id).GreaterThan(0);
            RuleFor(x => x.Name)
                .NotEmpty()
                .MaximumLength(100)
                .Must((cmd, name) => BeUniqueName(cmd.Id, name))
                .WithMessage("Facility name already used");
        }

        private bool BeUniqueName(int id, string name)
        {
            return !_context.Facilities.Any(f => f.Name == name && f.Id != id);
        }
    }
    #endregion

    #region Handler
    public class UpdateFacilityCommandHandler(IGenericRepository<Facility> repository) : IRequestHandler<UpdateFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> _repository = repository;

        public async Task<RequestResult<bool>> Handle(UpdateFacilityCommand request, CancellationToken cancellationToken)
        {
            var facility = _repository.GetById(request.Id);
            if (facility == null)
            {
                return RequestResult<bool>.Failure("Facility not found.");
            }

            facility.Name = request.Name;
            _repository.SaveInclude(facility, nameof(Facility.Name));
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
