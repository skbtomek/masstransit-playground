using MassTransit;

namespace Contracts.File;

public interface GetFileDownloadHandle : CorrelatedBy<Guid>
{
    Guid FileId { get; }
    
}