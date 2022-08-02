using MassTransit;

namespace Contracts.Document;

[EntityName("generate-document-resolved")]
public interface GenerateDocumentResolved : CorrelatedBy<Guid>
{
    Guid DocumentFileId { get; }
}