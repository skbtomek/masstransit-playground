using Contracts.File;
using MassTransit;

namespace FileService.Consumers;

public class GetFileUploadHandleConsumer : IConsumer<GetFileUploadHandle>
{
    private readonly ILogger<GetFileUploadHandleConsumer> _logger;

    public GetFileUploadHandleConsumer(ILogger<GetFileUploadHandleConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetFileUploadHandle> context)
    {
        _logger.LogInformation("File upload handle created for {FileId}", context.Message.FileId);
        await context.Publish<GetFileUploadHandleResolved>(new
        {
            context.Message.CorrelationId,
            Uri = "some-uri"
        });
    }
}