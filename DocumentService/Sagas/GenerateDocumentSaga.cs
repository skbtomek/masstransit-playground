using System;
using Contracts.Document;
using Contracts.File;
using DocumentService.Sagas.Activities;
using MassTransit;
using Microsoft.Extensions.Logging;

namespace DocumentService.Sagas;

public class GenerateDocumentSaga : MassTransitStateMachine<GenerateDocumentSagaState>
{
    public GenerateDocumentSaga(ILogger<GenerateDocumentSaga> logger)
    {
        InstanceState(state => state.CurrentState);

        Initially(
            When(GenerateDocument)
                .Activity(selector => selector.OfType<GenerateDocumentActivity>())
                .Then(context =>
                {
                    context.Saga.ThrowExceptionOn = context.Message.ThrowExceptionOn;
                })
                .PublishAsync(context => context.Init<CreateFile>(new
                {
                    context.Message.CorrelationId,
                    FileName = context.Saga.GenerateDocumentName,
                    context.Message.ThrowExceptionOn,
                }))
                .TransitionTo(DocumentGeneratedAndSavedLocally));

        During(
            DocumentGeneratedAndSavedLocally,
            When(FileCreated)
                .ThenAsync(async context =>
                {
                    logger.LogInformation("File created");
                    context.Saga.GeneratedDocumentFileId = context.Message.FileId;
                    if (context.Saga.ThrowExceptionOn == nameof(FileCreated))
                    {
                        throw new Exception("Throwing during consuming CreateFileRequestResolved from FileService....");
                    }
                    await context.Publish<GetFileUploadHandle>(new
                    {
                        context.Message.CorrelationId,
                        FileId = context.Saga.GeneratedDocumentFileId,
                        context.Saga.ThrowExceptionOn,
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