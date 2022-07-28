using MassTransit;

namespace Contracts.Document;

public interface GenerateDocumentResolved : CorrelatedBy<Guid>
{
    Guid DocumentFileId { get; }
}