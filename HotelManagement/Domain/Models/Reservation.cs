namespace HotelManagement.Domain.Models
{
    public class Reservation : BaseModel
    {
        public DateTime CheckInDate { get; set; }
        public DateTime CheckOutDate { get; set; }
        public int RoomId { get; set; }
        public Room? Room { get; set; }
        public int GuestId { get; set; }
        public Guest? Guest { get; set; }
        public int NumberOfGuests { get; set; }
        public decimal TotalPrice { get; set; }
       
    }

}
