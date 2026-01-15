using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Hosting;
using PayOS;
using PayOS.Models.Webhooks;
using Swashbuckle.AspNetCore.Annotations;
using System.Text;
using System.Text.Json;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PayOsController : ControllerBase
    {
        private readonly IPayOSService _payOSService;
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _env;

        public PayOsController(IPayOSService payOSService, ApplicationDbContext context, IWebHostEnvironment env)
        {
            _payOSService = payOSService;
            _context = context;
            _env = env;
        }

        [HttpPost("create-payment-link")]
        [SwaggerOperation(Summary = "Create a payment link", Description = "Creates a payment link using PayOS service")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        public async Task<IActionResult> CreatePaymentLink([FromBody] PaymentLinkRequestModel request)
        {
            try
            {
                var amount_int = Convert.ToInt32(request.Amount); // Convert to smallest currency unit
                var result = await _payOSService.CreatePaymentLink(request.OrderCode, amount_int, request.Description, request.BuyerName, request.BuyerEmail);
                return Ok(new
                {
                    success = true,
                    Message = "Payment link created successfully",
                    Data = result
                });
            }
            catch (Exception ex)
            {
                return StatusCode(StatusCodes.Status500InternalServerError, new { Message = "Error creating payment link", Details = ex.Message });
            }
        }

        [HttpPost("webhook")]
        [SwaggerOperation(Summary = "Handle PayOS webhook", Description = "Handles incoming webhooks from PayOS and verifies signature")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status500InternalServerError)]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Webhook()
        {
            try
            {
                // Allow multiple reads of the Request.Body
                Request.EnableBuffering();

                string payload;
                using (var reader = new StreamReader(Request.Body, Encoding.UTF8, leaveOpen: true))
                {
                    payload = await reader.ReadToEndAsync();
                    // rewind for downstream consumers (if any)
                    Request.Body.Position = 0;
                }

                // If body empty, try form content (some providers post form-encoded)
                if (string.IsNullOrWhiteSpace(payload) && Request.HasFormContentType)
                {
                    var form = await Request.ReadFormAsync();
                    // common keys: payload, data, body; pick first that looks JSON
                    if (form.TryGetValue("payload", out var p)) payload = p;
                    else if (form.TryGetValue("data", out var d)) payload = d;
                    else if (form.TryGetValue("body", out var b)) payload = b;
                    else
                    {
                        // fallback: combine form into a simple string (not JSON)
                        payload = string.Join("&", form.Select(kv => $"{kv.Key}={kv.Value}"));
                    }
                }

                if (string.IsNullOrWhiteSpace(payload))
                {
                    // helpful debug in dev
                    if (_env.IsDevelopment())
                    {
                        var headers = Request.Headers.ToDictionary(h => h.Key, h => h.Value.ToString());
                        return BadRequest(new { message = "Empty webhook payload", headers });
                    }
                    return BadRequest("Empty webhook payload");
                }

                // Extract signature from headers (try several names)
                var signature = Request.Headers["X-Signature"].FirstOrDefault()
                             ?? Request.Headers["x-signature"].FirstOrDefault()
                             ?? Request.Headers["Signature"].FirstOrDefault()
                             ?? Request.Headers["signature"].FirstOrDefault()
                             ?? Request.Headers["X-Webhook-Signature"].FirstOrDefault()
                             ?? string.Empty;

                if (string.IsNullOrEmpty(signature))
                {
                    if (_env.IsDevelopment())
                        return Unauthorized(new { message = "Missing signature header" });
                    return Unauthorized();
                }

                // Verify signature + payload indicates success (service handles format)
                var verified = _payOSService.VerifyWebhookSignature(payload, signature);
                if (!verified)
                {
                    if (_env.IsDevelopment())
                    {
                        var (expectedBase64, expectedHex) = await _payOSService.ComputeExpectedSignaturesAsync(payload);
                        return Unauthorized(new
                        {
                            message = "Signature mismatch",
                            received = signature,
                            expectedBase64,
                            expectedHex,
                            payloadPreview = payload.Length > 200 ? payload.Substring(0, 200) + "..." : payload
                        });
                    }
                    return Unauthorized();
                }

                // Deserialize safely
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                Webhook webhookData = null!;
                try
                {
                    webhookData = JsonSerializer.Deserialize<Webhook>(payload, options)!;
                }
                catch
                {
                    // If SDK's Webhook type doesn't match payload, you can attempt to deserialize to your PaymentWebhookData model
                    // webhookData = JsonSerializer.Deserialize<PaymentWebhookData>(payload, options);
                }

                if (webhookData == null)
                {
                    // if unable to map to SDK type, return 400 in dev with payload
                    if (_env.IsDevelopment())
                        return BadRequest(new { message = "Unable to parse webhook JSON", payload });
                    return BadRequest();
                }

                // Process webhook: update order if present
                try
                {
                    // Example: depends on PayOS webhook model; adapt to actual properties
                    var orderCode = webhookData.Data?.OrderCode ?? 0;
                    if (orderCode > 0)
                    {
                        var order = await _context.Orders.FirstOrDefaultAsync(o => o.Id == orderCode);
                        if (order != null)
                        {
                            order.Status = OrderStatus.Completed;
                            await _context.SaveChangesAsync();
                        }
                    }
                }
                catch
                {
                    // swallow or log; still return 200 to acknowledge webhook if signature valid
                }

                return Ok(new { 
                    success = true,
                    message = "Webhook processed successfully"
                });
            }
            catch (Exception ex)
            {
                // return 500 with message (development only)
                if (_env.IsDevelopment())
                    return StatusCode(StatusCodes.Status500InternalServerError, new { message = ex.Message, stack = ex.ToString() });
                return StatusCode(StatusCodes.Status500InternalServerError, new { message = "Error handling webhook" });
            }
        }
    }
}
