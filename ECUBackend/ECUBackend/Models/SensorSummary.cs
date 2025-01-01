namespace ECUBackend.Models
{
    public class SensorSummary
    {
        public string SensorType { get; set; }
        public uint InstanceId { get; set; }
        public double AverageValue { get; set; }
        public double LastValue { get; set; }
    }

}
