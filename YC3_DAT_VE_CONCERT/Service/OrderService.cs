using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Model;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http.HttpResults;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class OrderService : IOrderService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPayOSService _payOSService;
        private readonly IEmailService _emailService;
        public OrderService(ApplicationDbContext context, IEmailService emailService, IPayOSService payOSService)
        {
            _context = context;
            _emailService = emailService;
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

        public async Task<OrderResponseDto> GetOrderById(int orderId)
        {
            try
            {
                var existingOrder = await _context.Orders
                    .Include(o => o.Tickets)
                    .ThenInclude(t => t.Customer)
                    .FirstOrDefaultAsync(o => o.Id == orderId);

                if (existingOrder == null)
                {
                    throw new Exception($"Order ID {orderId} not found.");
                }

                var orderResponse = new OrderResponseDto
                {
                    Id = existingOrder.Id,
                    CustomerId = existingOrder.CustomerId,
                    CustomerName = existingOrder.Customer.Name,
                    OrderDate = existingOrder.OrderDate,
                    Status = existingOrder.Status.ToString(),
                    TotalAmount = existingOrder.Tickets.Where(t => t.OrderId == existingOrder.Id).Sum(t => t.Price).ToString("C"),
                    TotalTickets = existingOrder.Tickets.Count(),
                    Tickets = existingOrder.Tickets.Select(t => new TicketUserDtoResponse
                    {
                        Id = t.Id,
                        EventName = t.Event.Name,
                        UserName = t.Customer.Name,
                        EventDate = t.Event.Date,
                        SeatNumber = t.SeatNumber,
                        Price = t.Price,
                        PurchaseDate = t.PurchaseDate ?? DateTime.MinValue
                    }).ToList()
                };

                return orderResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving the order.", ex);
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

                var new_order = await _context.Orders
                    .Include(o => o.Tickets)
                        .ThenInclude(t => t.Event)
                    .Include(o => o.Customer)
                    .FirstOrDefaultAsync(o => o.Id == order.Id);

                if (new_order == null)
                {
                    throw new Exception("Order not found after creation.");
                }

                // Build data for email
                var seatInfoArray = bookedTickets.Select(t => t.SeatNumber).FirstOrDefault(); // Sửa lại là có thể thêm nhiêu vé cùng lúc
                var eventName = bookedTickets.FirstOrDefault()?.EventName ?? "Nhiều sự kiện";
                var eventDate = bookedTickets.FirstOrDefault()?.EventDate ?? DateTime.UtcNow;
                var orderIdString = new_order.Id.ToString();
                var totalAmountDecimal = new_order.TotalAmount;

                if (seatInfoArray == null)
                {
                    throw new Exception("No seat information available for email.");
                }

                // Send confirmation email (uses EmailService overload that accepts seat array and optional payment link)
                await _emailService.SendOrderConfirmationEmail(
                    customer.Name,
                    customer.Email,
                    orderIdString,
                    eventName,
                    eventDate,
                    seatInfoArray,
                    totalAmountDecimal,
                    paymentLink
                );

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
        public async Task<OrderResponseDto> UpdateOrder(int orderId, UpdateOrderStatusDto ticketId)
        {
            try
            {
                var existingOrder = await _context.Orders
                    .FirstOrDefaultAsync(o => o.Id == orderId);
                if (existingOrder == null)
                {
                    throw new Exception($"Order ID {orderId} not found.");
                }

                existingOrder.Status = Enum.Parse<OrderStatus>(ticketId.Status);
                _context.Orders.Update(existingOrder);
                await _context.SaveChangesAsync();
                return await GetOrderById(orderId);
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while updating the order.", ex);
            }
        }

        public async Task<bool> CancelOrder(int orderId)
        {
            try
            {
                var order = await _context.Orders
                    .Include(o => o.Tickets)
                    .FirstOrDefaultAsync(o => o.Id == orderId);
                if (order == null)
                {
                    throw new Exception($"Order ID {orderId} not found.");
                }
                if (order.Status == OrderStatus.Cancelled)
                {
                    throw new Exception($"Order ID {orderId} is already canceled.");
                }

                // Update order status
                order.Status = OrderStatus.Cancelled;
                _context.Orders.Update(order);
                await _context.SaveChangesAsync();
                // Update all tickets in the order
                foreach (var ticket in order.Tickets)
                {
                    ticket.Status = TicketStatus.Available; // Assuming Available is the status for unsold tickets
                    ticket.CustomerId = null; // Clear customer association
                    ticket.OrderId = null; // Clear order association
                }

                _context.Tickets.UpdateRange(order.Tickets);
                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                // Log the exception (logging mechanism not shown here)
                throw new ApplicationException("An error occurred while canceling the order.", ex);
            }
        }
    }
}
