using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PayOS;
using Swashbuckle.AspNetCore.Annotations;
using System.Text.Json;
using YC3_DAT_VE_CONCERT.Data;
using YC3_DAT_VE_CONCERT.Interface;
using YC3_DAT_VE_CONCERT.Model;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

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
        public async Task<IActionResult> Webhook()
        {
            string payload;
            using (var reader = new StreamReader(Request.Body))
            {
                payload = await reader.ReadToEndAsync();
            }

            var signature = Request.Headers["X-Signature"].FirstOrDefault()
                         ?? Request.Headers["x-signature"].FirstOrDefault()
                         ?? Request.Headers["Signature"].FirstOrDefault()
                         ?? Request.Headers["signature"].FirstOrDefault()
                         ?? string.Empty;

            if (string.IsNullOrEmpty(signature))
                return Unauthorized();

            var verifiedAndSuccess = _payOSService.VerifyWebhookSignature(payload, signature);
            if (!verifiedAndSuccess)
            {
                // Development-only debug info to compare signatures
                if (_env.IsDevelopment())
                {
                    try
                    {
                        var (expectedBase64, expectedHex) = await _payOSService.ComputeExpectedSignaturesAsync(payload);
                        return Unauthorized(new
                        {
                            message = "Signature validation failed (development debug)",
                            receivedSignature = signature,
                            expectedBase64,
                            expectedHex,
                            payloadLength = payload?.Length ?? 0
                        });
                    }
                    catch
                    {
                        // fallthrough to generic Unauthorized
                    }
                }

                return Unauthorized();
            }

            // existing success handling...
            try
            {
                var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
                var webhook = JsonSerializer.Deserialize<PaymentWebhookData>(payload, options);
                if (webhook?.Data != null)
                {
                    var orderCode = webhook.Data.OrderCode;
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
            }

            return Ok();
        }
    }
}
