namespace YC3_DAT_VE_CONCERT.Interface
{
    public interface IPayOSService
    {
        Task<string> CreatePaymentLink(long orderCode, decimal amount, string description, string buyerName, string buyerEmail);
        Task<object> GetPaymentInfo(long orderCode);
        Task<object> CancelPayment(long orderCode, string? reason = null);
        bool VerifyWebhookSignature(string webhookUrl, string signature);

        // Debug helper: compute expected signatures (base64 and hex) for a payload using configured checksum key.
        // Only for debugging; safe to remove later.
        Task<(string expectedBase64, string expectedHex)> ComputeExpectedSignaturesAsync(string payload);
    }
}
