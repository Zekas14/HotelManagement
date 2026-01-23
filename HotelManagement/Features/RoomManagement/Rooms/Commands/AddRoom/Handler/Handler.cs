using HotelManagement.Infrastructure.Data;
using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using HotelManagement.Domain.Enums;
using HotelManagement.Features.Common.Responses;

namespace HotelManagement.Features.RoomManagement.Rooms.Commands.AddRoom
{

    #region Command 
    public record AddRoomCommand(int RoomNumber, string Name ,string ImageUrl,int Capacity, string Type, decimal PricePerNight, bool IsAvailable) : IRequest<RequestResult<bool>>;

    #endregion

    #region Handler

    public class AddRoomCommandHandler(IGenericRepository<Room> repository,IMemoryCache cache) : IRequestHandler<AddRoomCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<Room> repository = repository;
        private readonly IMemoryCache cache = cache;

        public async Task<RequestResult<bool>> Handle(AddRoomCommand request, CancellationToken cancellationToken)
        {

            
            repository.Add(new Room
            {
                RoomNumber = request.RoomNumber,
                ImageUrl = request.ImageUrl,
                Type = Enum.Parse<RoomType>(request.Type),
                PricePerNight = request.PricePerNight,
                IsAvailable = request.IsAvailable,

            });
            await repository.SaveChangesAsync();
            cache.Remove("rooms");
            return RequestResult<bool>.Success(true, "Room added successfully.");
        }
    }

    #endregion
}

