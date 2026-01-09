using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YC3_DAT_VE_CONCERT.Model
{
    [Table("tickets")]
    public class Ticket
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Event is required")]
        [Column("event_id")]
        public int EventId { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Purchase date is required")]
        [DataType(DataType.DateTime)]
        [Column("purchase_date")]
        public DateTime PurchaseDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Seat number is required")]
        [StringLength(10)]
        [Column("seat_number")]
        [RegularExpression("^[A-Z][0-9]{1,3}$",
            ErrorMessage = "Seat number must be in format: A1, B12, etc.")]
        public string SeatNumber { get; set; }

        [Column("order_id")]
        [Required(ErrorMessage = "Order is required")]
        public int OrderId { get; set; }

        // Navigation Properties
        [ForeignKey("EventId")]
        public virtual Event Event { get; set; }

        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        [ForeignKey("OrderId")]
        public virtual Order Order { get; set; }
    }
}
