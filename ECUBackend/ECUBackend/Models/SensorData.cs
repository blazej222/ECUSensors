using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using System.Text.Json.Serialization;

namespace ECUBackend.Models
{
    public class SensorData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // MongoDB ObjectId
        [JsonPropertyName("sensorType")]
        public string SensorType { get; set; }
        [JsonPropertyName("instanceId")]
        public uint InstanceId { get; set; }
        [JsonPropertyName("value")]
        public double Value { get; set; }
        [JsonPropertyName("timestamp")]
        public DateTime Timestamp { get; set; }

    }
}
