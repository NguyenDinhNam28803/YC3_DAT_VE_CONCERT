using Microsoft.Extensions.Configuration;
using PayOS;
using PayOS.Models;
using PayOS.Models.V2.PaymentRequests;
using PayOS.Resources.V2.PaymentRequests;
using System;
using System.Security.Cryptography;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
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
                var amount_int = Convert.ToInt32(amount); // Convert to smallest currency unit
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

        //Placeholder - implement according to SDK methods
        public async Task<object> GetPaymentInfo(long orderCode)
        {
            //try
            //{
            //    var response = await Task.Run(() => _payOSClient.PaymentRequests.(orderCode));
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Error retrieving payment info: " + ex.Message);
            //}
            throw new NotImplementedException();
        }

        // Placeholder - implement according to SDK methods
        public async Task<object> CancelPayment(long orderCode, string? reason = null)
        {
            //try
            //{
            //    var response = await Task.Run(() => _payOSClient.CancelPaymentLink(orderCode, reason));
            //    return response;
            //}
            //catch (Exception ex)
            //{
            //    throw new Exception("Error cancelling payment: " + ex.Message);
            //}
            throw new NotImplementedException();
        }

        // Verify webhook signature using stored checksum key (HMAC-SHA256 -> Base64)
        public bool VerifyWebhookSignature(string payload, string signature)
        {
            try
            {
                if (string.IsNullOrEmpty(payload) || string.IsNullOrEmpty(signature) || string.IsNullOrEmpty(_checksumKey))
                    return false;

                // Normalize signature header: remove common prefixes like "sha256=" if present and trim
                signature = signature.Trim();
                if (signature.StartsWith("sha256=", StringComparison.OrdinalIgnoreCase))
                    signature = signature.Substring("sha256=".Length);

                // Compute HMAC-SHA256 over payload
                bool signatureValid = ComputeAndCompareHmac(payload, signature);

                // If initial check fails, try computing HMAC after removing "signature" property from body (some providers include signature in body)
                if (!signatureValid)
                {
                    var cleaned = RemoveSignatureProperty(payload);
                    if (cleaned != null && cleaned != payload)
                        signatureValid = ComputeAndCompareHmac(cleaned, signature);
                }

                if (!signatureValid)
                    return false;

                // If signature valid, parse payload and check success flag
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var webhook = JsonSerializer.Deserialize<PaymentWebhookData>(payload, options)
                              ?? JsonSerializer.Deserialize<PaymentWebhookData>(RemoveSignatureProperty(payload) ?? payload, options);

                if (webhook == null)
                    return false;

                // Determine success:
                // - If payload contains "success" boolean, use it.
                // - Otherwise, check Data or Code/Status fields (fallback).
                if (webhook.Success)
                    return true;

                // Fallback checks if Success boolean absent: check Data/Desc/Code status patterns
                if (webhook.Data != null)
                {
                    // Example: if there's a reference and positive amount, consider success. Adjust logic per PayOS docs.
                    if (webhook.Data.Amount > 0 && !string.IsNullOrEmpty(webhook.Data.Reference))
                        return true;
                }

                // Not successful
                return false;
            }
            catch
            {
                return false;
            }
        }

        private bool ComputeAndCompareHmac(string payload, string signature)
        {
            var keyBytes = Encoding.UTF8.GetBytes(_checksumKey);
            var payloadBytes = Encoding.UTF8.GetBytes(payload);

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(payloadBytes);

            // Try Base64 and hex representations
            var expectedBase64 = Convert.ToBase64String(hash);
            if (SecureEquals(expectedBase64, signature))
                return true;

            var expectedHex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
            if (SecureEquals(expectedHex, signature.ToLowerInvariant()))
                return true;

            return false;
        }

        private string? RemoveSignatureProperty(string payload)
        {
            try
            {
                var node = JsonNode.Parse(payload);
                if (node is JsonObject obj)
                {
                    // remove common property names for signature
                    obj.Remove("signature");
                    obj.Remove("Signature");
                    obj.Remove("sign");
                    obj.Remove("signatures");
                    return obj.ToJsonString();
                }
            }
            catch
            {
                // ignore parsing errors, return null to indicate no cleaning done
            }
            return null;
        }

        // inside PayOsService class
        public async Task<(string expectedBase64, string expectedHex)> ComputeExpectedSignaturesAsync(string payload)
        {
            // synchronous compute — wrapped with Task.FromResult for interface async
            var keyBytes = Encoding.UTF8.GetBytes(_checksumKey);
            var payloadBytes = Encoding.UTF8.GetBytes(payload ?? "");

            using var hmac = new HMACSHA256(keyBytes);
            var hash = hmac.ComputeHash(payloadBytes);

            var expectedBase64 = Convert.ToBase64String(hash);
            var expectedHex = BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();

            return await Task.FromResult((expectedBase64, expectedHex));
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