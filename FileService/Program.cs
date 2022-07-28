using FileService;
using FileService.Consumers;
using MassTransit;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddMassTransit(busConfig =>
{
    busConfig.AddConsumers(typeof(CreateFileConsumer).Assembly);

    busConfig.SetKebabCaseEndpointNameFormatter();

    busConfig.UsingAzureServiceBus((context, config) =>
    {
        config.UseConsumeFilter(typeof(LoggingEnrichmentFilter<>), context);
        config.Host(builder.Configuration.GetConnectionString("ServiceBus"));
        config.ConfigureEndpoints(context);
    });

});

var app = builder.Build();
app.Run();