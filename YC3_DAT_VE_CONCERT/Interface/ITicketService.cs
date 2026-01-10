using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface ITicketService
    {
        // Lấy danh sách tất cả vé
        Task<List<TicketDtoResponse>> GetAllTicket();

        // Lấy vé theo userId
        Task<List<TicketUserDtoResponse>> GetTicketsByUserId(int userId);

        // Lấy vé theo id
        Task<TicketDtoResponse> GetTicketById(int id);

        // Đặt vé 
        Task<TicketUserDtoResponse> BookTicket(int userId, int orderId, TicketUserDtoRequest ticketRequest);

        // Tạo vé mới
        Task<TicketDtoRequest> CreateTicket(TicketDtoRequest ticket);

        // Cập nhật vé
        UpdateTicketDto UpdateTicket(int id, UpdateTicketDto ticket);

        // Xóa vé
        bool DeleteTicket(int id);

    }
}
