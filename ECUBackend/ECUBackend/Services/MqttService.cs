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

    private readonly string _adminPrivateKey = "0x8afc011d81328f202730e85dce98c1c0596a977c99f38b05800ce96762fda0f5";
    private readonly string _contractAddress = "0xd8790cbc63d62a82e5a81f793af99f0793d127a3";


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

        CreateWalletDictionary();
        InitBlockchainService();
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
            _blockchainService.RewardSensor(_adminPrivateKey, _walletDict[$"{sensorData.SensorType}{sensorData.InstanceId}"].Address, rewardAmount);
    }

    private void InitBlockchainService()
    {
        var rpcUrl = Environment.GetEnvironmentVariable("RPC_URL") ?? "http://host.docker.internal:7545"; //"http://host.docker.internal:7545";
        var abi = File.ReadAllText("Resources/SensorToken.abi");

        // Inicjalizacja BlockchainService
        _blockchainService = new BlockchainService(rpcUrl, _contractAddress, abi);
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
