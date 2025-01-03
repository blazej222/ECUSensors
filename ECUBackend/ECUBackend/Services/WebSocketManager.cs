using System.Net.WebSockets;
using System.Text;
using System.Text.Json;

public class WebSocketManager
{
    private readonly List<WebSocket> _connectedClients = new();

    public async Task AddClient(WebSocket client)
    {
        _connectedClients.Add(client);

        while (client.State == WebSocketState.Open)
        {
            await Task.Delay(1000); // Keep connection alive
        }

        _connectedClients.Remove(client);
    }

    public async Task NotifyFrontend(object data)
    {
        var json = JsonSerializer.Serialize(data);

        foreach (var client in _connectedClients)
        {
            if (client.State == WebSocketState.Open)
            {
                await client.SendAsync(Encoding.UTF8.GetBytes(json), WebSocketMessageType.Text, true, CancellationToken.None);
            }
        }
    }
}