using HotelManagement.Domain.Models;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;
using HotelManagement.Infrastructure.Data.Repositories;
using MediatR;
using static FastEndpoints.Ep;

namespace HotelManagement.Features.RoomManagement.RoomFacilities.Queries.GetRoomFacilities
{
    public record GetRoomFacilitiesQuery(int RoomId) :IRequest<RequestResult<IReadOnlyList<RoomFacilitiesResponseDto>>>;
    public class RoomFacilitiesResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
    public class GetRoomFacilitiesQueryHandler(IGenericRepository<RoomFacility> repository) :IRequestHandler<GetRoomFacilitiesQuery, RequestResult<IReadOnlyList<RoomFacilitiesResponseDto>>>
    {
        private readonly IGenericRepository<RoomFacility> repository = repository;

        public  async Task<RequestResult<IReadOnlyList<RoomFacilitiesResponseDto>>> Handle(GetRoomFacilitiesQuery request, CancellationToken cancellationToken)
        {
            if (repository.GetById(request.RoomId) is null)
            {
                return RequestResult<IReadOnlyList<RoomFacilitiesResponseDto>>.Failure(ErrorCode.NotFound, "Room not found or has no facilities assigned");
            }
            var RoomFacilities = repository.Get(f => f.RoomId == request.RoomId)
                .Select(rf => new RoomFacilitiesResponseDto
                {
                    Id = rf.Facility.Id,
                    Name = rf.Facility.Name
                });
            return RequestResult<IReadOnlyList<RoomFacilitiesResponseDto>>.Success([.. RoomFacilities]);
        }
    }

}

