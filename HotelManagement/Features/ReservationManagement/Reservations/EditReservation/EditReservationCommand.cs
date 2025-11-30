using FluentValidation;
using HotelManagement.Domain.Models;
using HotelManagement.Features.Common;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.EditReservation
{
    public record EditReservationCommand(int ReservationId, DateTime? CheckIn,
        DateTime? CheckOut , int? RoomId, int? NumberOfGuests) :IRequest<RequestResult<bool>>;


    #region Validator
    public class EditReservationCommandValidator : AbstractValidator<EditReservationCommand>
    {
        private readonly IGenericRepository<Reservation> _repository;

        public EditReservationCommandValidator(IGenericRepository<Reservation> repository)
        {
            _repository = repository;

            RuleFor(x => x.ReservationId).GreaterThan(0);
            RuleFor(x => x.CheckIn)
                .LessThan(x => x.CheckOut)
                .WithMessage("Check-in must be before check-out.");
            RuleFor(x => x.CheckOut)
                .GreaterThan(x => x.CheckIn)
                .WithMessage("Check-out must be after check-in.");
            RuleFor(x => x.RoomId)
                .GreaterThan(0);
            RuleFor(x => x.NumberOfGuests)
                .GreaterThan(0)
                .WithMessage("Number of guests must be at least 1.");

        }
    }
    #endregion
    public class EditReservationCommandHandler(IGenericRepository<Reservation> repository, IMediator mediator) : IRequestHandler<EditReservationCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Reservation> _repository = repository;
        private readonly IMediator _mediator = mediator;

        public async Task<RequestResult<bool>> Handle(EditReservationCommand request, CancellationToken cancellationToken)
        {
            var reservationExists = await _mediator.Send(new EntityExistsQuery<Reservation>(request.ReservationId),cancellationToken);
            if (!reservationExists.IsSuccess)
            {
                return RequestResult<bool>.Failure(reservationExists.ErrorCode,reservationExists.Message);
            }
            Reservation reservation = new Reservation
            {
                Id = request.ReservationId,
                CheckInDate = request.CheckIn ?? reservationExists.Data.CheckInDate,
                CheckOutDate = request.CheckOut ?? reservationExists.Data.CheckOutDate,
                RoomId = request.RoomId ?? reservationExists.Data.RoomId,
                NumberOfGuests = request.NumberOfGuests ?? reservationExists.Data.NumberOfGuests,
            };
            _repository.SaveInclude(reservation, nameof(Reservation.CheckInDate), nameof(Reservation.CheckOutDate),nameof(Reservation.RoomId),nameof(Reservation.NumberOfGuests));
            return RequestResult<bool>.Success(true, "Reservation updated successfully.");
        }
    }


    #region Endpoint
    public class EditReservationEndPoint(IMediator mediator, IValidator<EditReservationCommand> validator) : PutEndpoint<EditReservationCommand, bool>(mediator,validator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/reservations/edit";

        public override async Task HandleAsync(EditReservationCommand req, CancellationToken ct)
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
