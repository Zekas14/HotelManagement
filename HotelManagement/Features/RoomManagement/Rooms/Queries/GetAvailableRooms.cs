using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.Extensions.Caching.Memory;
using HotelManagement.Domain.Enums;
using HotelManagement.Features.Common.Endpoints;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Features.Common;

namespace HotelManagement.Features.RoomManagement.Rooms.Queries
{
    #region Query
    public record GetAvailableRoomsQuery(
        int? Capacity = null,
        string? Type = null,
        decimal? MinPrice = null,
        decimal? MaxPrice = null,
        int[]? FacilityIds = null,
        bool OnlyAvailable = true
    ) : IRequest<RequestResult<IReadOnlyList<GetRoomResponseDto>>>; 
    #endregion

    #region Handler
    public class GetAvailableRoomsHandler(IGenericRepository<Room> roomRepository, IMemoryCache cache) : IRequestHandler<GetAvailableRoomsQuery, RequestResult<IReadOnlyList<GetRoomResponseDto>>>
    {
        private readonly IGenericRepository<Room> roomRepository = roomRepository;
        private readonly IMemoryCache cache = cache;

        public async Task<RequestResult<IReadOnlyList<GetRoomResponseDto>>> Handle(GetAvailableRoomsQuery request, CancellationToken cancellationToken)
        {
            // Build a cache key based on filters
            var facilityKey = request.FacilityIds is null ? "none" : string.Join('-', request.FacilityIds.OrderBy(x => x));
            var cacheKey = $"available_rooms_{request.Capacity}_{request.Type}_{request.MinPrice}_{request.MaxPrice}_{facilityKey}_{request.OnlyAvailable}";

            if (cache.TryGetValue(cacheKey, out IReadOnlyList<GetRoomResponseDto>? cached))
            {
                return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Success(cached!, "Available rooms retrieved from cache");
            }

            var query = roomRepository.Get(r=>r.IsAvailable);

          
            if (!string.IsNullOrWhiteSpace(request.Type))
            {
                if (Enum.TryParse<RoomType>(request.Type, ignoreCase: true, out var parsedType))
                {
                    query = query.Where(r => r.Type == parsedType);
                }
            }

            if (request.MinPrice.HasValue)
            {
                query = query.Where(r => r.PricePerNight >= request.MinPrice.Value);
            }

            if (request.MaxPrice.HasValue)
            {
                query = query.Where(r => r.PricePerNight <= request.MaxPrice.Value);
            }

            if (request.FacilityIds is { Length: > 0 })
            {
                var facilityIdsSet = request.FacilityIds.ToHashSet();
                query = query.Where(r => r.Facilities.Any(rf => facilityIdsSet.Contains(rf.FacilityId)));
            }

            var data = query
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
                return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Failure(ErrorCode.NotFound, "No rooms match the given preferences.");
            }

            var resultList = data.ToList();
            cache.Set(cacheKey, resultList, TimeSpan.FromMinutes(15));

            return RequestResult<IReadOnlyList<GetRoomResponseDto>>.Success(resultList, "Available rooms retrieved successfully");
        }
    }
    #endregion

    #region Endpoint
    public class GetAvailableRoomsEndpoint(IMediator mediator) : GetEndpoint<GetAvailableRoomsQuery>(mediator)
    {
        protected override string GetRoute() => $"{Constants.BaseApiUrl}/rooms/available/";

        public override async Task HandleAsync(CancellationToken ct)
        {
            int? capacity = Query<int?>("capacity", false);
            string? type = Query<string?>("type",false);
            decimal? minPrice = Query<decimal?>("minPrice", false);
            decimal? maxPrice = Query<decimal?>("maxPrice", false);
            var facilityIds = Query<List<int>>("facilityIds", false)?.ToArray();

            var query = new GetAvailableRoomsQuery(
                Capacity: capacity,
                Type: type,
                MinPrice: minPrice,
                MaxPrice: maxPrice,
                FacilityIds: facilityIds
            );


            var result = await mediator.Send(query, ct);
            IResult response = result.IsSuccess
                ? new SuccessEndpointResult<IReadOnlyList<GetRoomResponseDto>>(result.Data, result.Message)
                : FailureEndpointResult<IReadOnlyList<GetRoomResponseDto>>.BadRequest(result.Message);
            await Send.ResultAsync(response);
        }
    }
    #endregion
}