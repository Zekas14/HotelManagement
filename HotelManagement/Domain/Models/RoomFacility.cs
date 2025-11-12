using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Domain.Models
{
    public class RoomFacility : BaseModel
    {
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [ForeignKey("Facility")]
        public int FacilityId { get; set; }

        public Facility Facility { get; set; }
    }
}