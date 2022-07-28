using System;
using System.Threading.Tasks;
using MassTransit;
using MTMessageSender.Configuration;
using Operate.Events;

namespace MTMessageSender.Fixtures;

public class MessagingFixture : IDisposable
{
    public IBusControl? Bus { get; set; }

    public MessagingFixture()
    {
        StartBus();
    }

    private void StartBus()
    {
        var serviceBusConnectionString = Configuration.ConfigurationInstance
            .GetSection(AsyncApiConfiguration.ServiceBusConnectionString)
            .Value;

        Bus = MassTransit.Bus.Factory.CreateUsingAzureServiceBus(config =>
        { 
            config.Host(serviceBusConnectionString);
        });

        Bus.Start();
    }

    private void StopBus()
    {
        Bus!.Stop();
    }

    public async Task PublishMessageAsync<TMessage>(
        TMessage message)
        where TMessage : class, CorrelatedBy<Guid>
    {
        var publishSendEndpoint = await Bus!.GetPublishSendEndpoint<TMessage>();
        await publishSendEndpoint.Send(message);
    }

    public async Task Publish<T>(object message) where T: class, CorrelatedBy<Guid>
    {
        await Bus!.Publish<T>(message);
    }


    public void Dispose()
    {
        StopBus();
    }
}