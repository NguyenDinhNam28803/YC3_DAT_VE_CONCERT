using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Service;
using YC3_DAT_VE_CONCERT.Interface;
using Swashbuckle.AspNetCore.Annotations;

namespace YC3_DAT_VE_CONCERT.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class StatitiscalController : ControllerBase
    {
        private readonly IStatisticalService _statisticalService;
        public StatitiscalController(IStatisticalService statisticalService)
        {
            _statisticalService = statisticalService;
        }

        [HttpGet]
        [SwaggerOperation(Summary = "Get statistical data")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetStatisticalData()
        {
            try
            {
                var statisticalData = await _statisticalService.GetStatisticalData();
                return Ok(new
                {
                    success = true,
                    message = "Statistical data retrieved successfully",
                    data = statisticalData
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to retrieve statistical data",
                    detail = ex.Message
                });
            }
        }

        // Chưa hoàn thiaanj
        [HttpGet]
        [Route("events")]
        [SwaggerOperation(Summary = "Get event list with statistics")]
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(object))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(object))]
        public async Task<IActionResult> GetEventList()
        {
            try
            {
                var eventList = await _statisticalService.GetEventList();
                return Ok(new
                {
                    success = true,
                    message = "Event list retrieved successfully",
                    data = eventList
                });
            }
            catch (Exception ex)
            {
                return BadRequest(new
                {
                    success = false,
                    message = "Failed to retrieve event list",
                    detail = ex.Message
                });
            }
        }
    }
}
