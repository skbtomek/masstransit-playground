using MassTransit;

namespace Contracts.File;

[EntityName("get-file-upload-handle-resolved")]
public interface GetFileUploadHandleResolved : CorrelatedBy<Guid>
{
    string Uri { get; }
}