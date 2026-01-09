using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Sockets;

namespace YC3_DAT_VE_CONCERT.Model
{
    [Table("customers")]
    public class Customer
    {
        [Key]
        [Column("id")]
        public int Id { get; set; }

        [Required(ErrorMessage = "Name is required")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Name must be between 2 and 100 characters")]
        [Column("name")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Email is required")]
        [EmailAddress(ErrorMessage = "Invalid email format")]
        [StringLength(255)]
        [Column("email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Invalid phone number")]
        [StringLength(20)]
        [Column("phone")]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Password is required")]
        [StringLength(255, MinimumLength = 6, ErrorMessage = "Password must be at least 6 characters")]
        [Column("password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        [Column("role_id")]
        [Required(ErrorMessage = "Role ID is required")]
        public int? RoleId { get; set; }

        // Navigation Properties
        [ForeignKey("RoleId")]
        public virtual Role Role { get; set; }
        public virtual ICollection<Order> Orders { get; set; } = new List<Order>();
        public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
    }
}
