using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface ITicketService
    {
        // Lấy danh sách tất cả vé
        List<TicketDtoResponse> GetAllTicket();

        // Lấy vé theo userId
        List<TicketDtoResponse> GetTicketsByUserId(int userId);

        // Lấy vé theo id
        TicketDtoResponse GetTicketById(int id);

        // Tạo vé mới
        TicketDtoRequest CreateTicket(Ticket ticket);

        // Cập nhật vé
        UpdateTicketDto UpdateTicket(int id, Ticket ticket);

        // Xóa vé
        bool DeleteTicket(int id);

    }
}
