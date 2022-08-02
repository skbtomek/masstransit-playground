using MassTransit;

namespace Contracts.Document;

[EntityName("generate-document")]
public interface GenerateDocument : CorrelatedBy<Guid>
{
    Guid TemplateFileId { get; }
    Guid DataSourceFileId { get; }
    
    string ThrowExceptionOn { get; }
}