namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IPayOSService
    {
        Task<object> CreatePaymentLink(long orderCode, decimal amount, string description, string buyerName, string buyerEmail);
        Task<object> GetPaymentInfo(long orderCode);
        Task<object> CancelPayment(long orderCode, string? reason = null);
        bool VerifyWebhookSignature(string webhookUrl, string signature);
    }
}
