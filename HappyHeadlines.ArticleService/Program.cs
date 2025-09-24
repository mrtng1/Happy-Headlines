using HappyHeadlines.ArticleService.Infrastructure;
using HappyHeadlines.MonitorService;
using Serilog;
using DbContextFactory = HappyHeadlines.ArticleService.Infrastructure.ArticleDbContextFactory;

DotNetEnv.Env.Load();

MonitorService.Initialize();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

builder.Services.AddScoped<ArticleRepository>();

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