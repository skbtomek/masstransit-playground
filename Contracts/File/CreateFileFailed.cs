using MassTransit;

namespace Contracts.File;

[EntityName("create-file-failed")]
public interface CreateFileFailed : CorrelatedBy<Guid>
{
    string Reason { get; }
    
}