using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface ITicketPdfService
    {
        Task<TicketDto> GetTicketByIdAsync(int ticketId);
        Task<List<TicketDto>> GetTicketsByOrderIdAsync(int orderId);
        Task<byte[]> GenerateTicketPdfAsync(int ticketId);
        Task<byte[]> GenerateOrderTicketsPdfAsync(int orderId);
        Task<bool> SendTicketEmailAsync(int ticketId);
    }
}
