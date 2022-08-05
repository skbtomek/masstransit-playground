using System;
using MassTransit;

namespace SlowConsumer;

public interface SlowMessage : CorrelatedBy<Guid>
{
}