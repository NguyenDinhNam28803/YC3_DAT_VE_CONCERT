namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IPdfService
    {
        byte[] GenerateTicketPdf(TicketPdfDto ticketData);
        byte[] GenerateInvoicePdf(InvoicePdfDto invoiceData);
    }
}
