using MassTransit;

namespace Contracts.File;

public interface GetFileDownloadHandleResolved : CorrelatedBy<Guid>
{
    string Uri { get; }
}