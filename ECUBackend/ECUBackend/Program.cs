using ECUBackend.Models;
using ECUBackend.Services;
using System.Resources;
using System.Numerics;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
builder.Services.Configure<SensorDatabaseSettings>(
    builder.Configuration.GetSection("SensorDatabase"));

builder.Services.AddSingleton<SensorDataService>();
builder.Services.AddHostedService<MqttService>();

// Add services to the container.

builder.Services.AddControllers();
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddCors(options =>
{
    options.AddPolicy("AllowFrontend",
        policy =>
        {
            policy.WithOrigins("http://localhost:3000") // Adres frontendu
                  .AllowAnyHeader()
                  .AllowAnyMethod();
        });
});

var app = builder.Build();

app.UseCors("AllowFrontend");

// Configure the HTTP request pipeline.
if (true)//app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

//app.UseAuthorization();

app.MapControllers();

//Test blockchaina


var rpcUrl = "https://1rpc.io/holesky";
var contractAddress = "0xe10C866b9BCD771E16e38b7E594AC0df126d2C67"; // Adres kontraktu wdrożonego w Holesky
var adminPrivateKey = "40a19da9ea93ca341ad4118ff39cd259f2a42471fcb89e2c9539674ad21573a0";

// Wczytaj ABI
var abi = File.ReadAllText("Resources/SensorToken.abi");

// Inicjalizacja BlockchainService
var blockchainService = new BlockchainService(rpcUrl, contractAddress, abi);

// Adres portfela sensora i kwota nagrody
var sensorAddress = "0x0CDf25e1c917cFC69F390bb70940ABEDBd596A4C";
var rewardAmount = new BigInteger(1000000000000000000);

await blockchainService.RewardSensor(adminPrivateKey, sensorAddress, rewardAmount);

//Koniec testu blockchaina


app.Run();
