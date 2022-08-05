using Azure.Monitor.OpenTelemetry.Exporter;
using FileService;
using FileService.Consumers;
using MassTransit;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => { cfg.ReadFrom.Configuration(ctx.Configuration); });

builder.Services.AddMassTransit(busConfig =>
{
    busConfig.AddConsumers(typeof(FileServiceCreateFileConsumer).Assembly);

    busConfig.UsingAzureServiceBus((context, config) =>
    {
        config.UseConsumeFilter(typeof(LoggingEnrichmentFilter<>), context);

        config.Host(builder.Configuration.GetConnectionString("ServiceBus"));

        config.ConfigureEndpoints(context);
    });
});
builder.Services.AddOpenTelemetryTracing(tracerBuilder =>
{
    tracerBuilder.SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService("FileService")
                .AddTelemetrySdk()
                .AddEnvironmentVariableDetector())
        .AddSource("MassTransit")
        .AddAspNetCoreInstrumentation()
        .AddAzureMonitorTraceExporter(azureMonitor =>
        {
            azureMonitor.ConnectionString = builder.Configuration.GetConnectionString("ApplicationInsights");
        });
});

var app = builder.Build();

app.Run();