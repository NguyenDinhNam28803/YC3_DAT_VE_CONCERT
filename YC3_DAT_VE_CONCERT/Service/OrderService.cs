using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Data;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        public OrderService(ApplicationDbContext context)
        {
            _context = context;
        }

        public void CreateOrder(int customerId, int ticketId)
        {
            throw new NotImplementedException();
        }

        public void UpdateOrder(int orderId, int ticketId)
        {
            throw new NotImplementedException();
        }

        public void CancelOrder(int orderId)
        {
            throw new NotImplementedException();
        }
    }
}
