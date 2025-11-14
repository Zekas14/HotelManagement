using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Events
{
    public record MakeReservationEvent(int RoomId): INotification;
}
