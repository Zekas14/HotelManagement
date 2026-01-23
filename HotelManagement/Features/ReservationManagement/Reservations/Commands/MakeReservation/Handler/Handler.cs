using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation.Events;
using HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation.Queries;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation
{
    #region Command
    public record MakeReservationCommand   (
        DateTime CheckInDate,
        DateTime CheckOutDate,
        int RoomId,
        int GuestId,
        int NumberOfGuests,
        decimal PricePerNight
    ) : IRequest<RequestResult<bool>>;

    #endregion

    #region Handler

    public class MakeReservationHandler(IGenericRepository<Reservation> repository, IMediator mediator) : IRequestHandler<MakeReservationCommand, RequestResult<bool>>
    {
            private readonly IGenericRepository<Reservation> repository = repository;
            private readonly IMediator mediator = mediator;

            public async Task<RequestResult<bool>> Handle(MakeReservationCommand request, CancellationToken cancellationToken)
            {
            var isRoomAvailable = await mediator.Send(new CheckRoomAvailabilityQuery(request.RoomId, request.CheckInDate, request.CheckOutDate), cancellationToken);
            if (!isRoomAvailable)
            {
                return RequestResult<bool>.Failure(ErrorCode.RoomIsUnavailable,"Room is not available for the selected dates.");
            }
            var reservation = new Reservation
            {
                CheckInDate = request.CheckInDate,
                CheckOutDate = request.CheckOutDate,
                RoomId = request.RoomId,
                GuestId = request.GuestId,
                NumberOfGuests = request.NumberOfGuests,
                TotalPrice = ((request.CheckOutDate - request.CheckInDate).Days) * request.PricePerNight
            };
            repository.Add(reservation);
            await mediator.Publish(new OnMakingReservationEvent(request.RoomId));
            return RequestResult<bool>.Success(true);
        }

    }

    #endregion
    
}
 