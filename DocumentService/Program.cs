using Azure.Monitor.OpenTelemetry.Exporter;
using DocumentService;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) =>
{
    cfg.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddMessaging(builder.Configuration);
builder.Services.AddOpenTelemetryTracing(tracerBuilder =>
{
    tracerBuilder.SetResourceBuilder(
            ResourceBuilder.CreateDefault()
                .AddService("DocumentService")
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