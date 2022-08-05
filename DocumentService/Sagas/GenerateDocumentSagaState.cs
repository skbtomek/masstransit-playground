using System;
using MassTransit;

namespace DocumentService.Sagas;

public class GenerateDocumentSagaState : SagaStateMachineInstance
{
    public Guid CorrelationId { get; set; }
    public Guid GeneratedDocumentFileId { get; set; }
    
    public string? GenerateDocumentName { get; set; }
    public string? CurrentState { get; set; }
    
    public string? ThrowExceptionOn { get; set; }
}