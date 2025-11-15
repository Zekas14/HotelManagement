using HotelManagement.Domain.Models;
using HotelManagement.Features.ReservationManagement.Reservations.Events;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.RoomManagement.Rooms.EventHandlers
{
    public class MakeReservationEventHandler(IGenericRepository<Room> repository) : INotificationHandler<MakeReservationEvent>
    {
        private readonly IGenericRepository<Room> repository = repository;

        public async Task Handle(MakeReservationEvent notification, CancellationToken cancellationToken)
        {
            var room = new Room
            {
                Id = notification.RoomId,
                IsAvailable = false,
            };
            repository.SaveInclude(room,nameof(room.IsAvailable));
            repository.SaveChanges();
        }
    }
}
