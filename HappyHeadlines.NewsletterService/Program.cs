using HappyHeadlines.NewsletterService.Jobs;        
using HappyHeadlines.NewsletterService.Data; 
using HappyHeadlines.NewsletterService.Services;    
using Microsoft.EntityFrameworkCore;
using Quartz;
using HappyHeadlines.NewsletterService.Interfaces;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers & OpenAPI ---
builder.Services.AddControllers();
builder.Services.AddOpenApi();

// --- EF Core (SQLite) ---
builder.Services.AddDbContext<AppDb>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("db")));

// --- Messaging (Raw RabbitMQ Client) ---
// 1. Register the core consumer logic as a Singleton
builder.Services.AddSingleton<IArticleConsumer, ArticlePublishedConsumer>();

// 2. Register the hosted service to start and stop the consumer automatically
builder.Services.AddHostedService<ArticleConsumerHostedService>();


// --- Email + Razor templates ---
builder.Services.AddSingleton<IEmailSender, SmtpEmailSender>();
builder.Services.AddSingleton<ITemplateRenderer, RazorTemplateRenderer>();

// --- HttpClient to ArticleService ---
builder.Services.AddHttpClient<ArticleClient>(c =>
{
    var baseUrl = builder.Configuration["ArticleService:BaseUrl"] ?? "http://localhost:5055/";
    c.BaseAddress = new Uri(baseUrl);
});

// --- Quartz (daily digest at 07:00) ---
builder.Services.AddQuartz(q =>
{
    var jobKey = new JobKey("daily-digest");
    q.AddJob<DailyDigestJob>(o => o.WithIdentity(jobKey));
    q.AddTrigger(t => t
        .ForJob(jobKey)
        .WithIdentity("daily-digest-trigger")
        .WithCronSchedule(builder.Configuration["Digest:Cron"] ?? "0 0 7 * * ?"));
});
builder.Services.AddQuartzHostedService();

// --- Health checks ---
builder.Services.AddHealthChecks()
    .AddDbContextCheck<AppDb>("sqlite");

// -------------------------------------------------------

var app = builder.Build();
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDb>();
    await db.Database.MigrateAsync();
}
if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

//app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();