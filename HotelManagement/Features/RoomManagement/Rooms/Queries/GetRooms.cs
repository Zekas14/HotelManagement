using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using System.Reflection.Metadata;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Queries
{
    #region Query
    public record GetRoomsQuery :IRequest<RequestResult<IReadOnlyList<GetRoomResponseDto>>>;
    #endregion

    #region Dto
    public record GetRoomResponseDto
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public string  ImageUrl { get; set; }
        public string CreatedDate { get; set; }
        public string Type { get; set; }
        public IEnumerable<string>? Facilities { get; set; }
        public bool IsAvailable { get; set; }
    }
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
                    CreatedDate = e.CreatedAt.ToString(Constants.DateTimeFormat),
                });
            if (!data.Any())
            {
                return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Failure("No rooms found");
            } ;
            cache.Set("rooms", data.ToList(), TimeSpan.FromMinutes(20));
            return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Success([.. data], "Rooms retrieved successfully");
        }
    }
    #endregion

    #region Endpoint 
    public class GetRoomsEndPoint(IMediator mediator) : GetEndpoint<GetRoomsQuery>(mediator)
    {

        protected override string GetRoute() => "/apis/rooms";


        public override async Task HandleAsync(CancellationToken ct)
        {
            var query = new GetRoomsQuery();
            var result = await mediator.Send(query, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<IReadOnlyList<GetRoomResponseDto>>(result.Data, result.Message)
                : FailureEndpointResult<IReadOnlyList<GetRoomResponseDto>>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }

    }
    #endregion
}
