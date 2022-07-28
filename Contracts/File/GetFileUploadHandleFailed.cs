using MassTransit;

namespace Contracts.File;

public interface GetFileUploadHandleFailed : CorrelatedBy<Guid>
{
    string Reason { get; }
}