using FluentValidation;
using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Data;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;
using System.Reflection.Metadata;

namespace HotelManagement.Features.RoomManagement.Facilities
{
    #region Command & Validator
    public record AddFacillityCommand(string Name): IRequest<RequestResult<bool>>;
    public class AddFacillityCommandValidator : AbstractValidator<AddFacillityCommand>
    {
        private readonly ApplicationDbContext context;

        public AddFacillityCommandValidator(ApplicationDbContext context)
        {
            this.context = context;
            RuleFor(x => x.Name)
                .NotEmpty().WithMessage("Facility name is required.")
                .MaximumLength(100).WithMessage("Facility name must not exceed 100 characters.")
                .Must(BeUniqueName).WithMessage("Faclity Name already Used");

        }
        private bool BeUniqueName(string name)
        {
            return !context.Facilities.Any(f => f.Name == name);
        }
    }
    #endregion

    #region Command Handler
    public class AddFacillityCommandHandler(IGenericRepository<Facility> repository) : IRequestHandler<AddFacillityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> repository = repository;

        public  Task<RequestResult<bool>> Handle(AddFacillityCommand request, CancellationToken cancellationToken)
        {
            var facility = new Models.Facility
            {
                Name = request.Name
            };
             repository.Add(facility);
          return Task.FromResult( RequestResult<bool>.Success(true,"Facility Added Successfully"));
        }
    }
    #endregion

    #region Endpoint 
    public class AddFacillityEndpoint( IValidator<AddFacillityCommand> validator, IMediator mediator) : PostEndpoint<AddFacillityCommand,bool>(validator,mediator)
    {
        protected override string GetRoute()=> $"{Constants.BaseApiUrl}/facilities/add";

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
