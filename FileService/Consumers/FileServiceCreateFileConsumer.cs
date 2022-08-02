using Contracts.File;
using MassTransit;

namespace FileService.Consumers;

public class FileServiceCreateFileConsumer : IConsumer<CreateFile>
{
    private readonly ILogger<FileServiceCreateFileConsumer> _logger;

    public FileServiceCreateFileConsumer(ILogger<FileServiceCreateFileConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateFile> context)
    {
        if (context.Message.ThrowExceptionOn == nameof(FileServiceCreateFileConsumer))
        {
            throw new ConsumerException($"Throwing from {nameof(FileServiceCreateFileConsumer)}");
        }

        switch (context.Message.FileName)
        {
            case "invalid-name":
            {
                await context.Publish<CreateFileFailed>(new
                {
                    context.Message.CorrelationId,
                    Reason = "Invalid filename, please try with 'valid-name'"
                });
                _logger.LogInformation("Unable to create file, invalid name");
                break;
            }

            default:
            {
                var fileId = Guid.NewGuid();
                await context.Publish<CreateFileResolved>(new
                {
                    context.Message.CorrelationId,
                    FileId = fileId
                });
                _logger.LogInformation("File with id {FileId} created", fileId);
                break;
            }
        }
    }
}