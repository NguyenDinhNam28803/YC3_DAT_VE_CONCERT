using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IPdfService
    {
        byte[] GenerateTicketPdf(TicketDto ticket);
        byte[] GenerateBulkTicketsPdf(List<TicketDto> tickets);
        Task<byte[]> GenerateTicketPdfAsync(TicketDto ticket);
    }
}
