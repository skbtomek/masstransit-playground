using System;
using System.Threading.Tasks;
using Contracts.Document;
using MassTransit;
using MTMessageSender.Fixtures;
using Operate.Events.Document;
using Xunit;
using Xunit.Abstractions;

namespace MTMessageSender.Tests;

public class DocumentServicePlayground : IClassFixture<MessagingFixture>
{
    private readonly MessagingFixture _messagingFixture;
    private readonly ITestOutputHelper _testOutputHelper;

    public DocumentServicePlayground(MessagingFixture messagingFixture, ITestOutputHelper testOutputHelper)
    {
        _messagingFixture = messagingFixture;
        _testOutputHelper = testOutputHelper;
    }

    [Theory]
    //[InlineData("c840771d-dcdb-48c4-9fb6-fdee91b653ee", "8f3a5652-9a95-49ec-9650-8bc9d4773cb8")]
    [InlineData("2ABD0043-E0D6-4B24-9C7C-0C985E7929AB",
        "8f3a5652-9a95-49ec-9650-8bc9d4773cb8")] // exception with this templateId
    public async Task GenerateDocument(string templateIdText, string dataSourceIdText)
    {
        var correlationId = Guid.NewGuid();
        _testOutputHelper.WriteLine($"CorrelationId: {correlationId}");
        var templateId = Guid.Parse(templateIdText);
        var dataSourceFileId = Guid.Parse(dataSourceIdText);

        await _messagingFixture.Bus!.Publish<GenerateDocument>(new
        {
            CorrelationId = correlationId,
            TemplateFileId = templateId,
            DataSourceFileId = dataSourceFileId
        });
    }
}