using Microsoft.AspNetCore.Mvc;
using ECUBackend.Services;
using ECUBackend.Models;
using System.Text;

namespace ECU.Controllers
{
    [ApiController]
    //[Route("api/[controller]")]
    [Route("api/sensors")]
    public class SensorsController : ControllerBase
    {
        private readonly SensorDataService _sensorDataService;
        private readonly BlockchainService _blockchainService;

        public SensorsController(SensorDataService sensorDataService, BlockchainService blockchainService)
        {
            _sensorDataService = sensorDataService;
            _blockchainService = blockchainService;
        }

        [HttpGet]
        public async Task<List<string>> GetSensors() =>
            await _sensorDataService.GetDistinctSensors();

        [HttpGet("data")]
        public async Task<List<SensorData>> GetAllData() =>
            await _sensorDataService.GetAll();

        [HttpGet("types")]
        public async Task<List<string>> GetTypes() =>
            await _sensorDataService.GetDistinctSensorTypes();

        [HttpGet("wallet")]
        public async Task<int> GetSaldo(
            [FromQuery] string sensorType,
            [FromQuery] uint instanceId) =>
            await _blockchainService.GetBalance($"{sensorType}{instanceId}");


        [HttpGet("data/filter")]
        public async Task<IActionResult> ExportData(
            [FromQuery] string? sensorType,
            [FromQuery] uint? instanceId,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] string? sortBy,
            [FromQuery] bool? ascending,
            [FromQuery] int? limit,
            [FromQuery] string? format = null)
        {
            var data = await _sensorDataService.GetSensorData(sensorType, instanceId, startDate, endDate, sortBy, ascending, limit);

            if (!string.IsNullOrEmpty(format))
            {
                if (format.ToLower() == "csv")
                {
                    var csvContent = GenerateCsv(data);
                    var fileName = $"sensor_data_{DateTime.UtcNow:yyyyMMdd_HHmmss}.csv";

                    return File(
                        System.Text.Encoding.UTF8.GetBytes(csvContent),
                        "text/csv",
                        fileName);
                }
                else if (format.ToLower() == "json")
                {
                    var jsonContent = System.Text.Json.JsonSerializer.Serialize(data, new System.Text.Json.JsonSerializerOptions
                    {
                        WriteIndented = true
                    });
                    var fileName = $"sensor_data_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";

                    return File(
                        System.Text.Encoding.UTF8.GetBytes(jsonContent),
                        "application/json",
                        fileName);
                }
                else
                {
                    return BadRequest("Unsupported format. Use 'json' or 'csv'.");
                }
            }

            return Ok(data);
        }


        [HttpGet("summary")]
        public async Task<ActionResult<List<SensorSummary>>> GetSensorSummaries([FromQuery] int recordCount = 100)
        {
            var summaries = await _sensorDataService.GetSensorSummary(recordCount);
            return Ok(summaries);
        }


        private string GenerateCsv(IEnumerable<SensorData> data)
        {
            var csv = new StringBuilder();

            csv.AppendLine("Id,SensorType,InstanceId,Value,Timestamp");

            foreach (var item in data)
            {
                csv.AppendLine($"{item.Id},{item.SensorType},{item.InstanceId},{item.Value},{item.Timestamp:O}");
            }

            return csv.ToString();
        }

        //[HttpPost]
        //public async Task<ActionResult> Post([FromBody] SensorData data)
        //{
        //    await _sensorDataService.InsertSensorData(data);
        //    return CreatedAtAction(nameof(Get), new { id = data.Id }, data);
        //}
    }
}
