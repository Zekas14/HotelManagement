using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.MakeReservation.Events
{
    public record OnMakingReservationEvent(int RoomId): INotification;
}
