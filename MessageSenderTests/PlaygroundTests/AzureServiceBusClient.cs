using System;
using System.Text.Json;
using System.Threading.Tasks;
using MessageSender.Fixtures;
using Xunit;
using Xunit.Abstractions;

namespace MessageSender.PlaygroundTests;

public class AzureServiceBusClient : IClassFixture<ServiceBusFixture>
{
    private const string SendTopic = "generate-document";
    private const string ReceiveTopic = "generate-document-resolved";
    private const string ReceiveSubscription = "unit-test-client";
    
    private readonly ServiceBusFixture _serviceBusFixture;
    private readonly ITestOutputHelper _logger;

    private readonly Guid _correlationId;

    public AzureServiceBusClient(
        ServiceBusFixture serviceBusFixture,
        ITestOutputHelper logger)
    {
        _logger = logger;
        _serviceBusFixture = serviceBusFixture;
        _serviceBusFixture.CreateSubscriptionIfNotExists(ReceiveTopic, ReceiveSubscription)
            .GetAwaiter().GetResult();
        
        _correlationId = Guid.NewGuid();
        _logger.WriteLine($"CorrelationId: {_correlationId}");        
    }
    
    [Fact]
    public async Task Send_invalid_message()
    {
        var message = new
        {
            correlationId = _correlationId,
            message = new
            {
                someProperty = "xyz"
            }
        };

        await _serviceBusFixture.SendMessage(SendTopic, message);
    }
    
    [Fact]
    public async Task Send_with_incorrect_message_schema()
    {
        var message = new
        {
            correlationId = _correlationId,
            messageType = new[] { "urn:message:Contracts.Document:GenerateDocument" },
        };

        await _serviceBusFixture.SendMessage(SendTopic, message);
    }

    [Fact]
    public async Task Send_with_invalid_guid()
    {
        var message = new
        {
            correlationId = _correlationId,
            messageType = new[] { "urn:message:Contracts.Document:GenerateDocument" },
            message = new
            {
                correlationId = _correlationId,
                templateFileId = "not-valid-guid",
                dataSourceFileId = Guid.NewGuid(),
            }
        };

        await _serviceBusFixture.SendMessage(SendTopic, message);
    }

    [Fact]
    public async Task Send_valid_message_and_receive()
    {
        var message = new
        {
            correlationId = _correlationId,
            messageType = new[] { "urn:message:Contracts.Document:GenerateDocument" },
            message = new
            {
                correlationId = _correlationId,
                templateFileId = Guid.NewGuid(),
                dataSourceFileId = Guid.NewGuid(),
            }
        };

        await _serviceBusFixture.SendMessage(SendTopic, message);

        _logger.WriteLine("Message sent: {0}",
            JsonSerializer.Serialize(message, new JsonSerializerOptions
            {
                WriteIndented = true
            }));

        var receivedMessage = await _serviceBusFixture.ReceiveMessage(ReceiveTopic, ReceiveSubscription);
        Assert.NotNull(receivedMessage);
        _logger.WriteLine("Received message body: {0}", GetMessageJsonString(receivedMessage!.Body));
        Assert.Equal(_correlationId, Guid.Parse(receivedMessage.CorrelationId));
    }

    private string GetMessageJsonString(BinaryData data) => JsonSerializer.Serialize(JsonSerializer
            .Deserialize<object>(data),
        new JsonSerializerOptions { WriteIndented = true });
}