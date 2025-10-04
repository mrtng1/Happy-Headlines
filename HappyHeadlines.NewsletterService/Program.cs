using HappyHeadlines.NewsletterService.Consumers;   // ArticlePublishedConsumer
using HappyHeadlines.NewsletterService.Jobs;        // DailyDigestJob
using HappyHeadlines.NewsletterService.Data; // AppDb
using HappyHeadlines.NewsletterService.Services;    // IEmailSender, SmtpEmailSender, ITemplateRenderer, RazorTemplateRenderer
using MassTransit;
using HappyHeadlines.NewsletterService.Consumers;
using HappyHeadlines.NewsletterService.Data;
using HappyHeadlines.NewsletterService.Services;
using Microsoft.EntityFrameworkCore;
using Quartz;

var builder = WebApplication.CreateBuilder(args);

// --- Controllers & OpenAPI ---
builder.Services.AddControllers();
builder.Services.AddOpenApi(); // keeps your current OpenAPI style

// --- EF Core (SQLite) ---
builder.Services.AddDbContext<AppDb>(o =>
    o.UseSqlite(builder.Configuration.GetConnectionString("db")));

// --- MassTransit + RabbitMQ ---
builder.Services.AddMassTransit(x =>
{
    x.AddConsumer<ArticlePublishedConsumer>();

    x.UsingRabbitMq((ctx, cfg) =>
    {
        cfg.Host("host.docker.internal", "/", h => { h.Username("guest"); h.Password("guest"); });

        //cfg.Host(builder.Configuration["Rabbit:Host"] ?? "localhost", "/", h =>
        //{
        //    h.Username(builder.Configuration["Rabbit:User"] ?? "guest");
        //    h.Password(builder.Configuration["Rabbit:Pass"] ?? "guest");
        //});
        cfg.ReceiveEndpoint("ArticleQueue", e =>
        {
            // Bind to an exchange where ArticleService publishes events.
            // Adjust to direct/topic + routing key if needed.
            e.ConfigureConsumeTopology = false;
            e.Bind("article.published", x => { x.ExchangeType = "fanout"; });

            e.ConfigureConsumer<ArticlePublishedConsumer>(ctx);
            e.PrefetchCount = 16;
            e.UseMessageRetry(r => r.Exponential(5,
                TimeSpan.FromSeconds(1),
                TimeSpan.FromMinutes(1),
                TimeSpan.FromSeconds(5)));
        });
    });
});

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

if (app.Environment.IsDevelopment())
{
    app.MapOpenApi();
}

app.UseHttpsRedirection();
app.UseAuthorization();

app.MapControllers();
app.MapHealthChecks("/health");

app.Run();
