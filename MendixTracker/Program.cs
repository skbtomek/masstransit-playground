using MendixTracker;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

const string sendTopic = "contracts.document/generatedocument";
const string receiveTopic = "contracts.document/generatedocumentresolved";
const string receiveTopicSubscription = "mendix-tracker";

var loggerFactory = LoggerFactory.Create(config => config.AddConsole());

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var busClient = new BusClient(config, loggerFactory.CreateLogger<BusClient>());
await busClient.CreateSubscriptionIfNotExists(receiveTopic, receiveTopicSubscription);

var correlationId = Guid.NewGuid();

await busClient.SendMessage(sendTopic, new
{
    correlationId,
    messageType = new [] {"urn:message:Contracts.Document:GenerateDocument"},
    message = new
    {
        correlationId,
        templateFileId = Guid.NewGuid(),
        dataSourceFileId = Guid.NewGuid(),        
    }
});

await busClient.ReceiveMessage(receiveTopic, receiveTopicSubscription);