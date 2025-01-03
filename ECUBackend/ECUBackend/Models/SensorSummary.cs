using System.Numerics;
using System.Text.Json.Serialization;

namespace ECUBackend.Models
{
    public class SensorSummary
    {
        [JsonPropertyName("sensorType")]
        public string SensorType { get; set; }

        [JsonPropertyName("instanceId")]
        public uint InstanceId { get; set; }

        [JsonPropertyName("averageValue")]
        public double AverageValue { get; set; }

        [JsonPropertyName("lastValue")]
        public double LastValue { get; set; }

        [JsonPropertyName("balance")]
        public int Balance { get; set; }
    }

}
