namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IEmailService
    {
        Task SendEmail(string name, string email, string title, string text);
        Task SendOrderConfirmationEmail(string toName, string toEmail, string orderId, string concertName, DateTime concertDate, string seatInfo, decimal totalAmount, string? paymentLink = null);
    }
}
