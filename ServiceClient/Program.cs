using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using ServiceClient;

const string sendTopic = "generate-document";
const string receiveTopic = "generate-document-resolved";
const string receiveTopicSubscription = "service-client";

var loggerFactory = LoggerFactory.Create(config => config.AddConsole());

var config = new ConfigurationBuilder()
    .SetBasePath(Directory.GetCurrentDirectory())
    .AddJsonFile("appsettings.json")
    .AddJsonFile("appsettings.Development.json", optional: true)
    .Build();

var busClient = new BusClient(config, loggerFactory.CreateLogger<BusClient>());

await busClient.CreateSubscriptionIfNotExists(receiveTopic, receiveTopicSubscription);

//var correlationId = Guid.NewGuid();
var correlationId = Guid.Parse("409ee5e1-5f0a-4878-9d36-bda81850e834");
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