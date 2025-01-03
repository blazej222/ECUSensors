using MQTTnet;
using MQTTnet.Client;
using System.Text;
using ECUBackend.Services;
using ECUBackend.Models;
using MQTTnet.Server;
using System.Text.Json;
using System.Text.Json.Serialization;

public class MqttService : BackgroundService
{
    private IMqttClient _mqttClient;
    private MqttClientOptions _options;
    private const string TOPIC = "sensors";
    private SensorDataService _sensorService;
    private BlockchainService _blockchainService;
    private Dictionary<string, SensorCryptoWallet> _walletDict = new();
    private readonly WebSocketManager _webSocketManager;

    public MqttService(SensorDataService service, BlockchainService blockchainService, WebSocketManager webSocketManager)
    {
        _sensorService = service;
        _blockchainService = blockchainService;
        _webSocketManager = webSocketManager;
        _mqttClient = new MqttFactory().CreateMqttClient();

        string broker = Environment.GetEnvironmentVariable("MQQT_BROKER_ADDRESS") ?? "host.docker.internal"; // windows dns
        int port = int.Parse(Environment.GetEnvironmentVariable("MQQT_BROKER_PORT") ?? "1883");

        _options = new MqttClientOptionsBuilder()
            .WithTcpServer(broker, port) // MQTT broker address and port
                                         //.WithCredentials(username, password) // Set username and password
                                         //.WithClientId(clientId)
            .WithCleanSession()
            .Build();

        CreateWalletDictionary();
    }

    private void CreateWalletDictionary()
    {
        string json = File.ReadAllText("Resources/sensors.json");


        var wallets = JsonSerializer.Deserialize<List<SensorCryptoWallet>>(json);

        foreach (var wallet in wallets)
        {
            if (!string.IsNullOrEmpty(wallet.SensorId))
            {
                _walletDict[wallet.SensorId] = wallet;
            }
        }
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
        //var rewardAmount = new BigInteger(100000000000);
        var rewardAmount = 10000;

        await _sensorService.InsertSensorData(sensorData);
        //await
        _blockchainService.RewardSensor(_walletDict[$"{sensorData.SensorType}{sensorData.InstanceId}"].Address, rewardAmount);
		
		var summary = await _sensorService.GetSingleSensorSummary(sensorData.SensorType, sensorData.InstanceId);
        await _webSocketManager.NotifyFrontend(summary);

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