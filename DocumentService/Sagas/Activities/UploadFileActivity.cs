using System;
using System.Threading.Tasks;
using Contracts.File;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DocumentService.Sagas.Activities;

public class UploadFileActivity : IStateMachineActivity<GenerateDocumentSagaState, GetFileUploadHandleResolved>
{
    private readonly ILogger<UploadFileActivity> _logger;

    public UploadFileActivity(ILogger<UploadFileActivity> logger)
    {
        _logger = logger;
    }

    public void Probe(ProbeContext context)
    {
        context.CreateScope("upload-file");
    }

    public void Accept(StateMachineVisitor visitor)
    {
        visitor.Visit(this);
    }

    public async Task Execute(BehaviorContext<GenerateDocumentSagaState, GetFileUploadHandleResolved> context,
        IBehavior<GenerateDocumentSagaState, GetFileUploadHandleResolved> next)
    {
        var uri = context.Message.Uri;
        if (uri == "invalid")
        {
            throw new Exception("Invalid URI");
        }

        _logger.LogInformation("File uploaded to {FileUri}", uri);

        await next.Execute(context).ConfigureAwait(false);
    }

    public Task Faulted<TException>(
        BehaviorExceptionContext<GenerateDocumentSagaState, GetFileUploadHandleResolved, TException> context,
        IBehavior<GenerateDocumentSagaState, GetFileUploadHandleResolved> next) where TException : Exception
    {
        _logger.LogError(context.Exception, "Uploading file failed");
        next.Faulted(context);
        return Task.CompletedTask;
    }
}