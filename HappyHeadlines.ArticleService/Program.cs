using HappyHeadlines.Core.Entities;
using HappyHeadlines.Core.Interfaces;
using HappyHeadlines.Infrastructure;
using HappyHeadlines.Infrastructure.Repositories;
using HappyHeadlines.MonitorService;
using Serilog;

MonitorService.Initialize();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IRepository<Article>, ArticleRepository>();

builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ArticleDbContextFactory>();

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();

app.ApplyMigrations();

app.Run();