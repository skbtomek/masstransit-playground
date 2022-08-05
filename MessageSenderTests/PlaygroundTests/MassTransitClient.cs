using System;
using System.Threading.Tasks;
using Contracts.Document;
using MessageSender.Fixtures;
using SlowConsumer;
using Xunit;
using Xunit.Abstractions;

namespace MessageSender.PlaygroundTests
{
    public class MassTransitClient : IClassFixture<MassTransitFixture>
    {
        private readonly MassTransitFixture _massTransitFixture;

        private readonly Guid _correlationId;

        public MassTransitClient(
            MassTransitFixture massTransitFixture,
            ITestOutputHelper logger)
        {
            _massTransitFixture = massTransitFixture;

            _correlationId = Guid.NewGuid();
            logger.WriteLine($"CorrelationId: {_correlationId}");
        }

        [Fact]
        public async Task Send_valid_generate_document()
        {
            await _massTransitFixture.Bus!.Publish<GenerateDocument>(new
            {
                CorrelationId = _correlationId,
                TemplateFileId = Guid.NewGuid(),
                DataSourceFileId = Guid.NewGuid()
            });
        }

        [Fact]
        public async Task Should_throw_in_initial_state()
        {
            var templateId = Guid.NewGuid();
            var dataSourceFileId = Guid.NewGuid();

            await _massTransitFixture.Bus!.Publish<GenerateDocument>(new
            {
                CorrelationId = _correlationId,
                TemplateFileId = templateId,
                DataSourceFileId = dataSourceFileId,
                ThrowExceptionOn = "GenerateDocumentActivity"
            });
        }

        [Fact]
        public async Task Should_throw_after_initial_state()
        {
            await _massTransitFixture.Bus!.Publish<GenerateDocument>(new
            {
                CorrelationId = _correlationId,
                TemplateFileId = Guid.NewGuid(),
                DataSourceFileId = Guid.NewGuid(),
                ThrowExceptionOn = "FileCreated",
            });
        }

        [Fact]
        public async Task Should_throw_in_file_service()
        {
            await _massTransitFixture.Bus!.Publish<GenerateDocument>(new
            {
                CorrelationId = _correlationId,
                TemplateFileId = Guid.NewGuid(),
                DataSourceFileId = Guid.NewGuid(),
                ThrowExceptionOn = "FileServiceGetFileUploadHandleConsumer",
            });
        }

        [Fact]
        public async Task Slow_consumer()
        {
            var count = 0;
            while (count++ < 1)
            {
                await _massTransitFixture.Publish<SlowMessage>(new
                {
                    CorrelationId = Guid.NewGuid(),
                    Delay = TimeSpan.FromSeconds(60),
                    Number = count
                });
            }
        }
    }
}