using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YC3_DAT_VE_CONCERT.Model
{
    public enum OrderStatus
    {
        Pending,
        Completed,
        Cancelled
    }
    [Table("orders")]
    public class Order
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Customer is required")]
        [Column("customer_id")]
        public int CustomerId { get; set; }

        [Required(ErrorMessage = "Order date is required")]
        [DataType(DataType.DateTime)]
        [Column("order_date")]
        public DateTime OrderDate { get; set; } = DateTime.UtcNow;

        [Required(ErrorMessage = "Status is required")]
        [StringLength(50)]
        [Column("status")]
        [RegularExpression("^(pending|completed|cancelled)$",
            ErrorMessage = "Status must be pending, completed, or cancelled")]
        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        // Navigation Properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
