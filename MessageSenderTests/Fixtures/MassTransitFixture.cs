using System;
using System.Threading.Tasks;
using MassTransit;
using Microsoft.Extensions.Configuration;

namespace MessageSender.Fixtures;

public class MassTransitFixture : IDisposable
{
    public IBusControl? Bus { get; set; }

    public MassTransitFixture()
    {
        StartBus();
    }

    protected virtual string ConnectionStringName => "PlaygroundServiceBus";

    private void StartBus()
    {
        var serviceBusConnectionString = Configuration.ConfigurationInstance
            .GetConnectionString(ConnectionStringName);

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