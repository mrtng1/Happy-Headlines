using HappyHeadlines.ArticleService.Entities;
using HappyHeadlines.ArticleService.Infrastructure;
using HappyHeadlines.ArticleService.Services;
using HappyHeadlines.ArticleService.Interfaces;
using Prometheus;
using Serilog;
using StackExchange.Redis;
using DbContextFactory = HappyHeadlines.ArticleService.Infrastructure.ArticleDbContextFactory;


var builder = WebApplication.CreateBuilder(args);

if (builder.Environment.IsDevelopment())
{
    DotNetEnv.Env.Load();
}

builder.Configuration.AddEnvironmentVariables();

builder.Services.AddSingleton<IConnectionMultiplexer>(sp => 
{
    var configuration = sp.GetRequiredService<IConfiguration>();
    string? redisUrl = configuration["REDIS__URL"] ?? configuration["Redis:Url"];

    if (string.IsNullOrEmpty(redisUrl))
    {
        throw new InvalidOperationException("REDIS__URL is not set in the configuration.");
    }
    
    return ConnectionMultiplexer.Connect(redisUrl);
});
builder.Services.AddControllers();

//builder.Services.AddScoped<IArticleRepository, ArticleRepository>();
builder.Services.AddSingleton<IArticleConsumer, ArticleConsumer>();
builder.Services.AddHostedService<ArticleConsumerService>(); 

// register the background service for caching recent articles
builder.Services.AddHostedService<RecentArticleCacheService>();

builder.Services.AddScoped<ArticleRepository>();
builder.Services.AddScoped<IArticleRepository, CachingArticleRepository>(sp => 
{
    // Create the decorator, injecting the original repository and the Redis client
    var dbRepository = sp.GetRequiredService<ArticleRepository>();
    var redisMultiplexer = sp.GetRequiredService<IConnectionMultiplexer>();
    
    var logger = sp.GetRequiredService<ILogger<CachingArticleRepository>>();

    return new CachingArticleRepository(dbRepository, redisMultiplexer, logger);
});

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
            logger.LogInformation($"Database ready for {continent}");
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

app.UseRouting();
app.MapMetrics();

app.UseAuthorization();
app.MapControllers();


app.Run();