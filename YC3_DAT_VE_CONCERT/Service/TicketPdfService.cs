using Microsoft.EntityFrameworkCore;
using PayOS.Exceptions;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class TicketPdfService : ITicketPdfService
    {
        private readonly ApplicationDbContext _context;
        private readonly IPdfService _pdfService;
        private readonly IEmailService _emailService;
        private readonly ILogger<TicketPdfService> _logger;

        public TicketPdfService(
           ApplicationDbContext context,
           IPdfService pdfService,
           IEmailService emailService,
           ILogger<TicketPdfService> logger)
        {
            _context = context;
            _pdfService = pdfService;
            _emailService = emailService;
            _logger = logger;
        }

        public async Task<TicketDto> GetTicketByIdAsync(int ticketId)
        {
            var ticket = await _context.Tickets
                .Include(t => t.Order)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Venue)
                .FirstOrDefaultAsync(t => t.Id == ticketId);

            if (ticket == null)
                throw new NotFoundException($"Ticket {ticketId} not found");

            return MapToDto(ticket);
        }

        public async Task<List<TicketDto>> GetTicketsByOrderIdAsync(int orderId)
        {
            var tickets = await _context.Tickets
                .Include(t => t.Order)
                .Include(t => t.Event)
                    .ThenInclude(e => e.Venue)
                .Where(t => t.OrderId == orderId)
                .ToListAsync();

            return tickets.Select(MapToDto).ToList();
        }

        public async Task<byte[]> GenerateTicketPdfAsync(int ticketId)
        {
            var ticketDto = await GetTicketByIdAsync(ticketId);
            return await _pdfService.GenerateTicketPdfAsync(ticketDto);
        }

        public async Task<byte[]> GenerateOrderTicketsPdfAsync(int orderId)
        {
            var tickets = await GetTicketsByOrderIdAsync(orderId);

            if (tickets.Count == 0)
                throw new NotFoundException($"No tickets found for order {orderId}");

            return _pdfService.GenerateBulkTicketsPdf(tickets);
        }

        public async Task<bool> SendTicketEmailAsync(int ticketId)
        {
            try
            {
                var ticketDto = await GetTicketByIdAsync(ticketId);
                var pdfBytes = await _pdfService.GenerateTicketPdfAsync(ticketDto);

                var attachments = new List<EmailAttachment>
            {
                new EmailAttachment
                {
                    FileName = $"Ticket_{ticketId}.pdf",
                    ContentType = "application/pdf",
                    Content = pdfBytes
                }
            };

                await _emailService.SendTicketEmailAsync(
                    ticketDto.CustomerName,
                    ticketDto.CustomerEmail,
                    ticketDto.EventName,
                    ticketDto.EventDate,
                    ticketDto.SeatNumber,
                    attachments
                );

                _logger.LogInformation($"Ticket email sent successfully for {ticketId}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Failed to send ticket email for {ticketId}");
                return false;
            }
        }

        private TicketDto MapToDto(Ticket ticket)
        {
            return new TicketDto
            {
                TicketId = ticket.Id,
                EventName = ticket.Event.Name,
                EventDate = ticket.Event.Date,
                VenueName = ticket.Event.Venue.Name,
                VenueAddress = ticket.Event.Venue.Location,
                SeatNumber = ticket.SeatNumber,
                Price = ticket.Price,
                CustomerName = ticket.Order.Customer.Name,
                CustomerEmail = ticket.Order.Customer.Email,
                QrCodeData = $"TICKET:{ticket.Id}|EVENT:{ticket.EventId}|SEAT:{ticket.SeatNumber}|ORDER:{ticket.OrderId}",
                PurchasedDate = ticket.PurchaseDate,
            };
        }
    }
}
