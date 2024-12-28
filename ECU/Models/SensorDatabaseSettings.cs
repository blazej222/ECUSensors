namespace ECU.Models
{
    public class SensorDatabaseSettings
    {
        public string ConnectionString { get; set; } = null!;

        public string DatabaseName { get; set; } = null!;

        public string SensorCollectionName { get; set; } = null!;
    }
}
