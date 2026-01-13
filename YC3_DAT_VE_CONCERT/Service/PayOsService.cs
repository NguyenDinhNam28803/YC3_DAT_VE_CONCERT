using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Resources.V2.PaymentRequests;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Service
{
    public class PayOsService : IPayOSService
    {
        private readonly PayOSClient _payOSClient;
        private readonly string _checksumKey;

        public PayOsService(IConfiguration configuration)
        {
            if (configuration == null) throw new ArgumentNullException(nameof(configuration));

            var clientId = configuration["PayOS:ClientId"] ?? throw new ArgumentNullException("PayOS:ClientId");
            var apiKey = configuration["PayOS:ApiKey"] ?? throw new ArgumentNullException("PayOS:ApiKey");
            _checksumKey = configuration["PayOS:ChecksumKey"] ?? throw new ArgumentNullException("PayOS:ChecksumKey");

            var options = new PayOS.PayOSOptions
            {
                ClientId = clientId,
                ApiKey = apiKey,
                ChecksumKey = _checksumKey
            };

            _payOSClient = new PayOSClient(options);
        }

        // IMPORTANT: method name must match the interface IPayOSService.CreatePaymentLink
        public async Task<string> CreatePaymentLink(long orderCode, decimal amount, string description, string buyerName, string buyerEmail)
        {
            try
            {
                var amount_int = Convert.ToInt32(amount * 100); // Convert to smallest currency unit
                                                                // Use SDK request type explicitly to avoid ambiguous reference with your local model
                var sdkRequest = new CreatePaymentLinkRequest
                {
                    OrderCode = orderCode,
                    Description = description,
                    Amount = amount_int,
                    ReturnUrl = "https://your-return-url.com",
                    CancelUrl = "https://your-cancel-url.com"
                };

                var response = await _payOSClient.PaymentRequests.CreateAsync(sdkRequest);
                return response.CheckoutUrl;
            }
            catch (Exception ex)
            {
                throw new Exception("Error creating payment link: " + ex.Message);
            }
        }

        // Placeholder - implement according to SDK methods
        public Task<object> GetPaymentInfo(long orderCode)
        {
            throw new NotImplementedException();
        }

        // Placeholder - implement according to SDK methods
        public Task<object> CancelPayment(long orderCode, string? reason = null)
        {
            throw new NotImplementedException();
        }

        // Verify webhook signature using stored checksum key (HMAC-SHA256 -> Base64)
        public bool VerifyWebhookSignature(string webhookUrl, string signature)
        {
            if (string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(_checksumKey) || webhookUrl == null)
                return false;

            using var hmac = new HMACSHA256(Encoding.UTF8.GetBytes(_checksumKey));
            var hash = hmac.ComputeHash(Encoding.UTF8.GetBytes(webhookUrl));
            var expected = Convert.ToBase64String(hash);

            return SecureEquals(expected, signature);
        }

        private static bool SecureEquals(string a, string b)
        {
            if (a == null || b == null) return false;
            if (a.Length != b.Length) return false;
            var diff = 0;
            for (int i = 0; i < a.Length; i++) diff |= a[i] ^ b[i];
            return diff == 0;
        }
    }
}