using HotelManagement.Domain.Enums;

namespace HotelManagement.Domain.Models
{
    public class Payment : BaseModel
    {
        public int ReservationId { get; set; }
        public Reservation Reservation { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
        public PaymentStatus Status { get; set; }
        public string PaymentMethod { get; set; }
        public Invoice Invoice { get; set; }
    }
}
