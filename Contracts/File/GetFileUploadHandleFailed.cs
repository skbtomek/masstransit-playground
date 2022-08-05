using System;
using MassTransit;

namespace Contracts.File;

[EntityName("get-file-upload-handle-failed")]
public interface GetFileUploadHandleFailed : CorrelatedBy<Guid>
{
    string Reason { get; }
}