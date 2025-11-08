using System.ComponentModel.DataAnnotations.Schema;

namespace HotelManagement.Models
{
    public class RoomFacility
    {
        public int Id { get; set; }
        [ForeignKey("Room")]
        public int RoomId { get; set; }
        public Room Room { get; set; }

        [ForeignKey("Facility")]
        public int FacilityId { get; set; }

        public Facility Facility { get; set; }
    }
    
}

