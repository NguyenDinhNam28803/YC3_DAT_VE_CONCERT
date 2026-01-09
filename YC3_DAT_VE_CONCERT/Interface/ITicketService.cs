using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface ITicketService
    {

        List<Ticket> GetAllTicket();
        List<Ticket> GetTicketsByUserId(int userId);
        Ticket GetTicketById(int id);
        void CreateTicket(Ticket ticket);
        void UpdateTicket(int id, Ticket ticket);
        bool DeleteTicket(int id);

    }
}
