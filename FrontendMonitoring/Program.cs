using FrontendMonitoring.Components;
using FrontendMonitoring.Models;
using FrontendMonitoring.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Database configuratie
var connectionString = builder.Configuration.GetValue<string>("connectionString");
// if (string.IsNullOrEmpty(connectionString))
// {
//     throw new InvalidOperationException("Connection string 'DefaultConnection' is not configured.");
// }

// 3. MudBlazor
builder.Services.AddMudServices();

// 4. HTTP Clients (voor je eigen services)
builder.Services.AddHttpClient<ApiClient>();
builder.Services.AddScoped<AuthenticationService>();
builder.Services.AddScoped<AfvalApiClient>();
builder.Services.AddScoped<WeatherApiClient>();
builder.Services.AddScoped<DetectionService>();

// 5. Razor components (Blazor Server)
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

var app = builder.Build();

// 6. Pipeline configuratie
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseAntiforgery();

app.MapStaticAssets();
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();
