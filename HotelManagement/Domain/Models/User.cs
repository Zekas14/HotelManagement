namespace HotelManagement.Domain.Models
{
    public class User : BaseModel
    {
        public string? FullName { get; set; }
        public string? Username { get; set; }
        public string? PhoneNumber { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
    }
}
