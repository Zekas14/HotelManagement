namespace HotelManagement.Domain.Models
{
    public class Review : BaseModel
    {
        public int GuestId { get; set; }
        public Guest Guest { get; set; } = null!;

        public int RoomId { get; set; }
        public Room Room { get; set; } = null!;

        public int Rating { get; set; }
        public string? Comment { get; set; }
        public string? StaffResponse { get; set; }
        public DateTime? ResponseDate { get; set; }
    }
}
