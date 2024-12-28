using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;

namespace ECU.Models
{
    public class SensorData
    {
        [BsonId]
        [BsonRepresentation(BsonType.ObjectId)]
        public string? Id { get; set; } // MongoDB ObjectId
        public DateTime Timestamp { get; set; }
        public string SensorType { get; set; }
        public string Instance { get; set; }
        public double Value { get; set; }

    }
}
