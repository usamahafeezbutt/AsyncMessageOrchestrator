using Azure.Messaging.ServiceBus;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Azure.Functions.Worker.Builder;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using StackExchange.Redis;

var builder = FunctionsApplication.CreateBuilder(args);

builder.ConfigureFunctionsWebApplication();

builder.Services
    .AddApplicationInsightsTelemetryWorkerService()
    .ConfigureFunctionsApplicationInsights();

builder.Services.AddSingleton<IConnectionMultiplexer>(
           ConnectionMultiplexer.Connect(builder.Configuration["RedisConnection"]!));

builder.Services.AddSingleton(new ServiceBusClient(builder.Configuration["ServiceBusConnection"]));
builder.Services.AddHttpClient();

builder.Build().Run();
