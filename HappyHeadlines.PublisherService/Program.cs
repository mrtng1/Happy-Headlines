using DotNetEnv;
using HappyHeadlines.PublisherService.Interfaces;
using HappyHeadlines.PublisherService.Services;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;
using OpenTelemetry.Instrumentation.AspNetCore;

Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddOpenTelemetry()
    .WithTracing(tracerProviderBuilder =>
        tracerProviderBuilder
            .AddSource("HappyHeadlines.PublisherService") // Tracer name
            .SetResourceBuilder(
                ResourceBuilder.CreateDefault()
                    .AddService(serviceName: "PublisherService", serviceVersion: "1.0.0"))
            .AddAspNetCoreInstrumentation() 
            //.AddRabbitMQInstrumentation()  
            .AddConsoleExporter());

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<IArticlePublisher, RabbitMqPublisher>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.MapControllers();

app.Run();