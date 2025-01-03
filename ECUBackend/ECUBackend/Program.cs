using ECUBackend.Models;
using ECUBackend.Services;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<BlockchainService>();
// Add services to the container.
builder.Services.Configure<SensorDatabaseSettings>(
    builder.Configuration.GetSection("SensorDatabase"));

builder.Services.AddSingleton<SensorDataService>();
builder.Services.AddSingleton<WebSocketManager>();
builder.Services.AddHostedService<MqttService>();

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000")
                  .AllowAnyHeader()
                  .AllowAnyMethod()
                  .AllowCredentials();
        });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Middleware dla obsługi zapytań OPTIONS
app.Use(async (context, next) =>
{
    if (context.Request.Method == "OPTIONS")
    {
        context.Response.Headers.Add("Access-Control-Allow-Origin", "http://localhost:3000");
        context.Response.Headers.Add("Access-Control-Allow-Methods", "GET, POST, OPTIONS");
        context.Response.Headers.Add("Access-Control-Allow-Headers", "*");
        context.Response.StatusCode = 200;
        await context.Response.CompleteAsync();
        return;
    }
    await next();
});


app.UseWebSockets();


// Configure the HTTP request pipeline.
if (true)//app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();
app.UseSwagger();
app.UseSwaggerUI();

// WebSocket endpoint
app.Use(async (context, next) =>
{
    if (context.Request.Path == "/ws" && context.WebSockets.IsWebSocketRequest)
    {
        var webSocket = await context.WebSockets.AcceptWebSocketAsync();
        var webSocketManager = context.RequestServices.GetRequiredService<WebSocketManager>();
        await webSocketManager.AddClient(webSocket);
    }
    else
    {
        await next();
    }
});

app.Run();
