using MediatR;

namespace HotelManagement.Features.RoomManagement.RoomFacilities.Queries
{
    public record GetRoomFacilities(int Id) :IRequest<List<RoomFacilityDto>>
    {

    }

    public class RoomFacilityDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
    }
}
