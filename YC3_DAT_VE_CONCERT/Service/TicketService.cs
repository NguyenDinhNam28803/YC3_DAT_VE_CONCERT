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
        public List<TicketDtoResponse> GetAllTicket()
        {
            throw new NotImplementedException();
        }

        // Lấy vé theo ID người dùng
        public List<TicketDtoResponse> GetTicketsByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        // Lấy vé theo ID vé
        public TicketDtoResponse GetTicketById(int id)
        {
            throw new NotImplementedException();
        }

        // Tạo vé mới
        public TicketDtoRequest CreateTicket(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        // Cập nhật vé
        public UpdateTicketDto UpdateTicket(int id, Ticket ticket)
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
