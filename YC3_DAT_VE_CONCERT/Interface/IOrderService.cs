using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IOrderService
    {
        Task<List<OrderResponseDto>> GetAllOrders();

        Task<OrderResponseDto> GetOrderById(int orderId);

        Task<List<OrderResponseDto>> GetOrdersByUserId(int userId);
        // Tạo đơn đặt vé mới
        Task<OrderResponseDto> CreateOrder(CreateOrderDto orderDto);

        // Cập nhật đơn đặt vé
        Task<OrderResponseDto> UpdateOrder(int orderId, UpdateOrderStatusDto ticketId);

        // Hủy đơn đặt vé
        Task<bool> CancelOrder(int orderId);
    }
}
