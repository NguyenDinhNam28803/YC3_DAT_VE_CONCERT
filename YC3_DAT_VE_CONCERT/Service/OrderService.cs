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
        private readonly IPayOSService _payOSService;
        public OrderService(ApplicationDbContext context, IPayOSService payOSService)
        {
            _context = context;
            _payOSService = payOSService;
        }
        
        public async Task<List<OrderResponseDto>> GetAllOrders()
        {
            try
            {
                var orders = await _context.Orders
                    .Include(o => o.Tickets)
                    .Select(o => new OrderResponseDto
                    {
                        Id = o.Id,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer.Name,
                        OrderDate = o.OrderDate,
                        Status = o.Status.ToString(),
                        TotalAmount = o.Tickets.Where(t => t.OrderId == o.Id).Sum(t => t.Price).ToString("C"),
                        TotalTickets = o.Tickets.Count(),
                        Tickets = o.Tickets.Select(t => new TicketUserDtoResponse
                        {
                            Id = t.Id,
                            EventName = t.Event.Name,
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
                    .Include(o => o.Tickets)
                    .Where(o => o.CustomerId == userId)
                    .Select(o => new OrderResponseDto
                    {
                        Id = o.Id,
                        CustomerId = o.CustomerId,
                        CustomerName = o.Customer.Name,
                        OrderDate = o.OrderDate,
                        Status = o.Status.ToString(),
                        TotalAmount = o.Tickets.Where(t => t.OrderId == o.Id).Sum(t => t.Price).ToString("C"),
                        TotalTickets = o.Tickets.Count(),
                        PaymentLink = o.PaymentLink,
                        Tickets = _context.Tickets
                            .Include(t => t.Customer)
                            .Where(t => t.OrderId == o.Id)
                            .Select(t => new TicketUserDtoResponse
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
                // 1. Validate customer
                var customer = await _context.Customers
                    .AsNoTracking()
                    .FirstOrDefaultAsync(c => c.Id == orderDto.CustomerId);

                if (customer == null)
                {
                    throw new Exception($"Customer ID {orderDto.CustomerId} not found.");
                }

                // 2. Validate tất cả tickets trước
                var ticketIds = orderDto.Tickets.Select(t => t.TicketId).ToList();
                var tickets = await _context.Tickets
                    .Include(t => t.Event)
                    .Where(t => ticketIds.Contains(t.Id))
                    .ToListAsync();

                if (tickets.Count != ticketIds.Count)
                {
                    throw new Exception("Some tickets not found.");
                }

                // Validate tickets nếu 1 người đặt 1 vé nhiều lần
                var checkDuplicateTicketIds = ticketIds.GroupBy(id => id)
                    .Where(g => g.Count() > 1)
                    .Select(g => g.Key)
                    .ToList();
                if (checkDuplicateTicketIds.Count > 0)
                {
                    throw new Exception($"Duplicate ticket IDs in the order: {string.Join(", ", checkDuplicateTicketIds)}");
                }

                foreach (var ticket in tickets)
                {
                    if (ticket.Status == TicketStatus.Sold)
                    {
                        throw new Exception($"Ticket ID {ticket.Id} (Seat {ticket.SeatNumber}) is already sold.");
                    }
                }

                // 3. Tạo Order
                var order = new Order
                {
                    CustomerId = orderDto.CustomerId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    PaymentLink = string.Empty
                };

                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // 4. Update tất cả tickets
                foreach (var ticket in tickets)
                {
                    ticket.CustomerId = orderDto.CustomerId;
                    ticket.OrderId = order.Id;
                    ticket.Status = TicketStatus.Sold;
                    ticket.PurchaseDate = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();

                // 5. Update order status
                order.TotalTickets = tickets.Count;
                order.TotalAmount = tickets.Sum(t => t.Price);
                var paymentLink = await _payOSService.CreatePaymentLink(
                    orderCode: order.Id,
                    amount: order.Tickets.Where(t => t.OrderId == order.Id).Sum(t => t.Price),
                    description: $"Payment for Order ID {order.Id}",
                    buyerName: customer.Name,
                    buyerEmail: customer.Email
                );
                if(paymentLink == null)
                {
                    throw new Exception("Failed to create payment link.");
                }
                order.PaymentLink = paymentLink;
                await _context.SaveChangesAsync();

                // 6. Prepare response
                var bookedTickets = tickets.Select(t => new TicketUserDtoResponse
                {
                    Id = t.Id,
                    EventName = t.Event.Name,
                    EventDate = t.Event.Date,
                    SeatNumber = t.SeatNumber,
                    Price = t.Price,
                    PurchaseDate = t.PurchaseDate ?? DateTime.MinValue
                }).ToList();

                return new OrderResponseDto
                {
                    Id = order.Id,
                    CustomerId = order.CustomerId,
                    CustomerName = customer.Name,
                    OrderDate = order.OrderDate,
                    Status = order.Status.ToString(),
                    TotalAmount = order.Tickets.Where(t => t.OrderId == order.Id).Sum(t => t.Price).ToString("C"),
                    TotalTickets = order.Tickets.Count(),
                    PaymentLink = paymentLink,
                    Tickets = bookedTickets
                };
            }
            catch (Exception ex)
            {
                var fullError = ex.ToString(); // Lấy full error
                Console.WriteLine(fullError);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.ToString());
                }

                throw new Exception($"Error creating order: {ex.Message}", ex);
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
