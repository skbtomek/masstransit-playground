using MassTransit;

namespace DocumentService;

internal class LoggingEnrichmentFilter<TMessage> : IFilter<ConsumeContext<TMessage>>
    where TMessage : class
{
    private readonly ILogger<LoggingEnrichmentFilter<TMessage>> _logger;

    public LoggingEnrichmentFilter(ILogger<LoggingEnrichmentFilter<TMessage>> logger) => this._logger = logger;

    public Task Send(ConsumeContext<TMessage> context, IPipe<ConsumeContext<TMessage>> next)
    {
        _logger.LogInformation("Consuming {Event} with CorrelationId {CorrelationId}", context.Message.GetType().Name,
            context.CorrelationId);

        return next.Send(context);
    }

    public void Probe(ProbeContext context) => context.CreateFilterScope(nameof(LoggingEnrichmentFilter<TMessage>));
}