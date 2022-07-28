using System;
using System.Threading.Tasks;
using MassTransit;
using MTMessageSender.Fixtures;
using Operate.Events.Document;
using Operate.Events.File;
using Xunit;
using Xunit.Abstractions;

namespace MTMessageSender.Tests;

public class DocumentService : IClassFixture<MessagingFixture>
{
    private readonly MessagingFixture _messagingFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public DocumentService(MessagingFixture messagingFixture, ITestOutputHelper testOutputHelper)
    {
        _messagingFixture = messagingFixture;
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    //[InlineData("c840771d-dcdb-48c4-9fb6-fdee91b653ee", "8f3a5652-9a95-49ec-9650-8bc9d4773cb8")]
    [InlineData("2ABD0043-E0D6-4B24-9C7C-0C985E7929AB", "8f3a5652-9a95-49ec-9650-8bc9d4773cb8")] // exception with this templateId
    public async Task GenerateDocument(string templateIdText, string dataSourceIdText)
    {
        var correlationId = Guid.NewGuid();
        _testOutputHelper.WriteLine($"CorrelationId: {correlationId}");
        var templateId = Guid.Parse(templateIdText);
        var dataSourceFileId = Guid.Parse(dataSourceIdText);

        await _messagingFixture.PublishMessageAsync(DocumentEvents.GenerateDocumentRequestSent(
            correlationId, templateId, dataSourceFileId, ContentType.Docx, "ts_output_doc"));
    }

    [Theory]
    [InlineData("2a0b3a92-4829-47a7-86a6-933bff9ff43b")]
    // [InlineData("cb4fa3f6-7264-481a-9a01-142fc959dde0")]
    public async Task GetFileDownloadHandle(string guid)
    {
        var fileId = Guid.Parse(guid);

        await _messagingFixture.PublishMessageAsync(FileEvents.GetFileDownloadHandleRequestSent(
            InVar.CorrelationId, fileId, TimeSpan.FromHours(12)));
    }

    [Theory]
    [InlineData("11A1FE65-7AAF-4A1D-AAEB-214F1424AA80")] // forbidden guid
    public async Task GetFileMetadata(string guid)
    {
        var fileId = Guid.Parse(guid);
        var correlationId = InVar.CorrelationId;

        await _messagingFixture.PublishMessageAsync(FileEvents.GetFileRequestSent(
            correlationId, fileId));
    }
}