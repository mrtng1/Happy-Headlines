using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.ArticleService.Infrastructure;
using HappyHeadlines.ArticleService.Services;
using HappyHeadlines.MonitorService;
using HappyHeadlines.ArticleService.Interfaces;
using Serilog;
using DbContextFactory = HappyHeadlines.ArticleService.Infrastructure.ArticleDbContextFactory;

DotNetEnv.Env.Load();

MonitorService.Initialize();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddSingleton<IArticleConsumer, ArticleConsumer>();
builder.Services.AddHostedService<ArticleConsumerService>(); 


builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddSingleton<ArticleDbContextFactory>();

builder.Host.UseSerilog((context, configuration) => 
    configuration.ReadFrom.Configuration(context.Configuration));

var app = builder.Build();

// Create databases and tables on startup
using (var scope = app.Services.CreateScope())
{
    var contextFactory = scope.ServiceProvider.GetRequiredService<ArticleDbContextFactory>();
    var logger = scope.ServiceProvider.GetRequiredService<ILogger<Program>>();
    
    foreach (Continent continent in Enum.GetValues(typeof(Continent)))
    {
        try
        {
            using var context = contextFactory.Create(continent);
            await context.Database.EnsureCreatedAsync();
            logger.LogInformation($"✓ Database ready for {continent}");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, $"Failed to create database for {continent}");
            throw;
        }
    }
}

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

//app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();


app.Run();