using System;
using MassTransit;

namespace Contracts.File;

[EntityName("create-file")]
public interface CreateFile : CorrelatedBy<Guid>
{
    string FileName { get; }
    string? ThrowExceptionOn { get; }
}