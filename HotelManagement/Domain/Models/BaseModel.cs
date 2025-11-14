namespace HotelManagement.Domain.Models
{
    public class BaseModel
    {
        public int Id { get; set; }

        public bool IsDeleted { get; set; } = false;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public int CreatedBy { get; set; }

    }
}
