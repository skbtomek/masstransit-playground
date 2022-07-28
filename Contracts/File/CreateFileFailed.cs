using MassTransit;

namespace Contracts.File;

public interface CreateFileFailed : CorrelatedBy<Guid>
{
    string Reason { get; }
    
}