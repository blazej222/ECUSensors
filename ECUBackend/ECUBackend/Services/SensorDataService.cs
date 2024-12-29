using ECUBackend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

namespace ECUBackend.Services
{
    public class SensorDataService
    {
        private readonly IMongoCollection<SensorData> _sensorDataCollection;

        public SensorDataService(IOptions<SensorDatabaseSettings> sensorDatabaseSettings)
        {
            var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "mongodb://host.docker.internal:27017"; //windows dns
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

        public async Task<List<SensorData>> GetAll() =>
            await _sensorDataCollection.Find(_ => true).ToListAsync();

        public async Task<bool> CheckMongoConnection()
        {
            try
            {
                var connectionString = Environment.GetEnvironmentVariable("MONGO_CONNECTION_STRING") ?? "mongodb://host.docker.internal:27017"; //windows dns
                var client = new MongoClient(connectionString);
                await client.ListDatabasesAsync();
                return true;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"MongoDB connection failed: {ex.Message}");
                return false;
            }
        }
    }

}

