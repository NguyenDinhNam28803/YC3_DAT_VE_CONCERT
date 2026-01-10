using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Model;
using Microsoft.EntityFrameworkCore;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly ITicketService _ticketService;
        public OrderService(ApplicationDbContext context, ITicketService ticketService)
        {
            _context = context;
            _ticketService = ticketService;
        }
        
        public async Task<List<OrderResponseDto>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Select(o => new OrderResponseDto
                    {
                        Id = o.Id,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer.Name,
                        OrderDate = o.OrderDate,
                        Status = o.Status.ToString(),
                        Tickets = o.Tickets.Select(t => new TicketUserDtoResponse
                        {
                            Id = t.Id,
                            EventName = t.Event.Name,
                            UserName = t.Customer.Name,
                            EventDate = t.Event.Date,
                            SeatNumber = t.SeatNumber,
                            Price = t.Price,
                            PurchaseDate = t.PurchaseDate ?? DateTime.MinValue
                        }).ToList()
                    }).ToListAsync();
                if (orders == null || orders.Count == 0)
                {
                    throw new Exception("No orders found.");
                }
                return orders;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving orders.", ex);
            }
        }

        public async Task<List<OrderResponseDto>> GetOrdersByUserId(int userId)
        {
            try
            {
                var listOrders = await _context.Orders
                    .Where(o => o.CustomerId == userId)
                    .Select(o => new OrderResponseDto
                    {
                        Id = o.Id,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer.Name,
                        OrderDate = o.OrderDate,
                        Status = o.Status.ToString(),
                        Tickets = o.Tickets.Select(t => new TicketUserDtoResponse
                        {
                            Id = t.Id,
                            EventName = t.Event.Name,
                            UserName = t.Customer.Name,
                            EventDate = t.Event.Date,
                            SeatNumber = t.SeatNumber,
                            Price = t.Price,
                            PurchaseDate = t.PurchaseDate ?? DateTime.MinValue
                        }).ToList()
                    }).ToListAsync();

                return listOrders;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving orders for the user.", ex);
            }
        }

        public async Task<OrderResponseDto> CreateOrder(CreateOrderDto orderDto)
        {
            try
            {
                var order = new Order
                {
                    CustomerId = orderDto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending
                };
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                foreach (var ticketRequest in orderDto.Tickets)
                {
                    await _ticketService.BookTicket(orderDto.CustomerId, order.Id, ticketRequest);
                }
                var createdOrder = await _context.Orders
                    .Where(o => o.Id == order.Id)
                    .Select(o => new OrderResponseDto
                    {
                        Id = o.Id,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer.Name,
                        OrderDate = o.OrderDate,
                        Status = o.Status.ToString(),
                        Tickets = o.Tickets.Select(t => new TicketUserDtoResponse
                        {
                            Id = t.Id,
                            EventName = t.Event.Name,
                            UserName = t.Customer.Name,
                            EventDate = t.Event.Date,
                            SeatNumber = t.SeatNumber,
                            Price = t.Price,
                            PurchaseDate = t.PurchaseDate ?? DateTime.MinValue
                        }).ToList()
                    }).FirstOrDefaultAsync();
                if (createdOrder == null)
                {
                    throw new Exception("Order creation failed.");
                }
                return createdOrder;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the order.", ex);
            }
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
