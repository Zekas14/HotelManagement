using HotelManagement.Common;
using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using FluentValidation;

namespace HotelManagement.Features.RoomManagement.Facilities
{
    #region Command
    public record DeleteFacilityCommand(int Id) : IRequest<RequestResult<bool>>;
    #endregion

    #region Handler
    public class DeleteFacilityCommandHandler(IGenericRepository<Facility> repository) : IRequestHandler<DeleteFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Facility> _repository = repository;

        public async Task<RequestResult<bool>> Handle(DeleteFacilityCommand request, CancellationToken cancellationToken)
        {
            var facility = _repository.GetById(request.Id);
            if (facility == null)
            {
                return RequestResult<bool>.Failure("Facility not found.");
            }

            _repository.Delete(facility);
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
