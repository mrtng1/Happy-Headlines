using HappyHeadlines.WebApp.Client.Services;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;

var builder = WebAssemblyHostBuilder.CreateDefault(args);

builder.Services.AddScoped(sp => new HttpClient
{
    // gør relative kald som "/api/..." til samme origin (din NGINX)
    BaseAddress = new Uri(builder.HostEnvironment.BaseAddress)
});
builder.Services.AddScoped<PublisherApi>();
await builder.Build().RunAsync();
