using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using YC3_DAT_VE_CONCERT.Service;
using YC3_DAT_VE_CONCERT.Interface;

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
    }
}
