using System.Text;
using System.Text.Json;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace ServiceClient;

public class BusClient
{
    private readonly string _connectionString;
    private readonly ILogger _logger;

    public BusClient(IConfiguration configuration, ILogger logger)
    {
        _logger = logger;
        _connectionString = configuration.GetConnectionString("ServiceBus");
    }

    public async Task CreateSubscriptionIfNotExists(string topic, string subscription)
    {
        var administrationClient = new ServiceBusAdministrationClient(_connectionString);
        if (await administrationClient.SubscriptionExistsAsync(topic, subscription))
        {
            _logger.LogInformation(
                "Subscription {Subscription} already exists on topic {Topic}", subscription, topic);
        }
        else
        {
            await administrationClient.CreateSubscriptionAsync(topic, subscription);
            _logger.LogInformation(
                "Created subscription {Subscription} on topic {Topic}", subscription, topic);
        }
    }

    public async Task SendMessage(string topic, object messageBody)
    {
        _logger.LogInformation("Sending GenerateDocument message....");

        await using var client = new ServiceBusClient(_connectionString);
        await using var sender = client.CreateSender(topic);
        await sender.SendMessageAsync(
            new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messageBody))));

        _logger.LogInformation("Message sent: {Message}",
            JsonSerializer.Serialize(messageBody, new JsonSerializerOptions { WriteIndented = true }));
    }

    public async Task ReceiveMessage(string topic, string subscription)
    {
        _logger.LogInformation("Starting processing messages...");

        await using var client = new ServiceBusClient(_connectionString);
        await using var processor = client.CreateProcessor(topic, subscription,
            new ServiceBusProcessorOptions { ReceiveMode = ServiceBusReceiveMode.ReceiveAndDelete });

        processor.ProcessMessageAsync += ProcessorOnProcessMessageAsync;
        processor.ProcessErrorAsync += ProcessorOnProcessErrorAsync;

        await processor.StartProcessingAsync();

        Console.WriteLine("Press any key to stop processing messages...");
        Console.ReadKey();

        await processor.StopProcessingAsync();
    }

    private Task ProcessorOnProcessErrorAsync(ProcessErrorEventArgs arg)
    {
        _logger.LogError(arg.Exception, "Processing error on {EntityPath}", arg.EntityPath);
        return Task.CompletedTask;
    }

    private Task ProcessorOnProcessMessageAsync(ProcessMessageEventArgs arg)
    {
        var received = arg.Message;

        if (received != null)
        {
            _logger.LogInformation("Received message body: {Message}", JsonSerializer.Serialize(JsonSerializer
                    .Deserialize<object>(received.Body),
                new JsonSerializerOptions { WriteIndented = true }));
        }

        return Task.CompletedTask;
    }
}