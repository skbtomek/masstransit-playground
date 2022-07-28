using Contracts.File;
using MassTransit;

namespace FileService.Consumers;

public class CreateFileConsumer : IConsumer<CreateFile>
{
    private readonly ILogger<CreateFileConsumer> _logger;

    public CreateFileConsumer(ILogger<CreateFileConsumer> logger)
    {
        _logger = logger;
    }

    public async Task Consume(ConsumeContext<CreateFile> context)
    {
        switch (context.Message.FileName)
        {
            case "throw-exception":
                _logger.LogError("Just some error that results in exception");
                throw new ConsumerException("Some error");

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