namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IOrderService
    {
        void CreateOrder(int customerId, int ticketId);
        void UpdateOrder(int orderId, int ticketId);
        void CancelOrder(int orderId);
    }
}
