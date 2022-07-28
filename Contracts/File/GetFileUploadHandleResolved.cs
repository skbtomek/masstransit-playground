using MassTransit;

namespace Contracts.File;

public interface GetFileUploadHandleResolved : CorrelatedBy<Guid>
{
    string Uri { get; }
}