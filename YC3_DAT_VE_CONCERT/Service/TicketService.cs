using System;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Model;
using YC3_DAT_VE_CONCERT.Interface;

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
        public List<Ticket> GetAllTicket()
        {
            throw new NotImplementedException();
        }

        // Lấy vé theo ID người dùng
        public List<Ticket> GetTicketsByUserId(int userId)
        {
            throw new NotImplementedException();
        }

        // Lấy vé theo ID vé
        public Ticket GetTicketById(int id)
        {
            throw new NotImplementedException();
        }

        // Tạo vé mới
        public void CreateTicket(Ticket ticket)
        {
            throw new NotImplementedException();
        }

        // Cập nhật vé
        public void UpdateTicket(int id, Ticket ticket)
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
