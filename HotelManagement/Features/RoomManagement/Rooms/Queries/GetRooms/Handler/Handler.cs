using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Queries.GetRooms
{
    #region Query
    public record GetRoomsQuery :IRequest<RequestResult<IReadOnlyList<GetRoomResponseDto>>>;

    #endregion

    #region Handler 
    public class GetRoomsHandler(IGenericRepository<Room> roomRepository, IMemoryCache cache) : IRequestHandler<GetRoomsQuery, RequestResult<IReadOnlyList<GetRoomResponseDto>>>
    {
        private readonly IGenericRepository<Room> roomRepository = roomRepository;
        private readonly IMemoryCache cache = cache;
        public async Task<RequestResult<IReadOnlyList<GetRoomResponseDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            if (cache.TryGetValue("rooms", out IReadOnlyList<GetRoomResponseDto>? cachedRooms))
            {
                return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Success(cachedRooms!, "Rooms retrieved successfully from cache");
            }

            var data = roomRepository.GetAll()
                .Select(e => new GetRoomResponseDto
                {
                    Id = e.Id,
                    ImageUrl = e.ImageUrl,
                    IsAvailable = e.IsAvailable,
                    RoomNumber = e.RoomNumber,
                    Facilities = e.Facilities!.Select(rf => rf.Facility!.Name),
                    Type = e.Type.ToString(),
                    PricePerNight = e.PricePerNight,
                    CreatedDate = e.CreatedAt.ToString(Constants.DateTimeFormat),
                });
            if (!data.Any())
            {
                return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Failure(ErrorCode.NotFound,"No rooms found");
            } ;
            cache.Set("rooms", data.ToList(), TimeSpan.FromMinutes(20));
            return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Success([.. data], "Rooms retrieved successfully");
        }
    }
    #endregion
}
