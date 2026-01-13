using System.Text.Json.Serialization;

namespace YC3_DAT_VE_CONCERT.Model
{
    public class CreatePaymentRequest
    {
        public string ProductName { get; set; }
        public string Description { get; set; }
        public int Amount { get; set; }
        public string ReturnUrl { get; set; }
        public string CancelUrl { get; set; }
    }

    public class PaymentLinkRequestModel
    {
        public long OrderCode { get; set; }
        public decimal Amount { get; set; }
        public string Description { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
    }

    public class PaymentLinkResponseModel
    {
        [JsonPropertyName("bin")]
        public string? Bin { get; set; }

        [JsonPropertyName("accountNumber")]
        public string? AccountNumber { get; set; }

        [JsonPropertyName("accountName")]
        public string? AccountName { get; set; }

        // amount in sample is an integer (smallest unit or VND units) - use long to be safe
        [JsonPropertyName("amount")]
        public long Amount { get; set; }

        [JsonPropertyName("description")]
        public string? Description { get; set; }

        [JsonPropertyName("orderCode")]
        public long OrderCode { get; set; }

        [JsonPropertyName("currency")]
        public string? Currency { get; set; }

        [JsonPropertyName("paymentLinkId")]
        public string? PaymentLinkId { get; set; }

        // expiredAt can be null or a timestamp string -> DateTime? with custom parsing if needed
        [JsonPropertyName("expiredAt")]
        public DateTime? ExpiredAt { get; set; }

        [JsonPropertyName("status")]
        public string? Status { get; set; }

        [JsonPropertyName("checkoutUrl")]
        public string? CheckoutUrl { get; set; }

        [JsonPropertyName("qrCode")]
        public string? QrCode { get; set; }
    }

    // Response model trả về cho client
    public class PaymentResponse
    {
        public bool Success { get; set; }
        public string Message { get; set; }
        public string CheckoutUrl { get; set; }
        public long OrderCode { get; set; }
    }

    // Model để xử lý webhook callback
    public class PaymentWebhookData
    {
        public string Code { get; set; }
        public string Desc { get; set; }
        public bool Success { get; set; }
        public PaymentWebhookDataInfo Data { get; set; }
        public string Signature { get; set; }
    }

    public class PaymentWebhookDataInfo
    {
        public long OrderCode { get; set; }
        public int Amount { get; set; }
        public string Description { get; set; }
        public string AccountNumber { get; set; }
        public string Reference { get; set; }
        public string TransactionDateTime { get; set; }
    }
}
