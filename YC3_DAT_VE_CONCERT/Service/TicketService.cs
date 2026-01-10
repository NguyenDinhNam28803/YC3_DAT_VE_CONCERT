using Microsoft.EntityFrameworkCore;
using System;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class TicketService : ITicketService
    {
        private readonly ApplicationDbContext _context;
        public TicketService(ApplicationDbContext context)
        {
            _context = context;
        }

        // Lấy danh sách tất cả vé
        public async Task<List<TicketDtoResponse>> GetAllTicket()
        {
            try
            {
                var listTickets = await _context.Tickets
                    .Include(t => t.Event)
                    .Select(t => new TicketDtoResponse
                    {
                        Id = t.Id,
                        EventName = t.Event.Name,
                        SeatNumber = t.SeatNumber,
                        EventDate = t.Event.Date,
                        Price = t.Price
                    }).ToListAsync();

                return listTickets;
            } 
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving tickets.", ex);
            }
        }

        // Lấy vé theo ID người dùng
        public async Task<List<TicketUserDtoResponse>> GetTicketsByUserId(int userId)
        {
            try
            {
                var tickets = await _context.Tickets
                    .Where(t => t.CustomerId == userId)
                    .Include(t => t.Event)
                    .Include(t => t.Order)
                        .ThenInclude(o => o.Customer)
                    .Select(t => new TicketUserDtoResponse
                    {
                        Id = t.Id,
                        EventName = t.Event.Name,
                        UserName = t.Customer.Name,
                        EventDate = t.Event.Date,
                        SeatNumber = t.SeatNumber,
                        Price = t.Price,
                        PurchaseDate = t.PurchaseDate ?? DateTime.MinValue
                    }).ToListAsync();

                return tickets;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while retrieving tickets by user ID.", ex);
            }
        }

        // Lấy vé theo ID vé
        public async Task<TicketDtoResponse> GetTicketById(int id)
        {
            throw new NotImplementedException();
        }

        // Đặt vé
        public async Task<TicketUserDtoResponse> BookTicket(int userId, int orderId, TicketUserDtoRequest ticketRequest)
        {
            try
            {
                var ticket = await _context.Tickets.FindAsync(ticketRequest.TicketId);
                if (ticket == null)
                {
                    throw new Exception("Ticket not found.");
                }

                if (ticket.Status == TicketStatus.Sold)
                {
                    throw new Exception("Ticket is already sold.");
                }

                ticket.CustomerId = userId;
                ticket.OrderId = orderId;
                ticket.Status = TicketStatus.Sold;
                ticket.PurchaseDate = DateTime.UtcNow;
                await _context.SaveChangesAsync();
                
                var ticketResponse = new TicketUserDtoResponse
                {
                    Id = ticket.Id,
                    EventName = (await _context.Events.FindAsync(ticket.EventId))?.Name ?? "Unknown Event",
                    UserName = (await _context.Customers.FindAsync(userId))?.Name ?? "Unknown User",
                    EventDate = (await _context.Events.FindAsync(ticket.EventId))?.Date ?? DateTime.MinValue,
                    SeatNumber = ticket.SeatNumber,
                    Price = ticket.Price,
                    PurchaseDate = ticket.PurchaseDate ?? DateTime.MinValue
                };
                return ticketResponse;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while booking the ticket.", ex);
            }
        }

        // Tạo vé mới
        public async Task<TicketDtoRequest> CreateTicket(TicketDtoRequest ticket)
        {
            try
            {
                var eventexist = await _context.Events.FindAsync(ticket.EventId);
                if (eventexist == null)
                {
                    throw new Exception("Event not found.");
                }

                var seatExists = await _context.Tickets
                    .AnyAsync(t => t.EventId == ticket.EventId && t.SeatNumber == ticket.SeatNumber);
                if (seatExists)
                {
                    throw new Exception("Seat number already exists for this event.");
                }
                var newTicket = new Ticket
                {
                    EventId = eventexist.Id,
                    SeatNumber = ticket.SeatNumber,
                    Price = eventexist.TicketPrice,
                    Status = TicketStatus.Available
                   
                };
                _context.Tickets.Add(newTicket);
                await _context.SaveChangesAsync();
                return ticket;
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred while creating the ticket.", ex);
            }
        }

        // Cập nhật vé
        public UpdateTicketDto UpdateTicket(int id, UpdateTicketDto ticket)
        {
            throw new NotImplementedException();
        }

        // Xóa vé
        public bool DeleteTicket(int id)
        {
            throw new NotImplementedException();
        }
    }
}
