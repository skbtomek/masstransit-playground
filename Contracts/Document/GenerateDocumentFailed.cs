using System;
using MassTransit;

namespace Contracts.Document;

[EntityName("generate-document-failed")]
public interface GenerateDocumentFailed : CorrelatedBy<Guid>
{
    string Reason { get; }
}