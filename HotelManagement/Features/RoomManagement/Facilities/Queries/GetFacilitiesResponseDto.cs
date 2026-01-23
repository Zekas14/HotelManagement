namespace HotelManagement.Features.RoomManagement.Facilities.Queries
{
    public record GetFacilitiesResponseDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string CreatedDate { get; set; }
    }

}