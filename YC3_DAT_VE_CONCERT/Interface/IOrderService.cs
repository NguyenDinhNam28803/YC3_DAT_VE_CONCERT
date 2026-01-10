namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IOrderService
    {
        // Tạo đơn đặt vé mới
        void CreateOrder(int customerId, int ticketId);

        // Cập nhật đơn đặt vé
        void UpdateOrder(int orderId, int ticketId);

        // Hủy đơn đặt vé
        void CancelOrder(int orderId);
    }
}
