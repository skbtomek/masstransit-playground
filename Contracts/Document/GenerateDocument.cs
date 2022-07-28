using MassTransit;

namespace Contracts.Document;

public interface GenerateDocument : CorrelatedBy<Guid>
{
    Guid TemplateFileId { get; }
    Guid DataSourceFileId { get; }
}