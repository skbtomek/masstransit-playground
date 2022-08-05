using System;
using System.Threading.Tasks;
using Contracts.Document;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DocumentService.Sagas.Activities;

public class GenerateDocumentActivity : IStateMachineActivity<GenerateDocumentSagaState, GenerateDocument>
{
    private readonly ILogger<GenerateDocumentActivity> _logger;

    public GenerateDocumentActivity(ILogger<GenerateDocumentActivity> logger)
    {
        _logger = logger;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("generate-document");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<GenerateDocumentSagaState, GenerateDocument> context, 
    IBehavior<GenerateDocumentSagaState, GenerateDocument> next)
    {
        _logger.LogInformation("Template file with id {TemplateFileId} downloaded", context.Message.TemplateFileId);
        _logger.LogInformation("Data source file with id {DataSourceFileId} downloaded", context.Message.DataSourceFileId);

        if (context.Message.ThrowExceptionOn == nameof(GenerateDocumentActivity))
        {
            throw new Exception("I'm throwing from GenerateDocumentActivity!");
        }
        
        _logger.LogInformation("Document generated and saved locally");
        context.Saga.GenerateDocumentName = "new_doc_" + DateTime.Now;

        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(BehaviorExceptionContext<GenerateDocumentSagaState, GenerateDocument, TException> context, IBehavior<GenerateDocumentSagaState, GenerateDocument> next) where TException : Exception
    {
        _logger.LogError(context.Exception, "Document generation failed");
        next.Faulted(context);
        return Task.CompletedTask;
    }
}