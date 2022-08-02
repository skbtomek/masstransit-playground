using MassTransit;

namespace Contracts.File;

[EntityName("get-file-upload-handle")]
public interface GetFileUploadHandle : CorrelatedBy<Guid>
{
    Guid FileId { get; }
    string? ThrowExceptionOn { get; }
    
}