using HotelManagement.Domain.Enums;

namespace HotelManagement.Domain.Models
{
    public class Room : BaseModel 
    {
        public int RoomNumber { get; set; }
        public string? ImageUrl { get; set; }
        public RoomType Type { get; set; }
        public decimal PricePerNight { get; set; }
        public bool IsAvailable { get; set; }
        public ICollection<RoomFacility>? Facilities { get; set; }
    }
    

}