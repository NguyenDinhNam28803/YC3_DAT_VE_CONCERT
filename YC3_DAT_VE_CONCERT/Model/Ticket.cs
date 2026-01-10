using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YC3_DAT_VE_CONCERT.Model
{
    public enum TicketStatus
    {
        Available, // Có sẵn
        Sold, // Đã bán
        Cancelled // Đã hủy
    }
       
    [Table("tickets")]
    public class Ticket
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event is required")]
        [Column("event_id")]
        public int EventId { get; set; }
        [Column("customer_id")]
        public int? CustomerId { get; set; }

        [Required(ErrorMessage = "Price is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Price must be greater than 0")]
        [Column("price")]
        public decimal Price { get; set; }

        [DataType(DataType.DateTime)]
        [Column("purchase_date")]
        public DateTime? PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Seat number is required")]
        [StringLength(10)]
        [Column("seat_number")]
        [RegularExpression("^[A-Z][0-9]{1,3}$",
            ErrorMessage = "Seat number must be in format: A1, B12, etc.")]
        public string SeatNumber { get; set; }

        [Column("order_id")]
        public int? OrderId { get; set; }

        [Required(ErrorMessage = "Status is required")]
        [Column("status")]
        public TicketStatus Status { get; set; } = TicketStatus.Available;

        // Navigation Properties
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}
