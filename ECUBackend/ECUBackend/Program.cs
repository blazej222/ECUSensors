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


var rpcUrl = "http://127.0.0.1:7545";
var contractAddress = "0xd5f79e439cb4cfdb1fe1d55edbeaec9a428d2350"; // Adres kontraktu wdrożonego w Holesky
var adminPrivateKey = "0x99f49164afaec80f3baaeace6f08c49a7bcdd9cb452248436a814cfde33a1096";

// Wczytaj ABI
var abi = File.ReadAllText("Resources/SensorToken.abi");

// Inicjalizacja BlockchainService
var blockchainService = new BlockchainService(rpcUrl, contractAddress, abi);

// Adres portfela sensora i kwota nagrody
//var sensorAddress = "0x0CDf25e1c917cFC69F390bb70940ABEDBd596A4C";

String[] sensorAddresses = { "0x94D6548C8306DA8146E5A240B8d396099d36Fc20", "0x2364FA1A46B79AF6B774Fb41B331afD2eEF2bCaf", "0x5AB38A7f20996e59A0230F47Db6862D2710CfBe0" };

var rewardAmount = new BigInteger(100000000000);

foreach (String x in sensorAddresses)
{
    await blockchainService.RewardSensor(adminPrivateKey, x, rewardAmount);
}

//await blockchainService.RewardSensor(adminPrivateKey, sensorAddress, rewardAmount);

//Koniec testu blockchaina

foreach (String x in sensorAddresses)
{
    await blockchainService.GetBalance(x);
}


app.Run();
