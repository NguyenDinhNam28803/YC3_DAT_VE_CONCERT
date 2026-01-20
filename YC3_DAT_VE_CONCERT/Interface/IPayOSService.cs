using PayOS.Models.Webhooks;

namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IPayOSService
    {
        Task<string> CreatePaymentLink(long orderCode, decimal amount, string description, string buyerName, string buyerEmail);
        Task<object> GetPaymentInfo(Webhook webhookData);
        Task<object> CancelPayment(long orderCode, string? reason = null);
    }
}
