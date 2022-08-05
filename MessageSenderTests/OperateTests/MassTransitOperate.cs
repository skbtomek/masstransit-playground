using System;
using System.Threading.Tasks;
using MessageSender.Fixtures;
using Operate.Events.Document;
using Operate.Events.File;
using Xunit;
using Xunit.Abstractions;

namespace MessageSender.OperateTests;

public class MassTransitOperate : IClassFixture<MassTransitOperateFixture>
{
    private readonly MassTransitFixture _massTransitFixture;
    private readonly ITestOutputHelper _logger;

    private readonly Guid _correlationId;

    public MassTransitOperate(MassTransitOperateFixture massTransitFixture, ITestOutputHelper logger)
    {
        _massTransitFixture = massTransitFixture;
        _logger = logger;

        _correlationId = Guid.NewGuid();
        _logger.WriteLine($"CorrelationId: {_correlationId}");
    }

    [Fact]
    public async Task GenerateDocument()
    {
        var templateId = Guid.Parse("da8e5611-d6f2-470d-a1de-b65a6cf393b8");
        var dataSourceFileId = Guid.Parse("e8ad3e51-1ec2-4562-9e08-c1c3ec4220d4");

        var msgCount = 0;

        while (msgCount++ < 1000)
        {
            await _massTransitFixture.PublishMessageAsync(DocumentEvents.GenerateDocumentRequestSent(
                Guid.NewGuid(), templateId, dataSourceFileId, ContentType.Docx, "ts_output_doc"));            
        }
    }

    [Theory]
    [InlineData("2a0b3a92-4829-47a7-86a6-933bff9ff43b")]
    public async Task GetFileDownloadHandle(string guid)
    {
        var fileId = Guid.Parse(guid);

        await _massTransitFixture.PublishMessageAsync(FileEvents.GetFileDownloadHandleRequestSent(
            _correlationId, fileId, TimeSpan.FromHours(12)));
    }

    [Theory]
    [InlineData("ad4864c5-d914-4cbc-ab61-9eefbd4cfb04")]
    public async Task GetFileUploadHandle(string guid)
    {
        var fileId = Guid.Parse(guid);

        await _massTransitFixture.PublishMessageAsync(FileEvents.GetFileUploadHandleRequestSent(
            _correlationId, fileId, TimeSpan.FromHours(12)));
    }

    [Theory]
    [InlineData("11A1FE65-7AAF-4A1D-AAEB-214F1424AA80")]
    public async Task GetFileMetadata(string guid)
    {
        var fileId = Guid.Parse(guid);

        await _massTransitFixture.PublishMessageAsync(FileEvents.GetFileRequestSent(
            _correlationId, fileId));
    }
}