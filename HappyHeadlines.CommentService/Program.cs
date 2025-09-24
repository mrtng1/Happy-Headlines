using HappyHeadlines.CommentService.Infrastructure;
using HappyHeadlines.CommentService.Interfaces;
using HappyHeadlines.CommentService.Services;
using Microsoft.EntityFrameworkCore;
using Polly;
using Polly.Extensions.Http;

DotNetEnv.Env.Load();

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddDbContext<CommentDbContext>(options =>
    options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

// Circuit breaker policy
var circuitBreakerPolicy = HttpPolicyExtensions
    .HandleTransientHttpError() // Catches 500, 408, and HttpRequestException
    .CircuitBreakerAsync(
        3, // Amount of exceptions before circuit breaks
        TimeSpan.FromSeconds(30), // How long to keep the circuit broken
        onBreak: (result, timespan) => Console.WriteLine($"Circuit broken for {timespan.TotalSeconds}s due to {result.Exception.Message}"),
        onReset: () => Console.WriteLine("Circuit breaker has been reset."),
        onHalfOpen: () => Console.WriteLine("Circuit breaker is now half-open.")
    );

builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddScoped<ICommentRepository, CommentRepository>();
builder.Services.AddScoped<ICommentService, CommentService>();

string? profanityServiceUrl = builder.Configuration["PROFANITY_SERVICE_URL"];
if (string.IsNullOrEmpty(profanityServiceUrl))
{
    throw new InvalidOperationException("Profanity Service URL is not configured. Please set PROFANITY_SERVICE_URL in your .env file.");
}

// Apply the circuit breaker policy
builder.Services.AddHttpClient<IProfanityClient, ProfanityClient>(client =>
    {
        // base address of profanity service
        client.BaseAddress = new Uri(profanityServiceUrl);
    })
    .AddPolicyHandler(circuitBreakerPolicy);

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();

// migrations on startup
using (var scope = app.Services.CreateScope())
{
    var dbContext = scope.ServiceProvider.GetRequiredService<CommentDbContext>();
    dbContext.Database.Migrate();
}


app.Run();