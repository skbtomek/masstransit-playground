namespace MessageSender.Fixtures;

public class MassTransitOperateFixture : MassTransitFixture
{
    protected override string ConnectionStringName => "OperateServiceBus";
}