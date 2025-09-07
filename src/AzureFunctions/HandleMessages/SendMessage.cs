using Azure.Messaging.ServiceBus;
using HandleMessages.Dtos;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Azure.Functions.Worker;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;

namespace HandleMessages;

public class SendMessage
{
    private readonly ILogger<SendMessage> _logger;
    private readonly ServiceBusClient _busClient;
    private readonly IConnectionMultiplexer _redis;

    public SendMessage(
        ILogger<SendMessage> logger,
        ServiceBusClient busClient,
        IConnectionMultiplexer redis)
    {
        _logger = logger;
        _busClient = busClient;
        _redis = redis;
    }

    [Function("SendMessage")]
    public async Task<IActionResult> RunAsync([HttpTrigger(AuthorizationLevel.Function, "post")] HttpRequest req)
    {
        _logger.LogInformation("C# HTTP trigger function processed a request.");

        var body = await new StreamReader(req.Body).ReadToEndAsync();
        var data = JsonConvert.DeserializeObject<MessageDto>(body);

        var id = Guid.NewGuid().ToString();

        // Store message in Redis
        var db = _redis.GetDatabase();
        await db.StringSetAsync(id, data.Message);

        // Send only ID into the Service Bus Queue
        var sender = _busClient.CreateSender("sendmessage");
        await sender.SendMessageAsync(new ServiceBusMessage(id));

        _logger.LogInformation($"Stored message with ID {id} in Redis and sent ID to queue.");

        var response = new ResponseDto<string>(true, "Message sent successfully to azure service bus queue.", null);

        return new OkObjectResult(response);
    }
}