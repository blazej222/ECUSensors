using Microsoft.AspNetCore.Mvc;
using ECUBackend.Services;
using ECUBackend.Models;

namespace ECU.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/sensors")]
    public class SensorsController : ControllerBase
    {
        private readonly SensorDataService _sensorDataService;

        public SensorsController(SensorDataService sensorDataService) =>
            _sensorDataService = sensorDataService;

        [HttpGet]
        public async Task<List<string>> GetSensors() =>
            await _sensorDataService.GetDistinctSensors();

        [HttpGet("data")]
        public async Task<List<SensorData>> GetAllData() =>
            await _sensorDataService.GetAll();

        [HttpGet("types")]
        public async Task<List<string>> GetTypes() =>
            await _sensorDataService.GetDistinctSensorTypes();


        [HttpGet("data/filter")]
        public async Task<ActionResult<List<SensorData>>> GetFiltered(
            [FromQuery] string sensorType,
            [FromQuery] uint? instanceId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? sortBy,
            [FromQuery] bool? ascending,
            [FromQuery] int? limit
            )
        {
            var data = await _sensorDataService.GetSensorData(sensorType, instanceId, startDate, endDate, sortBy, ascending, limit);
            return Ok(data);
        }

        //[HttpPost]
        //public async Task<ActionResult> Post([FromBody] SensorData data)
        //{
        //    await _sensorDataService.InsertSensorData(data);
        //    return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        //}
    }
}
