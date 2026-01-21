namespace HotelManagement.Domain.Models
{
    public class Role : BaseModel
    {
        public string? Name { get; set; }
        public ICollection<UserRole>? UserRoles { get; set; }
    }
}
