using ECUBackend.Models;
using Microsoft.Extensions.Options;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Runtime.CompilerServices;

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

        public async Task<List<SensorData>> GetAll() =>
            await _sensorDataCollection.Find(_ => true).ToListAsync();

        public async Task<List<SensorData>> GetSensorData(
            string sensorType, uint? instanceId = null, DateTime? startDate = null, DateTime? endDate = null,
            string sortBy = null, bool? ascending = null, int? limit = 0)
        {
            //await _sensorDataCollection.Find(x =>
            //    (string.IsNullOrEmpty(sensorType) || x.SensorType == sensorType) &&
            //    (!instanceId.HasValue || x.InstanceId == instanceId) &&
            //    (!startDate.HasValue || x.Timestamp >= startDate) &&
            //    (!endDate.HasValue || x.Timestamp <= endDate))
            //    .ToListAsync();

            var filter = Builders<SensorData>.Filter.And(
                (string.IsNullOrEmpty(sensorType) ? Builders<SensorData>.Filter.Empty : Builders<SensorData>.Filter.Eq(x => x.SensorType, sensorType)),
                (!instanceId.HasValue ? Builders<SensorData>.Filter.Empty : Builders<SensorData>.Filter.Eq(x => x.InstanceId, instanceId)),
                (!startDate.HasValue ? Builders<SensorData>.Filter.Empty : Builders<SensorData>.Filter.Gte(x => x.Timestamp, startDate)),
                (!endDate.HasValue ? Builders<SensorData>.Filter.Empty : Builders<SensorData>.Filter.Lte(x => x.Timestamp, endDate))
            );

            if (string.IsNullOrEmpty(sortBy) && ascending.HasValue)
            {
                return await _sensorDataCollection
                .Find(filter)
                .Limit(limit)
                .ToListAsync();
            }
            if (string.IsNullOrEmpty(sortBy)) sortBy = "Timestamp";

            // ASP.NET serializes property names to camelCase by default in JSON
            if (sortBy.Length > 0 && char.IsLower(sortBy[0])) sortBy = char.ToUpper(sortBy[0]) + sortBy.Substring(1);
            var sort = ascending.HasValue && ascending.Value
            ? Builders<SensorData>.Sort.Ascending(sortBy)
            : Builders<SensorData>.Sort.Descending(sortBy);

            return await _sensorDataCollection
                .Find(filter)
                .Sort(sort)
                .Limit(limit)
                .ToListAsync();
        }

        public async Task<List<SensorSummary>> GetSensorSummary(int recordCount = 100)
        {
            var pipeline = new[]
            {
                new BsonDocument("$sort", new BsonDocument("Timestamp", -1)),

                new BsonDocument("$group", new BsonDocument
                {
                    { "_id", new BsonDocument
                        {
                            { "SensorType", "$SensorType" },
                            { "InstanceId", "$InstanceId" }
                        }
                    },
                    { "LastValues", new BsonDocument("$push", "$Value") },
                    { "LastValue", new BsonDocument("$first", "$Value") }
                }),

                new BsonDocument("$project", new BsonDocument
                {
                    { "_id", 0 },
                    { "SensorType", "$_id.SensorType" },
                    { "InstanceId", "$_id.InstanceId" },
                    { "AverageValue", new BsonDocument("$avg", new BsonDocument("$slice", new BsonArray { "$LastValues", recordCount })) },
                    { "LastValue", "$LastValue" }
                })
            };

            var result = await _sensorDataCollection.Aggregate<SensorSummary>(pipeline).ToListAsync();
            return result;
        }






        public async Task<bool> DeleteSensorDataAsync(FilterDefinition<SensorData> filter)
        {
            try
            {
                var deleteResult = await _sensorDataCollection.DeleteManyAsync(filter);
                return deleteResult.DeletedCount > 0;
            }
            catch (Exception ex)
            {
                // Log exception as needed
                Console.WriteLine($"Error deleting sensor data: {ex.Message}");
                return false;
            }
        }

        public async Task<List<string>> GetDistinctSensorTypes()
        {
            var distinctSensorTypes = await _sensorDataCollection
                .Aggregate()
                .Group(x => x.SensorType, g => new { SensorType = g.Key })
                .Project(result => result.SensorType)
                .ToListAsync();

            return distinctSensorTypes;
        }

        public async Task<List<string>> GetDistinctSensors()
        {
            var distinctSensorTypes = await _sensorDataCollection
                .Aggregate()
                .Group(x => new { x.SensorType, x.InstanceId }, g => new { g.Key.SensorType, g.Key.InstanceId })
                .Project(result => result.SensorType + result.InstanceId)
                .ToListAsync();

            distinctSensorTypes.Sort();
            return distinctSensorTypes;
        }

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

