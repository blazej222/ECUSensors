using ECU.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ECU.Services
{
    public class SensorDataService
    {
        private readonly IMongoCollection<SensorData> _sensorDataCollection;

        public SensorDataService(IOptions<SensorDatabaseSettings> sensorDatabaseSettings)
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "mongodb://localhost:27017";
            var client = new MongoClient(connectionString);
            var database = client.GetDatabase(sensorDatabaseSettings.Value.DatabaseName);
            _sensorDataCollection = database.GetCollection<SensorData>(sensorDatabaseSettings.Value.SensorCollectionName);
        }

        public async Task InsertSensorData(SensorData data) =>
            await _sensorDataCollection.InsertOneAsync(data);

        public async Task<List<SensorData>> GetAsync() =>
            await _sensorDataCollection.Find(_ => true).ToListAsync();

        public async Task<List<SensorData>> GetSensorData(
            string sensorType, DateTime? startDate, DateTime? endDate) =>
            await _sensorDataCollection.Find(x =>
                (string.IsNullOrEmpty(sensorType) || x.SensorType == sensorType) &&
                (!startDate.HasValue || x.Timestamp >= startDate) &&
                (!endDate.HasValue || x.Timestamp <= endDate))
                .ToListAsync();
    }
}
