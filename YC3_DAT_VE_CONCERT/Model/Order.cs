using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace YC3_DAT_VE_CONCERT.Model
{
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
        public string Status { get; set; } = "pending";

        [Required(ErrorMessage = "Amount is required")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0")]
        [Column("amount", TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        // Navigation Properties
        [ForeignKey("CustomerId")]
        public virtual Customer Customer { get; set; }

        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
