using Contracts.File;
using MassTransit;

namespace FileService.Consumers;

public class FileServiceGetFileUploadHandleConsumer : IConsumer<GetFileUploadHandle>
{
    private readonly ILogger<FileServiceGetFileUploadHandleConsumer> _logger;

    public FileServiceGetFileUploadHandleConsumer(ILogger<FileServiceGetFileUploadHandleConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<GetFileUploadHandle> context)
    {
        if (context.Message.ThrowExceptionOn == nameof(FileServiceGetFileUploadHandleConsumer))
        {
            throw new Exception($"Throwing from {nameof(FileServiceGetFileUploadHandleConsumer)}");
        }
        _logger.LogInformation("File upload handle created for {FileId}", context.Message.FileId);
        await context.Publish<GetFileUploadHandleResolved>(new
        {
            context.Message.CorrelationId,
            Uri = "some-uri"
        });
    }
}