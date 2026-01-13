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
        public int Amount { get; set; }
        public string Description { get; set; }
        public string BuyerName { get; set; }
        public string BuyerEmail { get; set; }
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
