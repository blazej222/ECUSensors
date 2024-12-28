using ECU.Models;
using ECU.Services;
using Microsoft.AspNetCore.Mvc;

namespace ECU.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SensorsController : ControllerBase
    {
        private readonly SensorDataService _sensorDataService;

        public SensorsController(SensorDataService sensorDataService) =>
            _sensorDataService = sensorDataService;

        [HttpGet]
        public async Task<List<SensorData>> Get() =>
            await _sensorDataService.GetAsync();

        [HttpGet("filter")]
        public async Task<ActionResult<List<SensorData>>> GetFiltered(
            [FromQuery] string sensorType,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            var data = await _sensorDataService.GetSensorData(sensorType, startDate, endDate);
            return Ok(data);
        }

        [HttpPost]
        public async Task<ActionResult> Post([FromBody] SensorData data)
        {
            await _sensorDataService.InsertSensorData(data);
            return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        }

    }
}
