using MassTransit;

namespace Contracts.File;

public interface GetFileUploadHandle : CorrelatedBy<Guid>
{
    Guid FileId { get; }
    
}