using System.ComponentModel.DataAnnotations;

namespace YC3_DAT_VE_CONCERT.Dto
{
    public class CustomerResponseDto
    {
        public int Id { get; set; }
        public string Role { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
        public string? Phone { get; set; }
        public int TotalOrders { get; set; }
        public int TotalTickets { get; set; }
    }

    public class UpdatePasswordDto
    {
        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }
        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }

    public class UpdateCustomerDto
    {
        [Required]
        [StringLength(100, MinimumLength = 2)]
        public string Name { get; set; }

        [Phone]
        public string? Phone { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string CurrentPassword { get; set; }

        [Required]
        [DataType(DataType.Password)]
        public string NewPassword { get; set; }

        [StringLength(100, MinimumLength = 6)]
        [DataType(DataType.Password)]
        public string? ConfirmPassword { get; set; }
    }
}
