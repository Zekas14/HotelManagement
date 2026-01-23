namespace HotelManagement.Features.RoomManagement.Rooms.Queries
{
    public record GetRoomResponseDto
    {
        public int Id { get; set; }
        public int RoomNumber { get; set; }
        public string  ImageUrl { get; set; }
        public string CreatedDate { get; set; }
        public string Type { get; set; }
        public IEnumerable<string>? Facilities { get; set; }
        public bool IsAvailable { get; set; }
        public decimal PricePerNight { get; set; }
    }
}
