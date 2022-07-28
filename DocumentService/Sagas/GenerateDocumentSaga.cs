using Contracts.Document;
using Contracts.File;
using DocumentService.Sagas.Activities;
using MassTransit;

namespace DocumentService.Sagas;

public class GenerateDocumentSaga : MassTransitStateMachine<GenerateDocumentSagaState>
{
    public GenerateDocumentSaga(ILogger<GenerateDocumentSaga> logger)
    {
        InstanceState(state => state.CurrentState);

        Initially(
            When(GenerateDocument)
                .Activity(selector => selector.OfType<GenerateDocumentActivity>())
                .PublishAsync(context => context.Init<CreateFile>(new
                {
                    context.Message.CorrelationId,
                    FileName = context.Saga.GenerateDocumentName
                }))
                .TransitionTo(DocumentGeneratedAndSavedLocally));

        During(
            DocumentGeneratedAndSavedLocally,
            When(FileCreated)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("File created");
                    context.Saga.GeneratedDocumentFileId = context.Message.FileId;
                    await context.Publish<GetFileUploadHandle>(new
                    {
                        context.Message.CorrelationId,
                        FileId = context.Saga.GeneratedDocumentFileId
                    });
                })
                .TransitionTo(WaitingForFileUploadHandle));

        During(
            WaitingForFileUploadHandle,
            When(FileUploadHandleResolved)
                .Activity(selector => selector.OfType<UploadFileActivity>())
                .PublishAsync(context => context.Init<GenerateDocumentResolved>(new
                {
                    context.Message.CorrelationId,
                    DocumentFileId = context.Saga.GeneratedDocumentFileId
                }))
                .TransitionTo(Final)
                .Finalize());
    }

    public State? DocumentGeneratedAndSavedLocally { get; set; }
    public State? WaitingForFileUploadHandle { get; set; }


    public Event<GenerateDocument>? GenerateDocument { get; } = null;
    public Event<CreateFileResolved>? FileCreated { get; } = null;
    public Event<GetFileUploadHandleResolved>? FileUploadHandleResolved { get; } = null;
}