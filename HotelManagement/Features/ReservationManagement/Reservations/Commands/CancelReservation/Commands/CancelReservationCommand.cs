using FluentValidation;
using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.CancelReservation.Queries;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.EditReservation;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.CancelReservation.Commands
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

}
