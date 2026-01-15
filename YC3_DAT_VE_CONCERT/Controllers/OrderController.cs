using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using YC3_DAT_VE_CONCERT.Dto;
using YC3_DAT_VE_CONCERT.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class OrderController : ControllerBase
    {
        private readonly IOrderService _orderService;
        private readonly IPayOSService _payOSService;
        public OrderController(IOrderService orderService, IPayOSService payOSService)
        {
            _orderService = orderService;
            _payOSService = payOSService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get all orders")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetAllOrders()
        {
            try
            {
                var orders = await _orderService.GetAllOrders();
                return Ok(new
                {
                    success = true,
                    message = "Orders retrieved successfully",
                    data = orders
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpGet]
        [Route("user/{userId}")]
        [SwaggerOperation(Summary = "Get orders by user ID")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetOrdersByUserId(int userId)
        {
            try
            {
                var existingOrder = await _orderService.GetOrdersByUserId(userId);
                if (existingOrder == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"User has no order!"
                    });
                }
                var orders = await _orderService.GetOrdersByUserId(userId);
                return Ok(new
                {
                    success = true,
                    message = "Orders retrieved successfully",
                    data = orders
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPost]
        [SwaggerOperation(Summary = "Create a new order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto orderDto)
        {
            try
            {
                var order =  await _orderService.CreateOrder(orderDto);
                return Ok(new
                {
                    success = true,
                    message = "Order created successfully",
                    order = order
                });
            }
            catch (Exception ex)
            {
                var fullError = ex.ToString(); // Lấy full error
                Console.WriteLine(fullError);

                if (ex.InnerException != null)
                {
                    Console.WriteLine("Inner Exception: " + ex.InnerException.ToString());
                }

                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }

        [HttpPut]
        [Route("{orderId}")]
        [SwaggerOperation(Summary = "Update an order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> UpdateOrder(int orderId, [FromBody] UpdateOrderStatusDto updateOrderStatus)
        {
            try
            {
                var existingOrder = await _orderService.GetOrderById(orderId);
                if (existingOrder == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Order with  ID {orderId} not found."
                    });
                }

                var response = await _orderService.UpdateOrder(orderId, updateOrderStatus);
                return Ok(new
                {
                    success = true,
                    message = "Order updated successfully",
                    data = response
                });
            }
            catch (Exception ex) {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });

            }
        }

        [HttpDelete]
        [Route("{orderId}")]
        [SwaggerOperation(Summary = "Cancel an order")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> CancelOrder(int orderId)
        {
            try
            {
                var existingOrder = await _orderService.GetOrderById(orderId);
                if (existingOrder == null)
                {
                    return NotFound(new
                    {
                        success = false,
                        message = $"Order with  ID {orderId} not found."
                    });
                }
                var result = await _orderService.CancelOrder(orderId);

                if (result)
                {
                    return Ok(new
                    {
                        success = true,
                        message = "Order cancelled successfully"
                    });
                }
                else
                {
                    return BadRequest(new
                    {
                        success = false,
                        message = "Failed to cancel order"
                    });
                }
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = ex.Message
                });
            }
        }
    }
}
