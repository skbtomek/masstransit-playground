using System;
using MassTransit;

namespace Contracts.File;

[EntityName("create-file-resolved")]
public interface CreateFileResolved : CorrelatedBy<Guid>
{
    Guid FileId { get;  }
}