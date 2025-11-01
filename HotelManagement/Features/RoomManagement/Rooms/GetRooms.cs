using HotelManagement.Common.Modules;
using HotelManagement.Common.Responses;
using HotelManagement.Common.Responses.EndpointResults;
using HotelManagement.Data.Repositories;
using HotelManagement.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;

namespace HotelManagement.Features.RoomManagement.Rooms
{
    #region Query
    public record GetRoomsQuery :IRequest<RequestResult<IReadOnlyList<GetRoomsResponseDto>>>;
    #endregion

    #region Dto
    public record GetRoomsResponseDto
    {
        public int RoomNumber { get; set; }
        public string  Name { get; set; }
        public string  ImageUrl { get; set; }
        public string CreatedDate { get; set; }
        public int Capacity { get; set; }
        public string Type { get; set; }
        public bool IsAvailable { get; set; }
    }
    #endregion

    #region Handler 
    public class GetRoomsHandler(IGenericRepository<Room> roomRepository, IMemoryCache cache) : IRequestHandler<GetRoomsQuery, RequestResult<IReadOnlyList<GetRoomsResponseDto>>>
    {
        private readonly IGenericRepository<Room> roomRepository = roomRepository;
        private readonly IMemoryCache cache = cache;
        public async Task<RequestResult<IReadOnlyList<GetRoomsResponseDto>>> Handle(GetRoomsQuery request, CancellationToken cancellationToken)
        {
            if (cache.TryGetValue("rooms", out IReadOnlyList<GetRoomsResponseDto>? cachedRooms))
            {
                return RequestResult<IReadOnlyList<GetRoomsResponseDto>>.Success(cachedRooms!, "Rooms retrieved successfully from cache");
            }

            var data = roomRepository.GetAll()
                .Select(e => new GetRoomsResponseDto
                {
                    Capacity = e.Capacity,
                    ImageUrl = e.ImageUrl,
                    IsAvailable = e.IsAvailable,
                    RoomNumber = e.RoomNumber,
                    Name = e.Name,
                    Type = e.Type.ToString(),
                    CreatedDate = e.CreatedAt.ToShortDateString(),
                });
            if (!data.Any())
            {
                return RequestResult<IReadOnlyList<GetRoomsResponseDto>>.Failure("No rooms found");
            } ;
            cache.Set("rooms", data.ToList(), TimeSpan.FromMinutes(20));
            return RequestResult<IReadOnlyList<GetRoomsResponseDto>>.Success([.. data], "Rooms retrieved successfully");
        }
    }
    #endregion

    #region Endpoint 
    public class GetRoomsEndPoint : GetModule<GetRoomsQuery>
    {

        protected override string Route => "/apis/rooms";


        protected override Delegate Handler => new Func<IMediator, Task<IResult>>(async (mediator) =>
        {
            var result = await mediator.Send(new GetRoomsQuery());
            return result.IsSuccess
                ? new SuccessEndpointResult<IReadOnlyList<GetRoomsResponseDto>>(result.Data, result.Message)
                : FailureEndpointResult<IReadOnlyList<GetRoomsResponseDto>>.NotFound("No Rooms Are Founded");
        });

    }
    #endregion
}
