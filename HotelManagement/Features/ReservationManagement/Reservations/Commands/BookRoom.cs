using HotelManagement.Domain.Models;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;

namespace HotelManagement.Features.ReservationManagement.Reservations.Commands
{
    public record BookRoomCommand(int RoomId) : IRequest<bool>;
    public class BookRoomCommandHandler(IGenericRepository<Room> repository) : IRequestHandler<BookRoomCommand, bool>
    {
        private readonly IGenericRepository<Room> repository = repository;

        public Task<bool> Handle(BookRoomCommand request, CancellationToken cancellationToken)
        { 
           
        }
    }
}
