using System;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Azure.Messaging.ServiceBus;
using Azure.Messaging.ServiceBus.Administration;
using Microsoft.Extensions.Configuration;

namespace MessageSender.Fixtures;

public class ServiceBusFixture
{
    private readonly string _connectionString;

    public ServiceBusFixture()
    {
        _connectionString = Configuration.ConfigurationInstance
            .GetConnectionString("PlaygroundServiceBus");
    }

    public async Task CreateSubscriptionIfNotExists(string topic, string subscription)
    {
        var administrationClient = new ServiceBusAdministrationClient(_connectionString);
        if (!await administrationClient.SubscriptionExistsAsync(topic, subscription))
        {
            await administrationClient.CreateSubscriptionAsync(topic, subscription);
        }
    }

    public async Task SendMessage(string topic, object messageBody)
    {
        await using var client = new ServiceBusClient(_connectionString);
        await using var sender = client.CreateSender(topic);
        await sender.SendMessageAsync(
            new ServiceBusMessage(Encoding.UTF8.GetBytes(JsonSerializer.Serialize(messageBody))));
    }

    public async Task<ServiceBusReceivedMessage?> ReceiveMessage(string topic, string subscription)
    {
        await using var client = new ServiceBusClient(_connectionString);
        await using var receiver = client.CreateReceiver(topic, subscription);

        return await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(20));
    }

    public async Task<ServiceBusReceivedMessage?> ReceiveMessage(string queue)
    {
        await using var client = new ServiceBusClient(_connectionString);
        await using var receiver = client.CreateReceiver(queue);
        
        return await receiver.ReceiveMessageAsync(TimeSpan.FromSeconds(20));
    }
}