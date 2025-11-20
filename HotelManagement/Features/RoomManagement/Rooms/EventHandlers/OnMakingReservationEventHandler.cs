using HotelManagement.Domain.Models;
using HotelManagement.Features.ReservationManagement.Reservations.MakeReservation.Events;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.RoomManagement.Rooms.EventHandlers
{
    public class OnMakingReservationEventHandler(IGenericRepository<Room> repository) : INotificationHandler<OnMakingReservationEvent>
    {
        private readonly IGenericRepository<Room> repository = repository;

        public async Task Handle(OnMakingReservationEvent notification, CancellationToken cancellationToken)
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
