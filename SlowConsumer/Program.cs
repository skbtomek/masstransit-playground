using System;
using MassTransit;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

var builder = WebApplication.CreateBuilder(args);

builder.Host.UseSerilog((ctx, cfg) => { cfg.ReadFrom.Configuration(ctx.Configuration); });

builder.Services.AddScoped<SlowConsumer.SlowConsumer>();

builder.Services.AddMassTransit(busConfig =>
{
    busConfig.UsingAzureServiceBus((context, config) =>
    {
        config.Host(builder.Configuration.GetConnectionString("ServiceBus"));
        
        config.ReceiveEndpoint("slow-consumer", e =>
        {
            e.LockDuration = TimeSpan.FromSeconds(5);
            e.Consumer<SlowConsumer.SlowConsumer>(context);
        });
    });
});

var app = builder.Build();
app.Run();
