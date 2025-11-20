using FluentValidation;
using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement.Reservations.CancelReservation.Queries;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.CancelReservation.Commands
{
    #region CancelReservationCommand
    public record CancelReservationCommand(int ReservationId, string Notes) : IRequest<RequestResult<bool>>;
    #endregion

    #region Validator
    public class CancelReservationValidator : AbstractValidator<CancelReservationCommand>
    {
        public CancelReservationValidator()
        {
            RuleFor(x => x.ReservationId).GreaterThan(0).WithMessage("ReservationId must be greater than 0");
            RuleFor(x => x.Notes).MaximumLength(500).WithMessage("Notes cannot exceed 500 characters");
        }
    }
    #endregion

    #region Handler 
    public class CancelReservationCommandHandler(IGenericRepository<Reservation> repository,IMediator mediator) : IRequestHandler<CancelReservationCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Reservation> _repository = repository;
        private readonly IMediator _mediator = mediator;

        public async Task<RequestResult<bool>> Handle(CancelReservationCommand request, CancellationToken cancellationToken)
        {
            var isReservationAllowedTobeCancelledResult = await _mediator.Send(new IsReservationAllowedTobeCancelledQuery(request.ReservationId), cancellationToken);
            if(!isReservationAllowedTobeCancelledResult.IsSuccess)
            {
                return RequestResult<bool>.Failure(isReservationAllowedTobeCancelledResult.ErrorCode, isReservationAllowedTobeCancelledResult.Message);
            }
            _repository.Delete(request.ReservationId);
            await _repository.SaveChangesAsync();
            return RequestResult<bool>.Success(true,"Reservation cancelled successfully");
        }
    }
    #endregion
    #region Endpoint
    public class CancelReservationEndpoint(IMediator mediator , IValidator<CancelReservationCommand> validator) :DeleteEndpoint<CancelReservationCommand>(mediator, validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reservations/cancel";
        public override async Task HandleAsync(CancelReservationCommand req, CancellationToken ct)
        {
            var validationResult = await Validate(req);
            if (!validationResult.IsSuccess)
            {
                await Send.ResultAsync(validationResult);

            }
            var result = await mediator.Send(req, ct);
            IResult response = result.IsSuccess ? new SuccessEndpointResult<bool>(result.Data, result.Message)
                : FailureEndpointResult<bool>.BadRequest(result.Message);
            await Send.ResultAsync(response);

        }
    }
    #endregion

}
