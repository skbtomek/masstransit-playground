using MassTransit;

namespace Contracts.File;

public interface CreateFileResolved : CorrelatedBy<Guid>
{
    Guid FileId { get;  }
}