using MassTransit;

namespace Contracts.Document;

public interface GenerateDocumentFailed : CorrelatedBy<Guid>
{
    string Reason { get; }
}