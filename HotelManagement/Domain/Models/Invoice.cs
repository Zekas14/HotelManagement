namespace HotelManagement.Domain.Models
{
    public class Invoice : BaseModel
    {
        public int PaymentId { get; set; }
        public Payment Payment { get; set; }
        public string InvoiceNumber { get; set; }
        public DateTime IssuedDate { get; set; }
        public DateTime DueDate { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
