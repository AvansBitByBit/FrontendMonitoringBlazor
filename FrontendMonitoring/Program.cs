using FrontendMonitoring.Components;
using FrontendMonitoring.Models;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using MudBlazor.Services;

var builder = WebApplication.CreateBuilder(args);

// 1. Database configuratie


// 3. MudBlazor
builder.Services.AddMudServices();

// 4. HTTP Clients (voor je eigen services)
builder.Services.AddHttpClient<HttpRequester.HttpRequester>();
builder.Services.AddHttpClient<HttpRequesterOnlyUrl.HttpRequesterOnlyUrl>();

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