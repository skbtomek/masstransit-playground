using MassTransit;

namespace Contracts.File;

public interface CreateFile : CorrelatedBy<Guid>
{
    string FileName { get; }
}