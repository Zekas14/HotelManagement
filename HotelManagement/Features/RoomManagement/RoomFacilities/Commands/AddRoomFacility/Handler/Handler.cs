using HotelManagement.Infrastructure.Data.Repositories;
using HotelManagement.Domain.Models;
using MediatR;
using Microsoft.CodeAnalysis;
using Microsoft.Extensions.Caching.Memory;
using HotelManagement.Features.Common.Responses;
using HotelManagement.Features.Common.Responses.EndpointResults;

namespace HotelManagement.Features.RoomManagement.RoomFacilities.Commands.AddRoomFacility
{
    #region Command
    public record AddRoomFacilityCommand(int RoomId, int FacilityId) : IRequest<RequestResult<bool>>;
    #endregion

    #region Handler
    public class AddRoomFacilityCommandHandler(IGenericRepository<RoomFacility> repository, IMemoryCache cache) : IRequestHandler<AddRoomFacilityCommand, RequestResult<bool>>
    {
        private readonly IGenericRepository<RoomFacility> _repository = repository;
        private readonly IMemoryCache _cache = cache;

        public async Task<RequestResult<bool>> Handle(AddRoomFacilityCommand request, CancellationToken cancellationToken)
        {
            var IsFacilityAssignedToRoom= _repository.GetAll()
                .Any(rf => rf.RoomId == request.RoomId && rf.FacilityId == request.FacilityId);
                
            if (IsFacilityAssignedToRoom)
                return RequestResult<bool>.Failure(ErrorCode.FacilityAlreadyAssignedToRoom, "Facility already assigned to room.");
            _repository.Add(new RoomFacility
            {
                RoomId = request.RoomId,
                FacilityId = request.FacilityId
            });
            _repository.SaveChanges();
            _cache.Remove("rooms");
            
            return RequestResult<bool>.Success(true, "Facility assigned to room successfully");
        }
    }

    #endregion
}
