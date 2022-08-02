using MassTransit;

namespace DocumentService.Sagas;

public class GenerateDocumentSagaDefinition : SagaDefinition<GenerateDocumentSagaState>
{
    protected override void ConfigureSaga(IReceiveEndpointConfigurator endpointConfigurator,
        ISagaConfigurator<GenerateDocumentSagaState> sagaConfigurator)
    {
        //endpointConfigurator.ConfigureDeadLetterQueueErrorTransport();
        //endpointConfigurator.ConfigureDeadLetterQueueDeadLetterTransport();
    }
}