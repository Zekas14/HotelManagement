using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands.MakeReservation.Events
{
    public record OnMakingReservationEvent(int RoomId): INotification;
}
