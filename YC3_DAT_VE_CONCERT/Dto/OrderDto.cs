using System.ComponentModel.DataAnnotations;

namespace YC3_DAT_VE_CONCERT.Dto
{
    // tạo đơn hàng
    public class CreateOrderDto
    {
        [Required]
        [Range(1, int.MaxValue)]
        public int CustomerId { get; set; }

        [Required]
        [MinLength(1, ErrorMessage = "At least one ticket is required")]
        public List<TicketUserDtoRequest> Tickets { get; set; }
    }

    // cập nhật trạng thái đơn hàng
    public class UpdateOrderStatusDto
    {
        [Required]
        [RegularExpression("^(pending|completed|cancelled)$")]
        public string Status { get; set; }
    }

    // kiểu trả về Order
    public class OrderResponseDto
    {
        public int Id { get; set; }
        public int CustomerId { get; set; }
        public string CustomerName { get; set; }
        public DateTime OrderDate { get; set; }
        public string Status { get; set; }
        public List<TicketUserDtoResponse> Tickets { get; set; }
    }
}
