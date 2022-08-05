using System;
using System.Threading.Tasks;
using MassTransit;
using MassTransit.AzureServiceBusTransport;
using Microsoft.Extensions.Logging;

namespace SlowConsumer;

public class SlowConsumer :
    IConsumer<SlowMessage>
{
    private readonly ILogger<SlowConsumer> _logger;

    public SlowConsumer(ILogger<SlowConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<SlowMessage> context)
    {
        var started = DateTime.UtcNow;

        var receiveContext = context.GetPayload<ServiceBusReceiveContext>();

        for (var i = 0; i < 10; i++)
        {
            _logger.LogInformation(
                "Message {Msg,16}: (lock: {LockToken,24}, locked-until: {Locked,24}, queued: {Queued,24}, delivery count: {Delivery,4})",
                context.MessageId, receiveContext.LockToken, receiveContext.LockedUntil, started,
                receiveContext.DeliveryCount);
            await Task.Delay(TimeSpan.FromSeconds(5));
        }

        _logger.LogInformation(
            "DONE {Msg,16}: (lock: {LockToken,24}, locked-until: {Locked,24}, queued: {Queued,24}, delivery count: {Delivery,4}, duration:{Duration,24})",
            context.MessageId, receiveContext.LockToken, receiveContext.LockedUntil, started,
            receiveContext.DeliveryCount, DateTime.Now - started);
    }
}