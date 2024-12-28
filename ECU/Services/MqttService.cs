using MQTTnet;
using MQTTnet.Client;
using System.Text;
using System.Text.Json;
using ECU.Models;

namespace ECU.Services
{

    public class MqttService
    {
        private readonly IMqttClient _mqttClient;
        private readonly SensorDataService _sensorDataService;

        public MqttService(SensorDataService sensorDataService)
        {
            _mqttClient = new MqttFactory().CreateMqttClient();
            _sensorDataService = sensorDataService;
        }

        public async Task StartAsync(string brokerAddress, int port)
        {
            var options = new MqttClientOptionsBuilder()
                .WithTcpServer(brokerAddress, port)
                .Build();
            /*
                        _mqttClient.UseApplicationMessageReceivedHandler(async e =>
                        {
                            var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                            var sensorData = JsonSerializer.Deserialize<SensorData>(payload);
                            if (sensorData != null)
                            {
                                await _sensorDataService.InsertSensorData(sensorData);
                            }
                        });*/

            _mqttClient.ApplicationMessageReceivedAsync += async e =>
            {
                var payload = Encoding.UTF8.GetString(e.ApplicationMessage.Payload);
                var sensorData = JsonSerializer.Deserialize<SensorData>(payload);
                if (sensorData != null)
                {
                    await _sensorDataService.InsertSensorData(sensorData);
                }
            };


            await _mqttClient.ConnectAsync(options);
            await _mqttClient.SubscribeAsync(new MqttTopicFilterBuilder().WithTopic("sensors/#").Build());
        }
    }
}