using HandleMessages.Dtos;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using System.Net.Http.Json;

namespace HandleMessages;

public class ProcessMessage
{
    private readonly ILogger<ProcessMessage> _logger;
    private readonly HttpClient _httpClient;
    private readonly IConnectionMultiplexer _redis;

    public ProcessMessage(
        ILogger<ProcessMessage> logger,
        IHttpClientFactory httpClientFactory,
        IConnectionMultiplexer redis)
    {
        _logger = logger;
        _httpClient = httpClientFactory.CreateClient();
        _redis = redis;
    }

    [Function(nameof(ProcessMessage))]
    public async Task Run(
        [ServiceBusTrigger("%QueueName%", Connection = "ServiceBusConnection")] string id,
        FunctionContext context)
    {
        _logger.LogInformation("Start Processing the Message");
        var db = _redis.GetDatabase();

        var message = await db.StringGetAsync(id);

        if (message.IsNullOrEmpty)
        {
            _logger.LogWarning($"Message not found in Redis for ID: {id}");
            return;
        }

        try
        {
            _logger.LogInformation($"Processing message with ID: {id} -> {message}");

            // Simulate some processing
            await Task.Delay(2000);

            // Call API to notify SignalR clients
            var apiBaseUrl = Environment.GetEnvironmentVariable("ApiBaseUrl");

            await _httpClient.PostAsJsonAsync(
                $"{apiBaseUrl}/api/messages/notifycomplete",
                new MessageDto { Message = message + "\n\n Processed" }   // here "id" is going into Message
            );
            // Delete message from Redis after success
            //await db.KeyDeleteAsync(id);

            _logger.LogInformation($"Successfully processed and deleted message ID {id} from Redis");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, $"Failed to process ID {id}, keeping in Redis for retry.");
            throw; // let Service Bus retry
        }
    }
}