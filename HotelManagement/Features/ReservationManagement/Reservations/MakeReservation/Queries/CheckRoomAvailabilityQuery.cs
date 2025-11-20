using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.MakeReservation.Queries
{
    public record CheckRoomAvailabilityQuery(int RoomId , DateTime From , DateTime To):IRequest<bool>;
    public class CheckRoomAvailabilityHandler(IGenericRepository<Reservation> repository) : IRequestHandler<CheckRoomAvailabilityQuery, bool>
    {
        private readonly IGenericRepository<Reservation> _repository = repository;
        
        public async Task<bool> Handle(CheckRoomAvailabilityQuery request, CancellationToken cancellationToken)
        { 
            var overlappingReservations =  _repository.Get(
                r => r.RoomId == request.RoomId &&
                r.CheckInDate < request.To &&
                r.CheckOutDate > request.From
            ).Any();
            return !overlappingReservations;
        }
    }
}
