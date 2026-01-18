using YC3_DAT_VE_CONCERT.Dto;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IEmailService
    {
        Task SendEmail(string name, string email, string title, string text);
        Task SendOrderConfirmationEmail(string toName, string toEmail, string orderId, string concertName, DateTime concertDate, string seatInfo, decimal totalAmount, string? paymentLink = null);

        // ... existing methods
        Task SendTicketEmailAsync(
            string toName,
            string toEmail,
            string eventName,
            DateTime eventDate,
            string seatInfo,
            List<EmailAttachment> attachments);
        Task SendEmailWithAttachmentsAsync(string toEmail, string subject, string body, List<EmailAttachment> attachments);
    }
}
