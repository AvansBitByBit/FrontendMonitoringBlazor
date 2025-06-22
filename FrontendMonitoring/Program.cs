using FrontendMonitoring.Components;
using FrontendMonitoring.Models;
using FrontendMonitoring.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Database configuratie
var connectionString = builder.Configuration.GetValue<string>("connectionString");
var apiUrl = builder.Configuration.GetValue<string>("apiUrl"); 


// 3. MudBlazor
builder.Services.AddMudServices();

// 4. HTTP Clients (voor je eigen services)
builder.Services.AddHttpClient<HttpRequester.HttpRequester>();
builder.Services.AddHttpClient<HttpRequesterOnlyUrl.HttpRequesterOnlyUrl>();
builder.Services.AddScoped<ITokenStorage, LocalStorageTokenStorage>();
builder.Services.AddScoped<AuthenticationStateProvider, JwtAuthenticationStateProvider>();
builder.Services.AddAuthorizationCore();

// Register AfvalApiClient as a service
builder.Services.AddScoped<FrontendMonitoring.Services.AfvalApiClient>();

// Register PythonPredictionApiClient as a service
builder.Services.AddHttpClient<FrontendMonitoring.Services.PythonPredictionApiClient>();

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