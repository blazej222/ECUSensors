using ECUBackend.Models;
using ECUBackend.Services;
using Microsoft.AspNetCore.Mvc;
using MongoDB.Driver;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace ECUBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class DebugController : ControllerBase
    {
        private readonly SensorDataService _sensorDataService;
        public DebugController(SensorDataService sensorDataservice) =>
            _sensorDataService = sensorDataservice;


        [HttpDelete("clear-database")]
        public async Task<ActionResult> DeleteAll()
        {
            var response = await _sensorDataService.DeleteSensorDataAsync(FilterDefinition<SensorData>.Empty);
            if (response) return Ok("Database was cleared.");
            else return StatusCode(500, "Error");
        }

        [HttpGet("check-mongo-connection")]
        public async Task<ActionResult> CheckMongoConnection()
        {
            bool isConnected = await _sensorDataService.CheckMongoConnection();
            if (isConnected)
            {
                return Ok("MongoDB connection successful");
            }
            else
            {
                return StatusCode(500, "MongoDB connection failed");
            }
        }
    }
}
