using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Queries;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.EditReservation
{
    public record EditReservationCommand(int ReservationId, DateTime? CheckIn,
        DateTime? CheckOut , int? RoomId, int? NumberOfGuests) :IRequest<RequestResult<bool>>;
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

}
