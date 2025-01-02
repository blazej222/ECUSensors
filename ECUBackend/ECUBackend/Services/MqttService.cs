using MQTTnet;
using MQTTnet.Client;
using System.Text;
using ECUBackend.Services;
using ECUBackend.Models;
using MQTTnet.Server;
using MongoDB.Bson;
using System.Text.Json;
using System.Text.Json.Serialization;

public class MqttService : BackgroundService
{
    private IMqttClient _mqttClient;
    private MqttClientOptions _options;
    private const string TOPIC = "sensors";
    private SensorDataService _sensorService;

    public MqttService(SensorDataService service)
    {
        _sensorService = service;
        _mqttClient = new MqttFactory().CreateMqttClient();

        string broker = Environment.GetEnvironmentVariable("MQQT_BROKER_ADDRESS") ?? "host.docker.internal"; //windows dns
        int port = int.Parse(Environment.GetEnvironmentVariable("MQQT_BROKER_PORT") ?? "1883");

        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, port) // MQTT broker address and port
            //.WithCredentials(username, password) // Set username and password
            //.WithClientId(clientId)
            .WithCleanSession()
            .Build();
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        try
        {
            var connectResult = await _mqttClient.ConnectAsync(_options, stoppingToken);
            if (connectResult.ResultCode == MqttClientConnectResultCode.Success)
            {
                _mqttClient.ApplicationMessageReceivedAsync += e =>
                {
                    string message = Encoding.UTF8.GetString(e.ApplicationMessage.PayloadSegment);
                    Console.WriteLine($"Received message: {message}");
                    SaveMessageToDatabase(message);
                    return Task.CompletedTask;
                };

                await _mqttClient.SubscribeAsync(TOPIC);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"MQTT service error: {ex.Message}");
        }
    }

    private async void SaveMessageToDatabase(string message)
    {
        var options = new JsonSerializerOptions
        {
            Converters = { new UnixTimestampConverter() }
        };

        SensorData sensorData = JsonSerializer.Deserialize<SensorData>(message, options);
        await _sensorService.InsertSensorData(sensorData);
    }

    public class UnixTimestampConverter : JsonConverter<DateTime>
    {
        public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            double unixTimestamp = reader.GetDouble();
            return DateTimeOffset.FromUnixTimeSeconds((long)unixTimestamp).UtcDateTime
                   + TimeSpan.FromSeconds(unixTimestamp % 1);
        }

        public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options)
        {
            double unixTimestamp = new DateTimeOffset(value).ToUnixTimeSeconds()
                                   + value.Millisecond / 1000.0;
            writer.WriteNumberValue(unixTimestamp);
        }
    }
}
