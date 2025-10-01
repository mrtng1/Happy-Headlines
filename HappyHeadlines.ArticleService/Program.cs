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

// Configure the HTTP request pipeline
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();


app.Run();